# Secret scanning

Use Gitleaks 8.30.1 with `.gitleaks.toml` (default rules plus connection-password and JSON signing/storage-key rules). Obtain it from the official release and verify its published checksum. CI pins the Linux archive checksum in `.github/actions/secret-scan/action.yml`.

From the repository root, with Gitleaks on PATH:

```powershell
./scripts/security/Scan-Secrets.ps1 -Mode WorkingTree
./scripts/security/Scan-Secrets.ps1 -Mode History
./scripts/security/Scan-Secrets.ps1 -Mode Artifacts -ArtifactPath ./Lunoria.Web/dist
```

Use `-GitleaksPath` for an explicit executable path. CI uses PowerShell 7; the script also runs under Windows PowerShell. WorkingTree scans current contents of tracked files, including staged additions, not unrelated untracked files or User Secrets. Add intended new source files to Git before the final local check. Deleted files remain covered by history.

History scans all locally available refs and rejects shallow clones. Fetch authorized branches/tags before a complete audit; this does not inspect remote-only refs, forks, deleted unreachable objects or Git hosting caches. Artifact scans decode to depth 3. A passing scan cannot prove absence of secrets; unsupported formats, obfuscation and missed rules remain possible.

Output is fully redacted; summaries contain only rule IDs, paths, line numbers and commit IDs. Temporary copied sources and redacted reports are removed after each script invocation. Never paste credentials or upload raw reports. History mode alone uses `retired-history.gitleaksignore`: 11 exact historical fingerprints whose keys the user confirmed retired on 2026-10-12. This does not exempt current sources, build artifacts, entire paths or rules. Retirement is user-attested, not independently tested against live services.

## Gates and remaining setup

- `.github/workflows/secret-scan.yml` runs on pull requests, pushes to main and manual dispatch.
- Both deployment workflows scan tracked sources/history before building and resulting artifacts before upload/deployment. Findings and scanner errors stop the job.
- After committing/pushing, verify GitHub Actions runs and configure **Secret scan / Tracked files and history** as a required main-branch check (select its actual reported check name). Local changes do not configure remote branch protection. Protect scanner/workflow changes with code review.
- Reviewed retired historical occurrences are excluded only in history mode. New findings and scanner errors still block deployment; a successful local scan does not establish remote enforcement.

## Historical remediation

1. Privately identify affected credentials/environments. Do not assume committed JWT/storage values are unused because local configuration now differs.
2. Rotate/revoke affected live keys, update all consumers and verify old credentials no longer work. SEC-02's local bearer-token risk acceptance does not automatically accept historical signing/storage keys.
3. Record safe evidence in `Lunoria.Web/PRODUCTION_READINESS_REVIEW.md`.
4. Explicitly decide whether to rewrite history (requires coordination/authorization) or approve a narrowly scoped historical baseline. The current baseline records only the user-confirmed retired occurrences described above; additional findings require separate review. Suppress only specific reviewed historical fingerprints, never entire configuration paths/rules; retain current-file and artifact scans. False-positive exceptions require reasoning too.
5. Rerun scans and verify CI/branch rules before marking SEC-03 complete.

Rotation is essential even if history is rewritten. Ignore rules do not remove already-tracked files or committed secrets. User Secrets is local developer storage, not a deployment secret store.
