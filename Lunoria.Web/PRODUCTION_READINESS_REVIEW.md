# Lunoria Production Readiness and Security Review

Original review: September 19, 2026. Source/dependency recheck and actionable backlog: October 12, 2026.

## Overall assessment

**Not ready for unrestricted public registration yet, even without payments.** There is substantial game functionality, and a rewrite is not necessary based on this review. Account-isolation issues should be resolved before exposing customer content.

This was a read-only source-code review and dependency audit, not a penetration test or inspection of the live Azure configuration. Findings describe the inspected code, not proof of exploitation. No application code was changed and no tests were added. Deployment settings and subsequent code changes need separate verification.

Payments are deferred while security takes priority. The proposed model remains **$5 per month for the host, with free player access**, subject to product decisions. Section 7 is the working checklist; its checkboxes, not the descriptive findings, track completion.

## 1. Security blockers

### High: scene ownership bypass

[SceneService.cs](../Eldoria.Application/Services/SceneService.cs) checks the supplied journey owner's identity for update, delete and dashboard operations, but does not establish that the target scene belongs to that journey. A caller can supply their own journey ID alongside another host's scene ID. Individual scene and scene-list reads accept userId without applying an owner restriction. The inspected controller/repository paths do not supply the missing protection.

Retrieve scenes with an owner-scoped query that binds the actual scene, parent journey and caller together. Cover reads as well as mutations. See ISO-01 through ISO-03.

### High: missing ownership checks on dialogue reads and mutations

Relevant files:

- [DialogPageSectionService.cs](../Eldoria.Application/Services/DialogPageSectionService.cs), particularly deletion around line 60.
- [DialogPageService.cs](../Eldoria.Application/Services/DialogPageService.cs), particularly deletion around line 66.
- [DialogPageSectionController.cs](../Eldoria.Api/Controllers/DialogPageSectionController.cs).
- [DialogPageController.cs](../Eldoria.Api/Controllers/DialogPageController.cs).
- [SceneDialogService.cs](../Eldoria.Application/Services/SceneDialogService.cs) and [SceneDialogController.cs](../Eldoria.Api/Controllers/SceneDialogController.cs).

The endpoints require authentication, but authentication alone does not establish ownership. Section deletion loads a section by ID and removes it without checking its owner. Creation and editing have the same parent-ownership gap. Dialogue-page mutations also lack the current user's identity. The inspected controllers and repositories do not supply the missing protection.

Scene-dialog create/read/edit/delete operations also lack ownership checks. The controller additionally exposes `/api/v1/SceneDialog/give-me-all/{journeyId}`, which ignores the supplied journey ID and reads hard-coded scene IDs. Remove this leftover endpoint.

**Impact:** a signed-in user who obtains another host's resource ID could read, modify or delete dialogue content. Checking a section's selected character owner does not authorize access to its parent page.

**Required changes:**

- Check ownership through the parent scene/journey before every read or mutation that requires host ownership.
- Pass the authenticated user ID into the relevant service methods.
- Prefer owner-scoped repository queries that cannot return another host's resource.
- Audit all endpoints for this same pattern, including nested resources and media operations.
- Return a consistent response for resources the caller cannot access.

Adding `[Authorize]` alone does not fix this issue.

### High: journey creation accepts another host's series

[JourneyService.CreateAsync](../Eldoria.Application/Services/JourneyService.cs) assigns the supplied SeriesId without verifying series ownership. The repository's sort-order transaction does not add that check. Authorize the parent before uploading media or creating the journey. See ISO-08.

### Retired findings: grid prototype

The prototype controller, anonymous prototype hub/session store, and custom 4 MB SignalR allowance were removed. The October 12 source recheck did not find these registrations. They are no longer active blockers; ordinary scene-grid functionality remains. Deployment verification is still required to ensure an old build is not exposed. General guest-hub abuse protection remains relevant.

### High: long-lived sessions without server-side revocation

[AuthService.cs](../Eldoria.Application/Services/AuthService.cs), around line 164, issues ten-day access tokens. [apiClient.ts](src/lib/apiClient.ts), around line 30, stores them in `localStorage`.

No server-side token revocation/security-version check was found. Account locking is checked at login, not during token validation.

**Impact:** a stolen token could remain usable for a long time. Clearing browser storage does not revoke a copied token.

**Required changes:**

- Use shorter-lived access tokens with a controlled renewal strategy.
- Implement server-side session invalidation for password resets and account locking.
- Choose an explicit browser-authentication design. Secure HttpOnly cookies are an option, but require appropriate CSRF protection.
- Add login and registration abuse protection.
- Include long-running connections in the session-expiration/revocation design.

This finding does not establish an existing XSS exploit. Browser-accessible token storage increases the consequences if one occurs.

### Medium: existing guest connections outlive invitation validity

[PlaythroughHub.cs](../Eldoria.Api/PlaythroughRealtime/PlaythroughHub.cs) validates an invitation on joining, but existing group membership is not forcibly removed on expiry/revocation. `SessionClosed` is a client notification, not server-side removal. Current broadcasts contain update reasons rather than full snapshots; HTTP snapshot requests revalidate tokens. Do not describe this as unrestricted post-revocation access to all game data. See GUEST-01 through GUEST-03.

### Important: invitation tokens appear in request logs

[Program.cs](../Eldoria.Api/Program.cs), around line 185, logs request paths. The public snapshot endpoint in [PlaythroughJoinSessionController.cs](../Eldoria.Api/Controllers/PlaythroughJoinSessionController.cs) includes the invitation token in the path.

**Required:** redact sensitive route values from application and infrastructure logs. Apply appropriate cache and referrer policies to invitation pages. Do not log reset tokens, passwords, authorization headers, or billing secrets.

Development HTTP body logging is development-only in the inspected code; ensure production is not configured as Development.

### Immediate credential hygiene

A database password and bearer token were previously shared in the conversation. Rotate the exposed database password if that has not already happened. Treat exposed bearer tokens as compromised while valid. Do not copy those values into documentation or source control.

## 2. Dependencies and abuse protection

### Audit results

| Check | Result on October 12, 2026 |
| --- | --- |
| `npm audit --omit=dev --json` | 36 affected package entries: 5 high, 31 moderate |
| Backend vulnerable-package audit, including transitive dependencies | `System.Text.Json` 8.0.4 flagged high |
| `npm run lint` | Failed with 8 errors, 0 warnings |

The frontend count is **not 36 independent exploits**. Several package entries share underlying advisories. Flagged packages include Tiptap-related packages, ProseMirror, Axios and React Router. Audits are time-sensitive; rerun when implementing fixes.

Installed `prosemirror-view` 1.42.0 is affected by a paste-handling XSS advisory fixed in 1.42.3. The rich-text editor makes this especially relevant; exploit reproduction was not attempted. Reference: [maintainer advisory](https://github.com/ProseMirror/prosemirror-view/security/advisories/GHSA-c8x8-7fp4-3x9w). Patch compatible editor dependencies together rather than blindly forcing upgrades.

Patch affected dependencies and assess reachability. The flagged React Router issue concerns RSC mode, which was not established in this application. The `System.Text.Json` advisory concerns `[JsonExtensionData]`; that attribute was not found in the searched application projects. An advisory match is not proof that the application is exploitable.

Reference: [Microsoft System.Text.Json advisory](https://github.com/dotnet/runtime/security/advisories/GHSA-8g4q-xg66-9fp4).

Resolve the lint failures before making lint a release gate. One reported hook-rule issue involves an API function named `useSceneParticipantConsumable`; assess whether it needs renaming rather than assuming it is an actual React hook violation at runtime.

### Runtime support

The backend targets .NET 8, which reaches end of support on **November 10, 2026**. Apply current security patches and plan an upgrade to a supported runtime, preferably an appropriate LTS version.

Reference: [.NET support policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core).

### Rate limits and storage controls

Required changes:

- Add per-account/IP limits for authentication, password recovery, uploads, and expensive public reads.
- Replace the shared public-viewer request bucket with appropriately partitioned limits so one visitor cannot exhaust everyone's allowance.
- Protect hub method invocations, not just connection-establishment HTTP requests.
- Add per-host storage quotas; per-file limits do not prevent unlimited uploads.
- Monitor unusual storage growth and outbound bandwidth.

[AzureStorageBlob.cs](../Eldoria.Application/Services/AzureStorageBlob.cs), around line 48, requests public blob access when creating the container. Verify the actual Azure configuration and decide whether uploaded media is intentionally public. Public media URLs may remain usable after invitation revocation.

The same file's `DeletePhotoFromUrl` method reports success without deleting. Dialog media has a separate real deletion path with reference checks; do not remove that existing protection. Add safe cleanup for remaining assets that preserves playthrough snapshot references. `ImagesController`/`ImagesService` currently lack owner context for deletion: do not enable real URL-based deletion there before adding ownership verification.

Existing upload checks include per-file size, extension allowlists, MIME and signature checks. These are strengths, not a complete quota or malware/content-validation system. Public snapshots also load a broad playthrough graph and unbounded event logs; optimize/bound them separately from rate limits. Registration reveals email existence, and login/register requests lack explicit maximum password lengths.

## 3. Forgot password

[ForgotPasswordPage.tsx](src/pages/auth/ForgotPasswordPage.tsx) is currently a placeholder. No implemented recovery endpoints were found.

Required before paid launch:

- Request-reset and submit-new-password endpoints.
- Cryptographically random, expiring, single-use tokens, stored only as hashes.
- Atomic token consumption and password update to prevent simultaneous reuse.
- Generic responses that do not reveal whether an email exists.
- Rate limiting and reliable email delivery.
- Reset links constructed from a configured, trusted frontend URL.
- Invalidation of existing sessions after reset.
- A notification that the password changed.
- Request/reset screens with invalid-token, expired-link, and success handling.
- No automatic login merely because the password was reset.

Add host email verification as well. Password recovery and billing management must remain accessible when a subscription expires.

The existing [forgot-password implementation plan](FORGOT_PASSWORD_IMPLEMENTATION.md) describes the proposed implementation; it is not evidence that the feature is complete.

Reference: [OWASP Forgot Password Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Forgot_Password_Cheat_Sheet.html).

## 4. Host-only subscriptions — deferred

Not part of the current security implementation sequence. Required before introducing paid access, not a blocker for a deliberately free, otherwise secure beta.

No implemented subscription enforcement was found. A checkout button alone is insufficient.

For the planned $5/month host subscription with free player access, implement:

- Hosted checkout and a customer portal.
- A database subscription record associated with the authenticated host.
- Server-controlled pricing; never trust a browser-supplied amount or account identity.
- Verified webhook processing with duplicate-event protection and safe out-of-order handling.
- Backend authorization checking both resource ownership and subscription access.
- Appropriate hosting and public-session lifecycle checks, including reconnect behavior.
- Defined policies for payment failure, cancellation, and active games when access expires.
- Preservation of customer game data after cancellation.
- Reconciliation for missed or failed billing updates.
- A customer-facing billing status screen and access to cancellation/payment-method management.

**Do not grant paid access because someone reaches the checkout-success page.** Subscription changes are asynchronous; access must follow verified billing state.

Players should not need their own subscriptions. Their access should follow the invitation and the host's permitted session lifecycle.

The existing [host-subscription implementation plan](HOST_SUBSCRIPTION_IMPLEMENTATION.md) describes the proposed direction.

References: [Stripe subscription webhooks](https://docs.stripe.com/billing/subscriptions/webhooks) and [webhook verification and handling](https://docs.stripe.com/webhooks).

## 5. Deployment and operational readiness

Before release, establish and verify:

| Area | Required outcome |
| --- | --- |
| Environment separation | Separate production database, storage, email, billing credentials, and application settings |
| Secrets | Managed configuration/Key Vault where appropriate, no secrets in frontend bundles, least-privilege database access |
| Recovery | Verified database backups, an actual restore rehearsal, and a media-recovery strategy |
| Monitoring | Alerts for server errors, failed webhooks, email failures, database availability, and unusual spending |
| Errors | Centralized production error responses without stack traces or secrets |
| HTTP security | Verified HTTPS, security headers, restrictive production CORS, and correct proxy configuration |
| Release process | Staging validation, reviewed migrations, deployment gates, and rollback procedures |
| Realtime operation | Documented reconnect, expiry, restart, and multi-instance behavior |

The inspected [API workflow](../.github/workflows/main_lunoria-api-test-jdj01.yml) and [frontend workflow](../.github/workflows/azure-static-web-apps-mango-hill-056fbd20f.yml) build and deploy but do not run lint, tests, or dependency-security checks. They target test-named Azure resources.

The frontend [Static Web Apps configuration](public/staticwebapp.config.json) does not declare security headers. Azure may supply some separately; verify the deployed responses rather than assuming they are absent.

Swagger is enabled unconditionally in the inspected API startup. Decide whether production documentation should be disabled or restricted. Public documentation alone is not an authorization vulnerability.

Validate critical boundaries before release: cross-account access, reset-token reuse, session revocation, duplicate billing events, failed payments, concurrent game actions, and phone reconnection. No tests were added during this review.

## 6. Recommended implementation order

1. **Rotate exposed credentials if outstanding; fix scene/dialogue/series account isolation.**
2. **Patch dependencies and plan the runtime upgrade; stop sensitive logging.**
3. **Harden authentication, guest sessions, rate limits and media controls.**
4. **Implement email verification and forgot password.**
5. **Validate production configuration, backups, monitoring and release gates in staging.**
6. **Run a limited beta. Handle subscriptions as a separate, later milestone.**

## 7. Working TODO checklist

### How to use this list

- Work on one numbered item at a time. IDs remain stable so requests can say, for example, `implement ISO-01`.
- `[ ]` means open or unverified. `[x]` means the scoped change AND its completion check are done. Record date, commit/reference and verification evidence in section 8 before checking an item off.
- If code is complete but staging/manual verification is pending, leave it unchecked and add a progress note. Audit observations are not proof of deployed remediation.
- No application changes, new tests, migrations or cloud changes are authorized by this checklist alone. The existing no-new-tests preference remains; use controlled manual checks with disposable staging accounts unless the user authorizes automated tests.
- Two-account checks use hosts A and B plus their own resources. Valid A operations must work; B's IDs, mismatched parent IDs, missing IDs and anonymous requests must fail without reads/writes to B's data. Do not exercise destructive checks against real customer content.
- Tasks requiring a product decision or Azure access should be marked pending that decision/access, not silently assumed complete. Never record secrets or invitation tokens as evidence.

### P0 — Credential containment (can run alongside account-isolation fixes)

- [x] **SEC-01 — Rotate the exposed database credential.** Owner/deployment action: update secure configuration and retire the old credential. Done when the app connects with the replacement and the old credential is rejected; document evidence without its value. Completed by user confirmation on 2026-10-12; see progress log.
- [x] **SEC-02 — Resolve exposed bearer-token validity.** Closed at the user's explicit request as an accepted local-development risk, not verified remediation. User confirmed the exposed token came from the local API and Azure uses a different signing key. Local token expiry/rejection and key rotation were not verified. Local LAN access and access to the Azure development database remain relevant; reopen if exposure scope changes. No Azure signing-key rotation performed or required solely for this local token exposure.
- [x] **SEC-03 — Scan for committed/bundled secrets.** Check tracked files, relevant Git history and built frontend assets with redacted output. Done when findings are triaged, affected secrets rotated where necessary, and a secret-scanning gate is in place; deletion from a file alone is not remediation. **Closed at the user's request (2026-10-13):** user confirmed historical signing/storage keys retired and the scan implementation committed/pushed. History (206 locally available commits), current tracked files and fresh frontend assets passed locally with 11 exact history-only exceptions; synthetic secrets were rejected. Actual GitHub run success and required branch checks have not been verified; these remain operational follow-ups in section 8, not claimed completion evidence.

### P0 — Account isolation (start implementation here)

- [ ] **ISO-01 — Scope individual scene reads to the owner.** Update SceneService.GetByIdAsync and its repository query. Done when A can read A's scene and cannot read B's scene or grid through the same route.
- [ ] **ISO-02 — Scope scene-list reads to the owner.** Authorize the requested journey in GetListAsync or use an owner-bound query. Done when another host's journey ID cannot enumerate scenes; valid empty journeys still work.
- [ ] **ISO-03 — Bind scene mutation/dashboard authorization to the real parent.** Fix UpdateAsync, DeleteAsync and GetSceneDashboardAsync together, sharing owner-scoped lookup logic. Done when A's journey ID plus B's scene ID cannot read, modify or delete B's scene; reject mismatched same-owner parents too. Confirm normal editing/deletion/dashboard behavior.
- [ ] **ISO-04 — Remove the hard-coded dialogue export route.** Delete `give-me-all` and check for callers. Done when the route no longer resolves and normal dialogue retrieval works.
- [ ] **ISO-05 — Authorize scene-dialog CRUD.** Pass caller identity through SceneDialogController/service/interfaces/repository for create, list, edit and delete. Done when all four operations enforce the owning scene/journey before any change or content return.
- [ ] **ISO-06 — Authorize dialog-page mutations before uploads.** Owner-scope create/update/delete in DialogPageController/service/repository. Done when foreign dialog/page IDs fail before media upload or removal, while normal image/video editing and reference-aware cleanup still work.
- [ ] **ISO-07 — Authorize dialog-section mutations.** Scope create/update/delete to the parent owner; retain selected-character ownership validation. Done when choosing an owned character cannot authorize editing someone else's section/page and narrator sections are also protected.
- [ ] **ISO-08 — Authorize series ownership during journey creation.** Check SeriesId before uploading the photo or allocating sort order. Done when A cannot attach a journey to B's series, missing series fails cleanly, and A's normal creation/order works.
- [ ] **ISO-09 — Audit remaining authoring endpoint boundaries.** Inventory library, character/spell assignments, sync, grids, objectives, chests and events, including draft/reorder endpoints. Done when each operation has documented owner and nested-ID checks; turn each discovered gap into a separate task before closing this audit item. Do not treat userId parameters alone as protection.
- [ ] **ISO-10 — Audit runtime and media endpoint boundaries.** Inventory playthrough/scene/participant/inventory/trade/objective/session/media operations, including linked target IDs. Done when foreign-owner and cross-playthrough IDs cannot cross boundaries; add separate tasks for findings.
- [ ] **ISO-11 — Verify consistent authorization responses.** Choose a consistent inaccessible-resource policy (normally not-found for owner-scoped resources), preserve validation responses for owned resources, and verify frontend handling. Done when the two-account staging matrix covers ISO-01 through ISO-10 without data leakage or unauthorized side effects.

### P0/P1 — Dependencies and supported runtime

- [ ] **DEP-01 — Patch the rich-text dependency family.** Update compatible Tiptap/ProseMirror packages and lockfile. Done when the known paste-XSS/editor advisories no longer match and editing, paste, save/reload and intro viewing pass smoke checks and build.
- [ ] **DEP-02 — Patch Axios.** Update the resolved version and assess advisory reachability (browser versus Node adapters). Done when relevant advisories are cleared or explicitly justified and authenticated JSON, multipart uploads and API errors still work.
- [ ] **DEP-03 — Patch React Router.** Update compatible router packages. Done when the flagged advisory is cleared and authenticated routes, deep links, guest joins and browser back navigation work. The RSC advisory is not proof this SPA uses RSC.
- [ ] **DEP-04 — Remove the vulnerable backend JSON dependency.** Identify the dependency chain bringing System.Text.Json 8.0.4 and update compatibly. Done when the transitive audit clears it, API builds, and representative API JSON responses still serialize correctly.
- [ ] **DEP-05 — Upgrade backend target/deployment runtime.** Choose a supported LTS and update project/package compatibility and CI configuration. Done when the API builds and runs in staging on the selected runtime, including EF and SignalR checks. .NET 8 support ends November 10, 2026; recheck vendor support dates when implementing.
- [ ] **DEP-06 — Establish a reproducible dependency audit baseline.** Audit frontend production AND development tooling, backend transitive packages and deployed runtime. Done when remaining findings have an owner, reachability assessment and remediation/exception expiry; do not equate package counts with independent exploits.

### P1 — Authentication and account lifecycle

- [ ] **AUTH-01 — Decide and document the browser/session design.** Select cookie or bearer/renewal design, token lifetimes and idle/absolute expiry; account for actual frontend/API domains and long sessions. Done when CSRF (if cookies), CORS, browser storage, logout and renewal behavior are specified before implementation.
- [ ] **AUTH-02 — Add server-side session invalidation.** Implement session/security-version storage and request validation, including locked/deleted users. Done when a revoked/locked account's previously valid token is rejected. Generate any migration for the user to apply.
- [ ] **AUTH-03 — Implement short-lived access and safe renewal.** Follow AUTH-01; if refresh tokens are used, hash them, rotate them, handle reuse/revocation and concurrent refresh. Done when expired access cannot be used and valid long gameplay sessions can renew safely.
- [ ] **AUTH-04 — Update browser sign-in/logout/expiry handling.** Remove obsolete persistence from apiClient/AuthProvider as required by the chosen design; implement real logout invalidation. Done when logout, reload, multiple tabs, expiry and invalid renewal behave consistently without exposing credentials.
- [ ] **AUTH-05 — Bound authentication inputs and reduce account enumeration.** Add sensible email/password maximum lengths (do not silently truncate), review registration responses and login timing differences. Done when oversized inputs are rejected before expensive hashing and the agreed non-enumerating flow preserves valid login/registration. Coordinate registration messaging with email delivery/verification.
- [ ] **AUTH-06 — Rate-limit login and registration.** Add per-source and appropriate account-oriented controls, trusted-proxy handling and predictable 429 responses without easy victim-account lockout. Done when a burst is limited without exhausting unrelated users' allowance. In-app limits are required even if edge controls are also used.

### P1 — Logging, invitation and guest access

- [ ] **LOG-01 — Stop sensitive application logging.** Replace raw path/body logging with structured safe fields/redaction. Done when login, registration and guest requests do not put passwords, bearer/invitation tokens or bodies into application logs, including development logging used with real credentials.
- [ ] **LOG-02 — Verify infrastructure log redaction.** Inspect proxy, Azure, telemetry and error-reporting settings with sanitized requests. Done when sensitive route/query values are absent from those stores as well; application redaction alone is insufficient.
- [ ] **GUEST-01 — Enforce revocation/rotation on existing connections.** Use server-enforced connection/group lifecycle, not just SessionClosed instructions. Done when a deliberately non-cooperating connected client receives no further updates after revocation/rotation; invalid HTTP snapshots must remain blocked.
- [ ] **GUEST-02 — Enforce expiry/completion/host invalidation.** Tie connection lifetime and guest snapshot eligibility to invitation expiry, game completion and the agreed host lock/session policy. Done when existing and reconnecting clients obey those boundaries. Current payloads expose reasons, not full snapshots; preserve that narrow broadcast design.
- [ ] **GUEST-03 — Partition public HTTP limits.** Replace the shared 120/min bucket with scoped limits and bounded partition state. Done when one visitor cannot consume every other guest's allowance and normal phone reconnects still work.
- [ ] **GUEST-04 — Limit hub invocations and connections.** Protect JoinPlaythrough and connection churn, set appropriate message/connection limits and monitor rejections. Done when repeated calls over one existing socket are limited; HTTP connection throttling alone is not sufficient.
- [ ] **GUEST-05 — Bound public snapshot work.** Narrow queries to required active/current data, bound/page event history, and coordinate DTO/client changes. Done when large journeys no longer reload unnecessary scene graphs/all logs on every update and public UI still receives intended characters/objectives/events.
- [ ] **GUEST-06 — Set invitation privacy and caching policy.** Configure no-store for sensitive API responses and an appropriate referrer policy on join pages; review third-party requests/browser history exposure. Done when deployed response headers and navigation do not leak invitation credentials through referrers or shared caches. Avoid disabling all static-asset caching unnecessarily.
- [ ] **GUEST-07 — Define restart/multi-instance behavior.** Document the selected hosting topology and how revocation, rate limits and SignalR groups work across instances/restarts. Done when the configured topology is exercised in staging; do not claim distributed guarantees from in-memory state.

### P1 — Media and resource controls

- [ ] **MEDIA-01 — Decide public versus private media policy.** Classify library/runtime images and videos and inspect actual Azure access settings. Done when any intentionally public media is documented and the consequences of invitation revocation are clear; public URLs do not become private when a guest link is revoked.
- [ ] **MEDIA-02 — Enforce the selected media access policy.** Apply storage/access changes and signed/authorized delivery where required, including snapshot assets. Done when direct unauthorized access is denied for private media and host/guest playback still works. Do not blindly disable public storage without updating clients.
- [ ] **MEDIA-03 — Track ownership and enforce storage quotas.** Attribute uploads to a host, enforce total storage/count limits with concurrency-safe reservations/accounting, and handle failed uploads. Done when parallel requests cannot exceed the intended quota and deleting/cleaning assets reconciles usage.
- [ ] **MEDIA-04 — Add upload rate and transport limits.** Align per-account rates, proxy/multipart limits and cancellation with existing image/video limits. Done when oversize/invalid/burst uploads fail predictably without orphan growth or unbounded buffering. Preserve existing extension/MIME/signature checks.
- [ ] **MEDIA-05 — Secure image deletion before enabling it.** Replace arbitrary URL-only deletion with owned asset identifiers/reference checks or remove the unused endpoint. Done when foreign/shared asset deletion is impossible; existing no-op behavior must not silently become an insecure real delete.
- [ ] **MEDIA-06 — Implement reference-aware orphan cleanup.** Cover failed creates/replacements and abandoned uploads while preserving authoring/runtime references. Done when cleanup is retryable, observable, and cannot remove an image still used by a playthrough; retain existing dialog-media safeguards.
- [ ] **MEDIA-07 — Bound other resource growth.** Define maximum pagination sizes, authoring collection/payload sizes and costly operation limits; review rich-text schema/URL validation and media decoding risks. Done when chosen boundaries are enforced server-side and abusive requests fail cheaply. Split newly discovered validation gaps into dedicated tasks.

### P1 — Password recovery and email verification

- [ ] **REC-01 — Configure trusted email delivery.** Select provider, secure credentials, trusted frontend URL and retry/monitoring behavior. Done when staging delivers a safe test message without secrets or recipient contents being unnecessarily logged.
- [ ] **REC-02 — Add recovery-token persistence.** Use cryptographically random tokens with hash-only storage, expiry and consumption state. Done when constraints/concurrency behavior and migration are reviewed; the user applies migrations.
- [ ] **REC-03 — Implement reset-request API.** Generic responses, per-account/IP throttles and trusted reset URLs. Done when existing/nonexistent emails have equivalent outward behavior and links never derive their origin from untrusted request headers.
- [ ] **REC-04 — Implement reset-consumption API.** Atomically consume token/change password, enforce password policy and invalidate sessions through AUTH-02. Done when expired/reused tokens fail and simultaneous redemption permits only one success; queue/send a password-change notification without undoing the reset on mail failure.
- [ ] **REC-05 — Build recovery screens.** Replace placeholder and handle request success, invalid/expired links, password validation and completed reset. Done when links work end-to-end, reset does not automatically authenticate, and recovery remains accessible without subscription access.
- [ ] **REC-06 — Implement email verification.** Add expiring single-use verification, throttled resend, generic responses and an agreed unverified-account policy. Done when forged/reused links fail and legitimate verification works; separate further schema/API/UI work into subtasks if needed.

### P1/P2 — Production configuration and operations

- [ ] **OPS-01 — Separate production resources and secrets.** Configure production DB/storage/email independently, correct environment name and managed secret access/least privilege. Done when deployed configuration is verified without printing values and production is not Development.
- [ ] **OPS-02 — Add safe centralized errors.** Standardize unexpected API errors and request correlation IDs; log diagnostic detail privately. Done when production responses expose no stack traces, connection strings or internal exception details and frontend errors remain useful.
- [ ] **OPS-03 — Restrict production CORS and verify HTTPS/proxies.** Use environment-specific trusted origins, only required credential support, correct forwarded headers and HTTPS/HSTS deployment configuration. Done when actual deployed requests/redirects behave correctly; CORS is not a substitute for authorization.
- [ ] **OPS-04 — Deploy and validate security headers.** Plan CSP (report-only rollout where useful), frame protection, nosniff and referrer policies for the SPA/API. Done when headers are verified on deployed responses and images, video, editor, SignalR and intentional embeds still work. Review cookie/CSRF settings against AUTH-01.
- [ ] **OPS-05 — Restrict production Swagger.** Disable it or require explicitly authorized access. Done when both UI and schema endpoints follow policy; public documentation alone was not an authorization vulnerability.
- [ ] **OPS-06 — Verify database backup and restore.** Document retention/recovery objectives and restore to an isolated environment. Done when a real restore and application smoke check succeeds; configured backups alone are insufficient.
- [ ] **OPS-07 — Verify media recovery.** Configure appropriate blob recovery/retention and rehearse restoring an asset with references intact. Done when recovery works without broad public access or breaking snapshot links.
- [ ] **OPS-08 — Add operational/security alerts.** Cover errors, authentication abuse, rejected limits, DB/storage availability, email failures and spend/bandwidth growth. Done when sample incidents trigger actionable alerts with token-safe logs and a named response owner.
- [ ] **OPS-09 — Document migration/deployment rollback.** Use reviewed migrations, staging rehearsal and an explicit DB-compatible rollback/forward-fix plan. Done when deployment and recovery are rehearsed; do not apply live migrations automatically.
- [ ] **OPS-10 — Add incident-response basics.** Document credential rotation, session revocation, temporary registration shutdown, restore and customer-notification decision paths. Done when the operator can locate and execute the runbook with required permissions.

### P2 — Verification and release gates

- [ ] **REL-01 — Fix the current lint failures.** Address each of the eight reported errors; distinguish misnamed non-hook functions from real hook/state issues. Done when full lint passes without disabling relevant rules to hide failures.
- [ ] **REL-02 — Add CI quality/security gates.** Require frontend typecheck/build/lint, backend build, dependency audits and secret checks; document exceptions and approval policy. Done when a deliberately failing check blocks release. No new automated test suite is implied by this task.
- [ ] **REL-03 — Recheck staging's public attack surface.** Confirm removed prototype/export routes are gone, anonymous access is limited to intended routes, ownership checks pass, and deployed configuration matches source. Done when a sanitized two-account verification record is attached.
- [ ] **REL-04 — Exercise critical state/concurrency flows.** Manually verify duplicate actions, simultaneous saves/trades, refresh/reconnect, invitation expiry/revocation and failed requests in staging. Done when no unauthorized or duplicated state changes occur; log new defects as separate checklist items.
- [ ] **REL-05 — Run a limited beta and release review.** Check all launch-critical items and document any accepted lower-priority risks with owners/dates. Done when outside hosts complete sessions, operational recovery is ready, and release approval is explicit. Payment work can remain deferred for a free beta.

### Deferred — Paid host access (not in the current security milestone)

- [ ] **BILL-01 — Confirm pricing and entitlement policies.** Decide trials/free limits, cancellations, failed payments and active-session grace; preserve customer data and free guest access.
- [ ] **BILL-02 — Add host billing records and hosted checkout.** Bind customer identity to the authenticated owner and price to server configuration; never trust browser account/amount fields.
- [ ] **BILL-03 — Implement verified webhook ingestion.** Verify signatures, make duplicates harmless and handle event ordering/retries. Completion requires sandbox verification, not merely reaching a success page.
- [ ] **BILL-04 — Enforce entitlements server-side.** Cover hosting, mutation and guest-session lifecycle/reconnect without blocking account recovery, cancellation or agreed data access.
- [ ] **BILL-05 — Add billing status and customer portal.** Provide cancellation/payment-method management and clear subscription state without storing card details in this app.
- [ ] **BILL-06 — Add reconciliation and billing operations.** Exercise missed events, payment failure/cancellation, duplicate events and reconciliation; alert on failures before paid launch.

## 8. Progress and verification log

### SEC-03 scan evidence and remaining work (2026-10-12)

- Scanner: checksum-verified Gitleaks 8.30.1, default rules plus `.gitleaks.toml` connection-password and JSON signing/storage-key rules. All output/reports redacted. No credentials were tested against live services and no secret values are recorded here.
- Scope: current contents of tracked files; all locally available Git refs (206 commits, non-shallow repository); existing and freshly rebuilt `Lunoria.Web/dist`. Remote-only refs, forks, unreachable/deleted objects, deployed Azure artifacts and private User Secrets were not scanned.
- Results: current tracked source and fresh frontend assets have no findings under these rules. Historical findings: 11 occurrences, not necessarily 11 distinct credentials. No connection-password finding under the configured rules; this does not prove passwords never existed in history.

| Historical location | Classification / occurrences | Example commit references | Resolution |
| --- | --- | --- | --- |
| `Eldoria.Api/appsettings.json` | JWT signing keys: 4; storage access keys: 3 | `8247abd8`, `081644ab`, `27d224ab`, `240ce8e8` | User confirmed retirement; exact history-only exceptions recorded. |
| `Eldoria.Api/appsettings.Development.json` | JWT signing keys: 2; storage access keys: 1 | `1c29a374`, `27d224ab` | User confirmed retirement; exact history-only exceptions recorded. |
| `Eldoria.Application/Services/AzureStorageBlob.cs` | Storage-service key literal: 1 | `5c1c549c` | User confirmed retirement; exact history-only exception recorded. |

- Added `.github/workflows/secret-scan.yml`, reusable `.github/actions/secret-scan/action.yml`, and `scripts/security/Scan-Secrets.ps1`. Both existing deployment workflows now require source/history scans before build and artifact scans before upload/deploy. Scanner version/archive checksum is pinned; outputs are redacted, and errors fail the gate. Added local-secret ignore patterns without ignoring tracked appsettings files.
- Verified locally: a temporary synthetic signing key and connection password caused exit 1; sample file was removed afterward (no test files added). Initial history scan returned 1. After adding the 11 reviewed fingerprints, history, current-file and fresh artifact scans each returned 0. Frontend build passed with existing SignalR annotation/chunk-size warnings.
- User explicitly confirmed old storage access keys were regenerated/revoked and historical JWT signing keys replaced in every API. This is user-attested retirement; the agent did not independently test old credentials against live services. `scripts/security/retired-history.gitleaksignore` records only exact historical occurrences and is applied only in History mode.
- No blanket allowlist, history rewrite, agent-performed credential rotation, cloud settings change, GitHub ruleset change or deployment was performed. User's existing appsettings/project edits were preserved. Current sources and artifacts remain scanned without historical exceptions.
- **Pushed; remote verification pending (2026-10-13):** user confirmed the commit was pushed. Verify actual GitHub checks before relying on this gate. New findings and scanner errors block deployments; do not bypass failures with continue-on-error or ignore all config files.

SEC-03 evidence and operational follow-ups (checklist closed at user request):

- [x] User confirmed old storage keys regenerated/revoked and JWT signing keys replaced everywhere (2026-10-12). Retirement is attested, not independently live-tested; SEC-02's earlier local bearer-token risk acceptance is separate.
- [x] Implement targeted handling of retired historical findings; history/current/artifact scans pass with no unresolved findings under the configured rules. Only 11 exact historical fingerprints are excluded.
- [x] Commit/push the gate changes: confirmed by the user on 2026-10-13.
- [ ] Verify the actual GitHub runs; require the secret-scan check in branch protection/rulesets. Remote run success and branch enforcement have not been verified.
- [ ] Recheck any additional authorized remote refs/deployment artifacts needed for release coverage; record scope and remaining limitations.

Run instructions and remediation policy: [scripts/security/README.md](../scripts/security/README.md).

| Date | Item | Status / evidence | Remaining work |
| --- | --- | --- | --- |
| 2026-10-12 | Baseline | Read-only source review; npm audit: 36 package entries (5 high/31 moderate); API transitive audit: System.Text.Json 8.0.4 high; lint: 8 errors/0 warnings. | No new fixes or deployed remediation verified. |
| 2026-10-12 | Retired prototype findings | Prototype controller/hub/store and 4 MB override absent from inspected source. | Verify deployed route removal under REL-03. |
| 2026-10-12 | SEC-01 | Completed by user confirmation: database password rotated, connection configuration updated, deployed API connection restored, and sensitive local settings moved to User Secrets. User confirmed completion after the verification checklist was supplied; the agent did not independently test credentials or Azure configuration. | No further SEC-01 work reported. SEC-02 is separately closed by risk acceptance below; SEC-03 repository/history secret review remains unchecked. |
| 2026-10-12 | SEC-02 | Closed by explicit user risk acceptance. User identified the exposed bearer token as local-only and confirmed Azure uses a separate signing key. This is an exception to the usual verified-remediation completion rule. | Token invalidation was not verified. Revisit if the local API becomes publicly reachable, the signing keys are shared, or production-token exposure is identified. General session-hardening tasks remain open. |

| 2026-10-13 | SEC-03 | Closed at user's explicit request after they confirmed the scan implementation was committed/pushed. Local source/history/frontend scans passed; historical keys retired by user attestation, with 11 exact history-only exceptions. | GitHub run success, required branch checks and any additional release scan coverage remain unverified operational follow-ups. |

Add an entry for each implemented item, including date, commit/reference, checks run and any manual/deployment checks still pending. Keep completed tasks in the list; do not delete the history or check off entire groups based on one successful build.

## Existing strengths

The application already has useful foundations: password hashing, default authenticated API access, owner-scoped queries in many areas, and cryptographically generated, hashed QR tokens.

The main issue is inconsistent enforcement around those foundations, not a need to rewrite the game.

**Highest priority: prevent one customer from reading or changing another customer's content before investing in additional game features or launching payments.**
