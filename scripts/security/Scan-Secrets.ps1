param(
    [ValidateSet('WorkingTree', 'History', 'Artifacts')]
    [string]$Mode = 'WorkingTree',
    [string]$GitleaksPath = 'gitleaks',
    [string]$ArtifactPath,
    [string]$HistoryRange = '--all'
)

# Never print file contents or credentials. Gitleaks output is fully redacted.
$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$configPath = Join-Path $repoRoot '.gitleaks.toml'
$scanArguments = @('--config', $configPath, '--redact=100', '--no-banner')
$scanExitCode = 2
$scanTemp = $null
try {
    $scanTemp = Join-Path ([IO.Path]::GetTempPath()) ('lunoria-scan-' + [guid]::NewGuid().ToString('N'))
    New-Item -ItemType Directory -Path $scanTemp | Out-Null
    $reportPath = Join-Path $scanTemp 'redacted-findings.json'
    $scanArguments += @('--report-format', 'json', '--report-path', $reportPath)
    switch ($Mode) {
        'WorkingTree' {
            # Scan current contents of tracked files, not ignored local secrets,
            # dependency directories or unrelated untracked user documents.
            $sourceDirectory = Join-Path $scanTemp 'source'
            New-Item -ItemType Directory -Path $sourceDirectory | Out-Null
            $trackedOutput = & git -C $repoRoot ls-files -z
            if ($LASTEXITCODE -ne 0) { throw 'Unable to enumerate tracked files.' }
            foreach ($relativePath in (($trackedOutput -join "`n") -split "`0")) {
                if (-not $relativePath) { continue }
                $source = Join-Path $repoRoot $relativePath
                if (-not (Test-Path -LiteralPath $source -PathType Leaf)) { continue }
                # Linux treats dotfiles (such as .gitignore) as hidden. Include
                # them explicitly rather than failing or omitting them from scans.
                if ((Get-Item -LiteralPath $source -Force).Attributes -band [IO.FileAttributes]::ReparsePoint) {
                    throw 'Tracked symlinks require explicit security review before scanning.'
                }
                $destination = Join-Path $sourceDirectory $relativePath
                New-Item -ItemType Directory -Force -Path (Split-Path $destination) | Out-Null
                Copy-Item -LiteralPath $source -Destination $destination -Force
            }
            & $GitleaksPath dir $sourceDirectory @scanArguments
            $scanExitCode = $LASTEXITCODE
        }
        'History' {
            $shallow = & git -C $repoRoot rev-parse --is-shallow-repository
            if ($LASTEXITCODE -ne 0 -or $shallow -ne 'false') {
                throw 'History scanning requires a full checkout (fetch-depth: 0).'
            }
            # Only exact, reviewed historical occurrences are excluded. Current
            # sources and artifacts must still reject these values if reintroduced.
            $historyIgnorePath = Join-Path $PSScriptRoot 'retired-history.gitleaksignore'
            & $GitleaksPath git $repoRoot "--log-opts=$HistoryRange" --gitleaks-ignore-path $historyIgnorePath @scanArguments
            $scanExitCode = $LASTEXITCODE
        }
        'Artifacts' {
            if (-not $ArtifactPath) { throw 'ArtifactPath is required.' }
            $artifactDirectory = (Resolve-Path -LiteralPath $ArtifactPath).Path
            if (-not (Test-Path -LiteralPath $artifactDirectory -PathType Container)) {
                throw 'ArtifactPath must be an existing build directory.'
            }
            & $GitleaksPath dir $artifactDirectory @scanArguments --max-decode-depth 3
            $scanExitCode = $LASTEXITCODE
        }
    }
    if (Test-Path -LiteralPath $reportPath) {
        $findings = Get-Content -Raw -LiteralPath $reportPath | ConvertFrom-Json
        $findings | Select-Object RuleID, File, StartLine, Commit | Format-Table -AutoSize
    }
} finally {
    if ($scanTemp -and (Test-Path -LiteralPath $scanTemp)) {
        $resolvedTemp = (Resolve-Path -LiteralPath $scanTemp).Path
        $expectedParent = [IO.Path]::GetFullPath([IO.Path]::GetTempPath()).TrimEnd([IO.Path]::DirectorySeparatorChar)
        if ((Split-Path $resolvedTemp) -ne $expectedParent -or
            (Split-Path $resolvedTemp -Leaf) -notmatch '^lunoria-scan-[a-f0-9]{32}$') {
            throw 'Refusing cleanup outside the generated scan directory.'
        }
        Remove-Item -LiteralPath $resolvedTemp -Recurse -Force
    }
}
if ($scanExitCode -eq 0) {
    Write-Output "Secret scan passed: $Mode"
} else {
    Write-Output "Secret scan failed: $Mode (exit $scanExitCode). Review redacted findings; never paste credentials."
}
exit $scanExitCode
