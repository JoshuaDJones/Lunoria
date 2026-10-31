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

Scene-dialog create/read/edit/delete ownership checks were implemented under ISO-05 on 2026-10-17; the user confirmed successful verification that day. The hard-coded `/api/v1/SceneDialog/give-me-all/{journeyId}` export route was removed from source under ISO-04 on 2026-10-14, together with its temporary Lunoria.Web page and helper. Confirm route removal in the running/deployed API. Dialog-page and section authorization remain separate ISO-06/07 work.

**Impact:** a signed-in user who obtains another host's resource ID could read, modify or delete dialogue content. Checking a section's selected character owner does not authorize access to its parent page.

**Required changes:**

- Check ownership through the parent scene/journey before every read or mutation that requires host ownership.
- Pass the authenticated user ID into the relevant service methods.
- Prefer owner-scoped repository queries that cannot return another host's resource.
- Audit all endpoints for this same pattern, including nested resources and media operations.
- Return a consistent response for resources the caller cannot access.

Adding `[Authorize]` alone does not fix this issue.

### High: journey creation accepts another host's series

[JourneyService.CreateAsync](../Eldoria.Application/Services/JourneyService.cs) now authorizes the supplied SeriesId through `ISeriesRepository.GetForUserAsync` before uploading media or allocating sort order (ISO-08, completed by user verification on 2026-10-17).

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

[PlaythroughHub.cs](../Eldoria.Api/PlaythroughRealtime/PlaythroughHub.cs) now registers invitation-bound local connections. Delivery revalidates persisted invitation eligibility, and invalid connections are aborted server-side; a five-second background sweep checks idle connections too. HTTP snapshots and hub eligibility reject locked/deleted hosts. Implementation added October 17; runtime verification remains pending under GUEST-01/02. Broadcasts still contain only update reasons, not full snapshots. Multi-instance fan-out remains GUEST-07.

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
- [x] **SEC-03 — Scan for committed/bundled secrets.** Check tracked files, relevant Git history and built frontend assets with redacted output. Done when findings are triaged, affected secrets rotated where necessary, and a secret-scanning gate is in place; deletion from a file alone is not remediation. **Completed by user confirmation (2026-10-13):** historical signing/storage keys retired, scan implementation pushed, and all GitHub runs successful after the Linux hidden-file fix. History (206 locally available commits), current tracked files and fresh frontend assets passed locally with 11 exact history-only exceptions; synthetic secrets were rejected. User also confirmed creating the main-branch protection ruleset requiring the scan; see section 8 for evidence and scope limitations.

### P0 — Account isolation (start implementation here)

- [x] **ISO-01 — Scope individual scene reads to the owner.** Update SceneService.GetByIdAsync and its repository query. Done when A can read A's scene and cannot read B's scene or grid through the same route. **Completed by user confirmation (2026-10-13):** after testing the local API with another account, the user confirmed the ownership check works and requested completion. `ISceneRepository.GetForUserAsync` / `SceneRepository.GetForUserAsync` filter by scene ID and the actual journey's UserId, including the grid in that same owner-scoped query. `SceneService.GetByIdAsync` uses it; `SceneController.Get` returns HTTP 404 for both missing and inaccessible scenes. API build passed (0 errors, 62 existing-code warnings). No migration or tests added. Runtime verification was user-performed, not independently observed by the agent; staging verification remains under ISO-11. List/dashboard/mutation fixes remain separate ISO-02/03 tasks.
- [x] **ISO-02 — Scope scene-list reads to the owner.** Authorize the requested journey in GetListAsync or use an owner-bound query. Done when another host's journey ID cannot enumerate scenes; valid empty journeys still work. **Completed by user confirmation (2026-10-14):** user confirmed the fix works after being given the owner/other-account, nonexistent-journey and empty-journey verification steps. `SceneService.GetListAsync` checks the requested journey exists and belongs to the caller before querying scenes/grids. Missing and foreign-owned journeys both return `Journey.NotFound`, mapped to HTTP 404 by `SceneController.List`. Owned journeys retain existing ordering/pagination and return 200 with an empty array when no scenes match. API build passed (0 errors, 62 existing-code warnings); no migration or tests added. Runtime verification was user-performed, not independently observed by the agent; staging verification remains under ISO-11.
- [x] **ISO-03 — Bind scene mutation authorization to the real parent; remove the unused dashboard.** **Closed at the user's explicit request (2026-10-14):** update/delete use a shared owner-scoped repository query requiring the actual journey owner, scene ID and journey ID to match. Invalid combinations return `Scene.NotFound`/HTTP 404 before grid/media operations or changes. Photo replacement checks the scene's filename rather than the journey's. Removed the unused dashboard controller route, service/interface method, repository query/interface method, backend DTO, dashboard-only service dependency, and Lunoria.Web legacy helper/type. Old clients are out of scope and retain obsolete references pending their planned deletion. Release API build passed (0 errors, 60 existing-code warnings); Lunoria.Web build passed with SignalR annotation warnings. No migrations or tests added. **Verification limitation:** update/delete runtime checks have not been confirmed. Under ISO-11, verify foreign-owner and mismatched same-owner scene/journey pairs are rejected without side effects, valid edits/deletions work, and the removed dashboard route no longer resolves after deployment. Use disposable scenes for deletion checks.
- [x] **ISO-04 — Remove the hard-coded dialogue export route.** Delete `give-me-all` and check for callers. Done when the route no longer resolves and normal dialogue retrieval works. **Closed at the user's explicit request (2026-10-14):** removed `SceneDialogController.GetAll`, the `tempGetAllDialogs` helper, its dedicated `AllDialogsPage`, and `/journeys/:journeyId/all-dialogs` frontend route. No navigation links to that temporary page were found. Normal `listSceneDialogs` and `GET /api/v1/SceneDialog/{sceneId}` remain unchanged; their ownership fix belongs to ISO-05. Release API and Lunoria.Web builds passed with existing warnings; no tests or migrations added. Runtime/deployment verification has not been independently observed or explicitly confirmed: confirm the removed route returns 404 with valid authentication and normal dialogue retrieval still works during release verification. Removed code is recoverable through Git.
- [x] **ISO-05 — Authorize scene-dialog CRUD.** Pass caller identity through SceneDialogController/service/interfaces/repository for create, list, edit and delete. Done when all four operations enforce the owning scene/journey before any change or content return. **Completed by user confirmation (2026-10-17):** user confirmed the fix works after receiving the ownership-verification steps. The controller passes authenticated user IDs to all four service methods. Create/list authorize the scene through `ISceneRepository.GetForUserAsync`; list additionally filters its dialogue query by the actual journey owner. Edit/delete retrieve tracked dialogues through a shared predicate on `dialog.Scene.Journey.UserId`. Missing and inaccessible scenes/dialogues produce the same HTTP 404 response per operation, before writes or content return. Successful response envelopes and owned empty-scene lists are preserved. Release API build passed (0 errors, 60 existing-code warnings); no tests, frontend changes or migrations added. Runtime verification was user-performed, not independently observed by the agent; staging verification remains under ISO-11. Dialog-page/section authorization remains ISO-06/07.
- [x] **ISO-06 — Authorize dialog-page mutations before uploads.** Owner-scope create/update/delete in DialogPageController/service/repository. Done when foreign dialog/page IDs fail before media upload or removal, while normal image/video editing and reference-aware cleanup still work. **Closed at the user's explicit request (2026-10-17):** controller passes the authenticated user ID through all three service methods. Create authorizes the parent dialogue with `ISceneDialogRepository.GetForUserAsync` before storage upload. Edit/delete load tracked pages through a shared predicate on `page.SceneDialog.Scene.Journey.UserId`; edit includes sections for existing video validation. Missing and inaccessible resources both return HTTP 404 before storage upload, mutation or deletion. Existing rollback cleanup, post-save orphan cleanup, and global authored/playthrough media-reference checks are unchanged. Authorization precedes Azure storage operations, not ASP.NET multipart request buffering/model binding. Release API build passed (0 errors, 60 existing-code warnings); no tests, frontend changes or migrations added. Runtime verification has not been independently observed or explicitly confirmed. During release verification, check owner create/edit/delete (image and video), foreign and missing IDs returning 404 without database/blob changes, and shared media remaining available to playthrough snapshots. Use disposable pages/media for deletion checks. Section mutation authorization remains ISO-07.
- [x] **ISO-07 — Authorize dialog-section mutations.** Scope create/update/delete to the parent owner; retain selected-character ownership validation. Done when choosing an owned character cannot authorize editing someone else's section/page and narrator sections are also protected. **Implemented and closed at the user's explicit request (2026-10-17); user will perform runtime testing:** create authorizes the parent page using `IDialogPageRepository.GetForUserAsync`. Edit/delete use the new tracked `IDialogPageSectionRepository.GetForUserAsync`, filtering through the section's actual page/dialogue/scene/journey owner. Delete now receives the authenticated user ID. Missing and inaccessible parents/sections return HTTP 404 before mutation or character lookup. Existing selected-character ownership and video-page validation are retained; narrator sections follow the same ownership path. Release API build passed; no tests, frontend changes or migrations added. Pending user checks: owner create/edit/delete succeeds; another account receives 404 with no changes, including requests selecting its own character or using narrator mode; nonexistent IDs return 404; foreign characters remain rejected for an owned page/section. Use disposable sections for delete checks. Runtime verification is not claimed complete; retain staging coverage under ISO-11.
- [x] **ISO-08 — Authorize series ownership during journey creation.** Check SeriesId before uploading the photo or allocating sort order. Done when A cannot attach a journey to B's series, missing series fails cleanly, and A's normal creation/order works. **Completed by user confirmation (2026-10-17):** user confirmed verification after receiving the owned-series, foreign-series and nonexistent-series checks. `JourneyService.CreateAsync` uses the existing owner-scoped `ISeriesRepository.GetForUserAsync` before storage upload or `AddWithNextSortOrderAsync`. Missing and foreign-owned series both return `Series.NotFound`, mapped to HTTP 404 by `JourneyController.Create`. Existing successful creation responses and sort-order allocation are unchanged. Release API build passed (0 errors, 3 existing-code warnings); no tests, frontend changes or migrations added. Runtime verification was user-performed, not independently observed by the agent; staging verification remains under ISO-11. Authorization precedes storage upload, not ASP.NET multipart buffering/model binding.
- [x] **ISO-09 — Audit remaining authoring endpoint boundaries.** Inventory library, character/spell assignments, sync, grids, objectives, chests and events, including draft/reorder endpoints. Done when each operation has documented owner and nested-ID checks; turn each discovered gap into a separate task before closing this audit item. **Source audit completed (2026-10-17):** controller-to-service-to-repository coverage and nested-ID checks are documented in section 9. Two legacy-data isolation gaps were identified and recorded separately as ISO-12/13. Completion means the audit is documented, not that those findings are fixed or runtime security is certified. No application changes, database queries, tests, deployment or credential access were performed for this audit.
- [x] **ISO-10 — Audit runtime and media endpoint boundaries.** Inventory playthrough/scene/participant/inventory/trade/objective/session/media operations, including linked target IDs. Done when the boundary review is documented and findings are assigned to separate remediation tasks. **Audit completed at the user's explicit request (2026-10-17):** see section 10. No new direct runtime ID-substitution bypass identified in reviewed paths. Existing guest lifecycle and media boundary gaps remain open under GUEST-01/02 and MEDIA-01/02/03/05. Completion covers the source audit only, not remediation or verified runtime isolation; runtime verification remains under ISO-11. No gameplay code or live data changed.
- [x] **ISO-11 — Verify consistent authorization responses.** Choose a consistent inaccessible-resource policy (normally not-found for owner-scoped resources), preserve validation responses for owned resources, and verify frontend handling. Done when the two-account staging matrix covers ISO-01 through ISO-10 without data leakage or unauthorized side effects. **Implemented and closed at the user's explicit request (2026-10-25).** API build, frontend TypeScript and targeted lint passed. Live two-account/staging verification has not been confirmed; retain the matrix below as outstanding release verification, not evidence of completed testing.

**ISO-11 implementation (2026-10-25), pending runtime verification:** owner-bound resource lookup failures use resource-specific 404 responses, with the same error code/message for nonexistent and foreign-owned IDs within each operation. Removed `Auth.Forbidden` ownership branches from journey, intro-page, playthrough, scene, chest, event and series services in favor of their matching not-found errors; removed obsolete controller Forbid mappings. Corrected explicit not-found mappings that returned 400 in character/spell reads/deletes, character spell assignments, journey/roster/intro-page routes, scene-character routes, and chest/event routes. Scene creation now maps an inaccessible journey to 404; roster additions and pending-inventory item lookup also map their explicit missing-resource errors. Authentication failures remain 401, validation remains 400, and existing conflicts remain 409. Payload selection/order validation may still return 400 when it treats nonexistent and foreign IDs identically; the policy does not turn every malformed input into a resource lookup.

Frontend review: the active Lunoria.Web client uses Axios rejection and `getApiError` in editor/playthrough error UI; no status-specific 403 ownership dependency was found. Added a neutral fallback for an empty 404 body without clearing the token or redirecting to login. Preserve existing API error messages and conflict handling. Backend Release build passed (0 errors, 60 existing warnings); frontend TypeScript and targeted `apiClient.ts` lint passed. No tests added, migration, live API/browser probes or data changes performed. ISO-12/13 legacy relationship risks are not repaired by response normalization.

#### ISO-11 manual staging matrix (not yet executed)

Use two host accounts A/B, plus two journeys/scenes/playthroughs owned by A. Create disposable resources through normal authorized UI/API operations. Authenticate separately as each account; do not paste credentials/tokens into logs or this document. Use valid request bodies to isolate ownership checks from model-validation failures.

| Checks | Expected outcome |
| --- | --- |
| Repeat library, series, journey and scene GET/edit/delete as owner A, then B, then with a nonexistent ID | Owner succeeds; foreign and missing resource lookups return matching 404 code/message. Rejected mutations change nothing. Use disposable resources for deletion. |
| Journey-dependent scene creation/list/order, intro pages, playthrough list/start | Foreign/nonexistent journey has matching 404; valid owner path still works. Creation rejection must not upload media or complete an existing playthrough. |
| Scene dialogs, pages, sections, grids, objectives, chests/loot, events/actions/drafts/order | Foreign root/parent rejected; nested child from another parent must not be read or changed. Missing/foreign parent responses match. Include narrator sections, uploads, drafts and reorder. |
| Journey/scene characters, spell assignments, alternate selection, sync preview/apply/acknowledge | Foreign assignment lookup gives 404; foreign/nonexistent selected references are rejected consistently (404 lookup or 400 field validation). Stale sync review remains 409. No automatic sync on read. |
| Same-owner mismatched journey/scene, scene/event/chest/page, playthrough/scene combinations | Rejected despite both resources being owned by A. Route ownership alone must not authorize a child from another parent. |
| Runtime start/end, activation/removal, stats/movement/transform/campfire/forfeit, attack/counterattack, chests/consumables/trade, pending inventory and objectives | Foreign root gets 404; substitute nested IDs from another scene/playthrough and confirm rejection without changed stats/inventory/turn/logs. State or field validation may reject first; use eligible game state for targeted ID checks. |
| Owned invalid inputs and conflicts | Malformed requests remain 400; stale review, completed-game and other existing conflict paths retain their intended 409. Successful responses are unchanged. |
| Unauthenticated host routes and removed routes | Host routes require authentication (401). Removed dashboard/temporary all-dialog routes do not resolve in the deployed build when checked with authenticated requests. |
| Active frontend | Owner editing/play still works; inaccessible deep links display an error, not a success state or login redirect. Failed saves retain the form/error and release busy guards. Empty 404 responses show the neutral fallback. |
| Guest HTTP and sockets | Run the GUEST-01/02 staging checks above, including a client ignoring `SessionClosed`, stale/expired invitations, completion, host lock/deletion and valid reconnects. |

Record route/method, scenario, status, sanitized error code, and side-effect result for each group. Do not mark runtime verification complete based only on compilation. Full release sign-off also requires the separately tracked ISO-12/13 and media-policy work; do not manufacture legacy cross-owner rows in production to test them.

Additional P0 account-isolation work discovered by ISO-09 (stable IDs; address before release):

- [x] **ISO-12 — Guard series reads and deletion against cross-owner journeys.** ISO-08 prevents new mismatched links but does not remediate existing ones. Done when an inconsistent legacy relationship cannot disclose or delete another owner's journey; normal series behavior remains intact. **Implemented and closed at the user's explicit request (2026-10-25):** The user's audit screenshot shows zero mismatched journeys and an empty detail result in the queried database. No repair indicated there at audit time; live API/concurrency verification remains unconfirmed. `SeriesRepository.GetForUserAsync` filters included journeys by caller ownership; both series get/update DTO mappings also filter as protection against tracked navigation fixup. New `DeleteForUserAsync` checks actual series ownership and all attached journeys (ignoring soft-delete filters) inside a serializable transaction, returning `Series.OwnershipConflict`/409 before deletion if any journey has another owner. Valid deletion uses `ExecuteDeleteAsync` with existing database cascades; no unrelated tracked changes are saved. Missing/foreign series still return 404. Media cleanup runs only after successful database deletion. No records are automatically transferred, reparented or repaired. See section 9.

ISO-12 evidence and remaining checks: Release API build passed (0 errors, 60 existing warnings). Existing HomePage deletion error handling displays the 409 message; no frontend change required. The one-off read-only SQL audit returned mismatch counts and administrative IDs only. Its script was removed at the user's request after the audit. The user ran the audit and supplied a screenshot showing zero mismatches and no detail rows. The agent did not query the database; this result does not establish other environments or runtime safeguard behavior. No migration or tests added. In disposable staging data, verify owned normal/empty series get/update/delete still work; foreign/missing series return 404; a legacy mismatched series hides the foreign journey in both GET and update responses and DELETE returns 409 without deleting any journeys or media. Include concurrent relationship changes and confirm database cascades still behave correctly for valid deletes. Obtain explicit approval for any repair based on audit results; do not insert corrupt fixtures or repair records in production.
- [x] **ISO-13 — Guard dialogue reads against legacy cross-owner character references.** Before ISO-07 an owned character could be placed in another owner's page. Done when mismatched legacy character references do not leak data, while legitimate character/narrator sections still render. **Implemented and closed at the user's explicit request (2026-10-25). Legacy-data audit and live runtime verification remain unconfirmed; checklist closure does not certify existing snapshots or completed testing.** `SceneDialogRepository.GetSceneDialogs` loads detached sections without character Includes, then resolves referenced characters through an owner-filtered query (including legitimate dialogue settings). No foreign character data is loaded through this path. Required caller identity is propagated through scene/dialog/page/section DTO mapping, including nested journey scenes and scene-intro mapping, so a populated foreign navigation also maps to `Character = null`. Section text, order, narrator flag, IDs and stored foreign keys are not changed. Existing viewer/editor null-character fallbacks remain usable; no frontend/schema change or tests added. API Release build passed (0 errors, 3 existing mapping warnings on final build). No live API/database checks or repairs performed.

ISO-13 remaining verification: owner dialogue/scene/journey responses retain normal owned character data/settings; narrator and characterless sections still render. In approved disposable staging data, a legacy foreign reference returns no foreign character ID/name/image/stats/settings, while preserving its section text/order; editing the section can select a legitimate owned character. Verify foreign parents still return 404. Rechecked `GetStartAssetsAsync` owner filter and `ValidateStartSource`: new playthroughs reject foreign dialogue character IDs with `Playthrough.InvalidSourceGraph`. Existing snapshots are independent copies and have not been certified clean or rewritten. Audit below before deciding on any separately approved repair.

Read-only ISO-13 audit queries for manual approved use (counts only; not executed by the agent). Authoring table names are singular `ScenePTDialogPage` / `ScenePTDialogSection`; runtime tables are plural. A missing source character does not prove a leak, because snapshots legitimately survive catalog deletion. A nonzero mismatch requires investigation, not automatic deletion or reassignment.

```sql
-- Authoring sections whose character belongs to a different journey owner.
SELECT COUNT_BIG(*) AS MismatchedDialogCharacterCount
FROM dbo.ScenePTDialogSection AS section
JOIN dbo.ScenePTDialogPage AS page ON page.Id = section.DialogPageId
JOIN dbo.SceneDialogs AS dialog ON dialog.Id = page.SceneDialogId
JOIN dbo.Scenes AS scene ON scene.Id = dialog.SceneId
JOIN dbo.Journeys AS journey ON journey.Id = scene.JourneyId
JOIN dbo.Characters AS character ON character.Id = section.CharacterId
WHERE character.UserId <> journey.UserId;

-- Historical snapshot provenance: compare copied characters to surviving sources.
SELECT COUNT_BIG(*) AS SnapshotSourceOwnerMismatchCount
FROM dbo.PlaythroughCharacters AS snapshot
JOIN dbo.Playthroughs AS playthrough ON playthrough.Id = snapshot.PlaythroughId
JOIN dbo.Characters AS source ON source.Id = snapshot.SourceCharacterId
WHERE source.UserId <> playthrough.UserId;

-- Historical runtime dialogue references pointing outside their own playthrough.
SELECT COUNT_BIG(*) AS CrossPlaythroughDialogCharacterCount
FROM dbo.ScenePTDialogSections AS section
JOIN dbo.ScenePTDialogPages AS page ON page.Id = section.DialogPageId
JOIN dbo.ScenePTDialogs AS dialog ON dialog.Id = page.SceneDialogId
JOIN dbo.ScenePTs AS scene ON scene.Id = dialog.ScenePTId
JOIN dbo.PlaythroughCharacters AS character ON character.Id = section.CharacterId
WHERE character.PlaythroughId <> scene.PlaythroughId;
```

### P0/P1 — Dependencies and supported runtime

- [x] **DEP-01 — Patch the rich-text dependency family.** Update compatible Tiptap/ProseMirror packages and lockfile. Done when the known paste-XSS/editor advisories no longer match and editing, paste, save/reload and intro viewing pass smoke checks and build. **Implemented and closed at the user's explicit request (2026-10-25), including the authorized unused-dependency cleanup.** Install, dependency-tree validation and production build passed; no editor-family audit findings remain. Browser smoke checks have not been confirmed and remain outstanding release verification. Other dependency findings are not closed by this item.

**DEP-01 package update implemented (2026-10-25), pending UI smoke verification:** initially all 13 direct Tiptap ranges raised to `^3.32.0`, with the resolved family aligned at 3.32.0; `prosemirror-view` moved from 1.42.0 to 1.42.8. Updated the editor-related lockfile entries without forced/legacy peer resolution or unrelated package-version upgrades. `npm ls --all` reports no dependency problems; `npm ci --dry-run --ignore-scripts --no-audit` succeeds. Production build (including TypeScript) passes with existing SignalR annotation and bundle-size warnings. Final npm audit has no Tiptap/ProseMirror findings, but still reports 12 other package findings (1 moderate, 11 high); this is not a clean whole-app audit. Sources: [ProseMirror paste advisory](https://github.com/ProseMirror/prosemirror-view/security/advisories/GHSA-c8x8-7fp4-3x9w), [Tiptap attribute-merging advisory](https://github.com/ueberdosis/tiptap/security/advisories/GHSA-cp6q-959q-f8rh), [Tiptap Markdown parsing advisory](https://github.com/ueberdosis/tiptap/security/advisories/GHSA-j95f-988m-3j2f). No tests added or browser/DB changes performed. Before closure, verify existing intro content loads; headings/lists/bold/italic/undo work; plain and formatted paste works; saving/reopening preserves content; preview and playthrough intro viewer render/navigation still work. Follow-up cleanup authorized by the user removed 22 unused/redundant direct declarations (19 runtime and 3 development), leaving core/pm/react/starter-kit as the four direct Tiptap packages. Required StarterKit extensions remain transitive. npm install and npm prune succeeded; dependency tree reports no problems and production build still passes with unchanged generated app bundle. Runtime smoke checks remain pending.
- [x] **DEP-02 — Patch Axios.** Update the resolved version and assess advisory reachability (browser versus Node adapters). Done when relevant advisories are cleared or explicitly justified and authenticated JSON, multipart uploads and API errors still work. Closed at the user's explicit request (2026-10-25); browser/API smoke checks remain unconfirmed release verification.

**DEP-02 implemented (2026-10-25), pending runtime verification:** raised the Axios range to `^1.20.0` and resolved Axios 1.18.1 → 1.20.0 and follow-redirects 1.16.0 → 1.16.2. No unrelated package versions changed. Install, `npm ls --all`, and the TypeScript/production build passed (existing SignalR annotation and bundle-size warnings remain). npm audit no longer reports Axios or follow-redirects; 10 other package findings remain (1 moderate, 9 high).

Reachability: Lunoria.Web is a browser SPA using the shared Axios client in `src/lib/apiClient.ts`, with bearer-token request interception and no configured custom adapter. Node HTTP/HTTP2, proxy, and follow-redirects transport advisories do not describe its normal browser request path. Browser/shared header and prototype-pollution hardening still warrants the update; an advisory match does not prove the app has a prototype-pollution entry point. Upload helpers construct native browser FormData. Sources: [Axios 1.20.0 release](https://github.com/axios/axios/releases/tag/v1.20.0), [Axios interceptor/header advisory](https://github.com/axios/axios/security/advisories/GHSA-j8rh-479h-cp32), [follow-redirects header advisory](https://github.com/follow-redirects/follow-redirects/security/advisories/GHSA-q3jh-wrc6-jgq8).

Remaining release verification for DEP-02: manually verify login and an authenticated load/save, a photo upload, and an API validation/not-found error displaying correctly without unexpected logout. These browser/API checks have not been performed. No tests added or authentication/application behavior changed.
- [x] **DEP-03 — Patch React Router.** Update compatible router packages. Done when the flagged advisory is cleared and authenticated routes, deep links, guest joins and browser back navigation work. The RSC advisory is not proof this SPA uses RSC. Closed at the user's explicit request; browser smoke checks remain unconfirmed release verification.

**DEP-03 implemented (2026-10-25), closed at the user's request:** raised `react-router-dom` to `^7.18.4`, resolving both router packages from 7.18.1 to 7.18.4. Only those two package versions changed in this update. Install and TypeScript/production build pass. npm audit cleared both router packages; subsequent frontend dependency cleanup cleared the remaining eight findings. The [RSC CSRF advisory](https://github.com/advisories/GHSA-qwww-vcr4-c8h2) affects unstable RSC APIs; this frontend uses `createBrowserRouter` and `RouterProvider`, with no RSC usage found in application source, so that vulnerable server execution path was not identified here. No route code or tests changed. Remaining release verification: authenticated navigation, refreshing a deep link, joining through a guest link/QR code, and browser back/forward navigation; these browser checks have not been confirmed.
- [x] **DEP-04 — Remove the vulnerable backend JSON dependency.** Identify the dependency chain bringing System.Text.Json 8.0.4 and update compatibly. Done when the transitive audit clears it, API builds, and representative API JSON responses still serialize correctly.

**DEP-04 implemented and closed at the user's explicit request:** added explicit `System.Text.Json` 8.0.6 references to Application and Infrastructure. The original 8.0.4 came through Azure.Storage.Blobs → Azure.Core, SqlClient → Azure.Identity, and EF Design → DependencyModel; both libraries need a patched minimum when restored independently. The API inherits 8.0.6, confirmed in its built `.deps.json`. This preserves the .NET 8 target and existing serializer configuration. [Microsoft's advisory](https://github.com/advisories/GHSA-8g4q-xg66-9fp4) covers denial of service with JsonExtensionData; 8.0.5 is the first patched 8.x package, and 8.0.6 was the latest stable 8.x package returned by NuGet during this update. No JsonExtensionData usage was found in application source; this does not establish dependency-internal reachability.

API build passed (59 warnings, zero errors); solution-wide transitive audit no longer reports System.Text.Json, and the API project reports no vulnerable packages. Independent library graphs still report Caching.Memory 8.0.0 in Application/Infrastructure, plus Azure.Identity 1.10.3, Microsoft.Identity.Client 4.56.0 and System.Formats.Asn1 5.0.0 in Infrastructure. Track these under DEP-05/06; a clean API graph is not a clean solution-wide audit. No tests or migrations added, no database accessed. Remaining unconfirmed release verification: representative scene/playthrough JSON responses and a create/update request using the running API. Deployed runtime validation remains DEP-05; a package patch alone does not prove the Azure runtime is patched.
- [ ] **DEP-05 — Upgrade backend target/deployment runtime.** Choose a supported LTS and update project/package compatibility and CI configuration. Done when the API builds and runs in staging on the selected runtime, including EF and SignalR checks. .NET 8 support ends November 10, 2026; recheck vendor support dates when implementing.
- [ ] **DEP-06 — Establish a reproducible dependency audit baseline.** Audit frontend production AND development tooling, backend transitive packages and deployed runtime. Done when remaining findings have an owner, reachability assessment and remediation/exception expiry; do not equate package counts with independent exploits.

**Additional frontend dependency remediation (2026-10-25):** at the user's request, updated all eight remaining flagged packages within compatible major versions: Vite 8.3.5, PostCSS 8.5.31, source-map-js 1.2.2, brace-expansion 5.0.12, browserslist 4.29.4, nanoid 3.3.20, zod 4.6.7, and baseline-browser-mapping 2.11.33. Raised direct Vite/PostCSS minimum ranges and refreshed their dependency chains in the lockfile. Full npm audit (including development tooling) reports **zero vulnerabilities**. Clean `npm ci --ignore-scripts --no-audit` and production build pass; bundle-size warning remains. `npm ls --all` still reports one extraneous optional tooling package, `@emnapi/wasi-threads@1.2.3`, even after prune and clean installation; no missing/invalid dependency reports. This diagnostic is not an audit vulnerability.

Lint remains unsuccessful with eight errors in unchanged source: PlayHubPage:34 and PlaythroughPage:184 (`set-state-in-effect`); ScenePlaythroughPage:287/464 (`rules-of-hooks`), :582 (`immutability`), :666 (`set-state-in-effect`); PlaythroughGuestPage:91/97 (`set-state-in-effect`). No tests added or application source changes made. Browser smoke verification remains outstanding. This frontend result does not close DEP-05 or the backend/runtime scope of DEP-06; DEP-04 was closed separately.

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
- [x] **GUEST-01 — Enforce revocation/rotation on existing connections.** Use server-enforced connection/group lifecycle, not just SessionClosed instructions. Done when a deliberately non-cooperating connected client receives no further updates after revocation/rotation; invalid HTTP snapshots must remain blocked. **Implemented and closed at the user's explicit request (2026-10-25).** Build passed; live runtime verification has not been confirmed. Retain the staging checks below under ISO-11.
- [x] **GUEST-02 — Enforce expiry/completion/host invalidation.** Tie connection lifetime and guest snapshot eligibility to invitation expiry, game completion and the agreed host lock/session policy. Done when existing and reconnecting clients obey those boundaries. Current payloads expose reasons, not full snapshots; preserve that narrow broadcast design. **Implemented and closed at the user's explicit request (2026-10-25).** Build passed; live runtime verification has not been confirmed. Retain the staging checks below under ISO-11; multi-instance behavior remains GUEST-07.

**GUEST-01/02 implementation (2026-10-17), pending runtime verification:** `GuestConnectionRegistry` retains only invitation hashes and server connection callbacks, never raw invitations. `PlaythroughRealtimeNotifier` no longer broadcasts to permanently authorized playthrough groups: each delivery checks the persisted token hash, revocation, expiry, completion and host `IsLocked`/`IsDeleted` flags. Rotation/revocation notifications revalidate connections, preserving any connection already joined using the new invitation. Invalid connections receive a best-effort `SessionClosed` notification and are aborted regardless of client cooperation. `GuestConnectionMonitor` checks idle connections every five seconds (plus database/processing time); a failed database check sends no update and is retried. HTTP snapshot eligibility applies the same host flags. Already in-flight network messages cannot be recalled.

Host policy: locking/deleting a host denies guest snapshots and connections; ordinary host browser logout does not end a shared invitation. Unlocking can permit a still-valid invitation again. Account/session-version invalidation remains AUTH-02; distributed fan-out/topology verification remains GUEST-07. Registry state is local to the API process; do not assume notification delivery across multiple instances. No migration, frontend changes or tests added. Release API build passed with 0 errors and existing warnings.

Remaining staging verification under ISO-11 despite requested GUEST-01/02 closure: connect a normal phone plus a client that ignores `SessionClosed`; rotate/revoke the invitation and confirm old HTTP requests/rejoins fail and the old socket receives no subsequent gameplay updates. Verify the new invitation still works. Repeat for expiry, playthrough completion (including starting a replacement playthrough), and host locking/deletion, including idle connections. Confirm valid guests still receive updates, normal host logout leaves invitations usable, reconnects validate again, and database outages never result in unvalidated delivery. Use disposable staging data for state changes; these checks have not been performed by the agent or confirmed by the user.
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
- **GitHub runs successful by user confirmation (2026-10-13):** the initial Linux run failed reading hidden `.gitignore`; added `-Force` to source inspection/copying and reran the local source scan successfully. User subsequently confirmed all GitHub runs succeeded. Runs were not independently inspected by the agent. New findings and scanner errors block deployments; do not bypass failures with continue-on-error or ignore all config files.

SEC-03 evidence and operational follow-ups:

- [x] User confirmed old storage keys regenerated/revoked and JWT signing keys replaced everywhere (2026-10-12). Retirement is attested, not independently live-tested; SEC-02's earlier local bearer-token risk acceptance is separate.
- [x] Implement targeted handling of retired historical findings; history/current/artifact scans pass with no unresolved findings under the configured rules. Only 11 exact historical fingerprints are excluded.
- [x] Commit/push the gate changes: confirmed by the user on 2026-10-13.
- [x] Successful GitHub runs confirmed by the user on 2026-10-13 after the Linux hidden-file fix.
- [x] Require the secret-scan check in branch protection/rulesets. On 2026-10-13 the user confirmed creating the ruleset after screenshots showed Active enforcement, an empty bypass list, the Default branch target (confirmed as main), and the required `Tracked files and history` check sourced from GitHub Actions. Enabling pull requests with zero approvals was instructed before creation. Saved settings and an actual blocked merge were not independently inspected/tested by the agent.
- [ ] Recheck any additional authorized remote refs/deployment artifacts needed for release coverage; record scope and remaining limitations.

Run instructions and remediation policy: [scripts/security/README.md](../scripts/security/README.md).

| Date | Item | Status / evidence | Remaining work |
| --- | --- | --- | --- |
| 2026-10-12 | Baseline | Read-only source review; npm audit: 36 package entries (5 high/31 moderate); API transitive audit: System.Text.Json 8.0.4 high; lint: 8 errors/0 warnings. | No new fixes or deployed remediation verified. |
| 2026-10-12 | Retired prototype findings | Prototype controller/hub/store and 4 MB override absent from inspected source. | Verify deployed route removal under REL-03. |
| 2026-10-12 | SEC-01 | Completed by user confirmation: database password rotated, connection configuration updated, deployed API connection restored, and sensitive local settings moved to User Secrets. User confirmed completion after the verification checklist was supplied; the agent did not independently test credentials or Azure configuration. | No further SEC-01 work reported. SEC-02 is separately closed by risk acceptance below; SEC-03 repository/history secret review remains unchecked. |
| 2026-10-12 | SEC-02 | Closed by explicit user risk acceptance. User identified the exposed bearer token as local-only and confirmed Azure uses a separate signing key. This is an exception to the usual verified-remediation completion rule. | Token invalidation was not verified. Revisit if the local API becomes publicly reachable, the signing keys are shared, or production-token exposure is identified. General session-hardening tasks remain open. |

| 2026-10-13 | SEC-03 | Completed by user confirmation: implementation pushed, all GitHub runs successful after fixing Linux hidden-file handling, and main-branch ruleset created with the required scan. Local source/history/frontend scans passed; historical keys retired by user attestation, with 11 exact history-only exceptions. | Saved ruleset enforcement was not independently exercised. Any additional release scan coverage remains subject to the scope limitations above. |

Add an entry for each implemented item, including date, commit/reference, checks run and any manual/deployment checks still pending. Keep completed tasks in the list; do not delete the history or check off entire groups based on one successful build.

## 9. ISO-09 authoring endpoint boundary audit (2026-10-17)

### Scope and method

Read-only source review of current authoring controllers, services, repository predicates, DTO mapping and relevant relationship configuration. The active frontend is Lunoria.Web; obsolete clients are not the security boundary. A route requires protection even when no current frontend calls it.

`Program.cs` sets an authenticated fallback policy. Reviewed authoring controllers propagate `User.GetUserId()` rather than trusting a caller-supplied owner ID. The audit then traced that identity to actual predicates or explicit ownership comparisons, including child IDs, payload references, draft saves and reorder sets. Authentication alone was not counted as authorization.

No live API probes, SQL inspection, cloud inspection, uploads, destructive requests, new tests or application edits were performed. Findings are based on the current source, not proof of exploitation or a guarantee about deployed versions. Existing user edits were preserved. ISO-10 covers playthrough/guest/session/media routes; ISO-11 covers staging behavior and uniform inaccessible-resource responses. Several current authoring routes still distinguish missing/foreign resources using 400/403/404; that existing information-disclosure/consistency work remains explicitly open under ISO-11.

### Library operations

All routes below are relative to `/api/v1`. CRUD means collection GET/POST plus ID GET/PUT/DELETE where present.

| Routes / operations | Owner and linked-ID enforcement | Source / result |
| --- | --- | --- |
| `Character`: CRUD; GET `alternates` | Create assigns caller ownership; list/single/update/delete use owner-filtered character repository queries. Create/update resolve alternate IDs through the caller's character lookup before upload. Alternate selection lists only caller-owned characters; exclude ID only removes a candidate. | `CharacterService`, `CharacterRepository`: checks present. |
| PUT `CharacterSpells/{characterId}` | Owner-filtered character; every distinct requested spell must be returned by the owner-filtered spell query before links change. Archived spells may only remain if already assigned. | `CharacterSpellService.ReplaceCharacterSpells`, `SpellRepository.GetSpellsByIdsForUserAsync`: checks present. |
| `Spell`: CRUD and spell-type list filter | Owner-filtered spell list/single/update/delete; create/update validate the spell type belongs to caller before media/write. List's optional type filter cannot broaden the owner predicate. | `SpellService`, `SpellRepository`, `SpellTypeRepository`: checks present. |
| `SpellType`: CRUD | Caller assigned at creation; list/ID/name uniqueness/update/delete owner-scoped; in-use check follows ownership. | `SpellTypeService`, `SpellTypeRepository`: checks present. |
| `ConsumableItem`: CRUD | Caller assigned at creation; reads/update/delete use `ItemRepository` UserId predicates before storage changes. No client-supplied related resource IDs. | `ConsumableItemService`, `ItemRepository`: checks present. |
| `EquippableItem`: CRUD | Owner-filtered equipment; affected spell type and every distinct granted spell are owner-validated before upload/write. Delete checks ownership before use/deletion checks. | `EquippableItemService.ResolveReferencesAsync`, `EquippableItemRepository`: checks present. |

### Series, journey, scene and assignments

| Routes / operations | Owner and nested-ID enforcement | Source / result |
| --- | --- | --- |
| `Series`: CRUD | Root series and name queries filter UserId; create assigns caller. However, child journeys are included/mapped without checking their owner and deletion cascades. | `SeriesService`, `SeriesRepository`, `SeriesConfig`: **ISO-12** below. |
| `Journey`: CRUD | List filters caller; get/update/delete check actual journey UserId before content return or changes. Create checks series ownership before upload/order allocation (ISO-08). Get loads the graph before the explicit owner comparison but does not return it on failure. | `JourneyService`, `JourneyRepository`: current request checks present; historical parent inconsistencies remain ISO-12. |
| `Scene`: list/get/create/update/delete; PUT `order` | List/create/reorder authorize journey. Single read owner-scoped; update/delete bind scene ID, supplied journey ID and actual owner. Reorder validates unique IDs/contiguous orders and repository verifies exact membership in that journey. | `SceneService`, `SceneRepository`: checks present after ISO-01/02/03. Dashboard removed. |
| `JourneyIntroPage`: list/create/update/delete; PUT `order` | Authorize journey before data/media operations; update/delete additionally require `page.JourneyId == journeyId`. Reorder validates IDs/orders then exact parent membership in repository. | `JourneyIntroPageService.GetOwnershipError`, `JourneyIntroPageRepository.ReorderAsync`: checks present. |
| `JourneyCharacter`: POST `{journeyId}/players`, PUT `{journeyId}` roster replacement | Authorize journey; resolve each requested character with caller ownership before additions/removals. Alternate-only characters rejected. Assignments are attached to the authorized journey. | `JourneyCharacterService.SaveRosterAsync`, `OwnershipRepository`, `CharacterRepository`: checks present. |
| `JourneyCharacter`: PUT `{journeyId}/order` | Authorize journey and require distinct assignment IDs exactly equal to its current roster before changing order. | `JourneyCharacterService.ReorderAsync`: checks present. |
| `JourneyCharacter`: PUT `assignment/{id}`, DELETE `{id}` | Assignment's actual journey owner checked; supplied alternate form independently owner-validated on update. No second parent route ID to substitute. | `JourneyCharacterRepository.GetForUserAsync`, `OwnershipRepository.GetJourneyCharacterAsync`: checks present. |
| POST/DELETE `journey-characters/{id}/spells[/{spellId}]` | Grant validates assignment owner and spell owner; removal binds both assignment and spell ID in an owner-filtered link query. | `JourneyCharacterSpellService`, `JourneyCharacterSpellRepository`: checks present. |
| `SceneCharacter`: list/get/add/update/delete; PUT `{id}/spells` | List/add authorize scene; add validates character owner. Single/update/delete scope assignment through actual scene/journey owner. Alternate form and replacement spells independently owner-validated before writes. | `SceneCharacterService`, `SceneCharacterRepository`, `OwnershipRepository`: checks present. |
| `JourneyCharacter` and `SceneCharacter`: GET `{id}/sync-preview`, POST `{id}/sync`, POST `{id}/acknowledge-base-changes` | Controllers select assignment kind; service loads owner-filtered target and owner-filtered base. Apply/acknowledge use a review token and unit of work; alternate lookup is owner-filtered and copied spells are restricted to the base owner's spells. | `CharacterSyncService.ReviewAsync/ApplyAsync/LoadAsync/ReplaceSpellsAsync`: checks present; review token is concurrency protection, not a substitute for ownership. |
| GET `Journey/{id}/character-sync-summary` | Repository filters root journey by caller even with `IgnoreQueryFilters`; scenes/assignments reached via that journey. Archived bases are intentional. | `JourneyCharacterSyncSummaryService`, `JourneyRepository.GetCharacterSyncSummarySourceAsync`: root check present. As with other graphs, historical linked-data integrity is an assumption to verify, not established by ignoring soft-delete filters. |

### Scene content, drafts and reordering

| Routes / operations | Owner and nested-ID enforcement | Source / result |
| --- | --- | --- |
| `scenes/{sceneId}/grid`: GET/POST/PUT/DELETE | `OwnershipRepository.GetSceneAsync` binds scene to journey owner before grid query or storage operations. Grid is looked up by that scene; no independently supplied grid ID. | `SceneGridService`, `SceneGridRepository`: checks present. |
| `scenes/{sceneId}/objectives`: GET/PUT | `GetTemplateAsync` filters scene ID and actual journey owner. Replacement builds new point/set graph under that scene; input order defines ordering, with no separate child IDs to substitute. | `SceneObjectiveService.ListAsync/ReplaceAsync`, `SceneObjectiveRepository`: checks present. Runtime selection is ISO-10. |
| `scenes/{sceneId}/chests`: list/create/update/delete | List/create authorize actual scene/journey; update/delete load owner-filtered chest and require its SceneId to match the supplied scene. | `SceneChestService.GetOwnedSceneAsync/GetOwnedChestAsync`, `SceneChestRepository`: checks present. |
| Chest POST `draft`, PUT `{chestId}/draft` | Scene and existing chest binding verified. Every loot item's ownership is checked before graph replacement; entries are created under that chest, not accepted as arbitrary child IDs. | `SceneChestService.SaveDraftAsync/ValidateLootEntryAsync`: checks present. |
| Chest loot list/create/update/delete | Scene/chest relationship checked first. Update/delete also require owner-filtered loot entry's SceneChestId to match. Equipment/consumable IDs are owner-filtered before create/update. | `SceneChestLootEntryRepository`, `SceneChestService`: checks present. |
| `scenes/{sceneId}/events`: list/create/update/delete | List/create validate actual scene/journey. Update/delete load owner-filtered event and require SceneId match. | `SceneEventService`, `SceneEventRepository`: checks present. |
| Event POST `draft`, PUT `{eventId}/draft` | Authorized scene/event; any supplied existing action ID must belong to that event and cannot repeat. Every action payload validated before repository transaction replaces graph. | `SceneEventService.SaveDraftAsync`, `SceneEventRepository.SaveDraftAsync`: checks present. |
| Event action create/update/delete | Event must belong to supplied scene and caller. Update/delete require owner-filtered action and matching SceneEventId. | `GetOwnedEventAsync`, `GetActionForUserAsync`: checks present. |
| Event and action PUT `order` | Authorize scene or bound event, validate submitted order, then repository queries children by that exact parent and verifies full ID membership before updates. | `SceneEventService.ReorderAsync/ReorderActionsAsync`, `SceneEventRepository.ReorderAsync`: checks present. |
| Event payload references | Single-character target must be attached to the scene's journey. Alternate character/spell/equipment ownership checked; consumable generic lookup is followed by explicit UserId check. All-target cases cannot smuggle a character ID. Draft and individual writes share validation. | `SceneEventService.ValidateActionAsync`: checks present. |
| `SceneDialog`: create/list/edit/delete | Create/list authorize owning scene; list and tracked edit/delete queries scope through actual journey owner. | ISO-05 implementation; **legacy nested character exception: ISO-13** below. |
| `DialogPage`: create/update/delete | Create authorizes parent dialogue; update/delete scope page through dialogue/scene/journey owner before storage operations. Shared-media reference checks intentionally remain global. | ISO-06 implementation. Runtime/media behavior still requires verification. |
| `DialogPageSection`: create/update/delete | Create authorizes page; edit/delete scope section through actual parent chain. Selected character independently owner-validated; narrator mode does not bypass parent check. | ISO-07 implementation. New mismatched links blocked; old references require ISO-13. |

### Findings requiring separate fixes

#### ISO-12: legacy series/journey owner mismatch

Original October 17 evidence (addressed in source October 25; verification pending): [SeriesRepository.GetForUserAsync](../Eldoria.Infrastructure/Db/Repositories/SeriesRepository.cs) included all `series.Journeys`, and [SeriesService](../Eldoria.Application/Services/SeriesService.cs) mapped all children and removed the root without a mismatch guard. Read/mapping filters and transaction-protected deletion checks are now implemented as documented under ISO-12. [SeriesConfig](../Eldoria.Infrastructure/Db/Configurations/SeriesConfig.cs) still configures cascading deletion of journeys. The foreign key binds SeriesId, not equality of the two UserIds; the schema was not changed.

Before ISO-08, user A could create a journey owned by A under B's series by supplying B's SeriesId. If any such row exists, B's authorized series GET/update response can disclose A's journey data, and deleting B's series can cascade into A's journey. This is a source-confirmed conditional vulnerability, **not evidence that such data exists**. The new creation guard does not clean existing rows. A filtered include alone would not prevent database cascade deletion: the deletion guard and explicit data-remediation decision are necessary too.

#### ISO-13: legacy dialogue-section character owner mismatch

Original October 17 evidence (addressed in authoring source October 25; audit/runtime verification pending): [SceneDialogRepository.GetSceneDialogs](../Eldoria.Infrastructure/Db/Repositories/SceneDialogRepository.cs) authorized only the dialogue root before including characters, and [DialogPageSectionMappings](../Eldoria.Application/Common/DialogPageSectionMappings.cs) serialized them without an owner check. It now resolves only owned characters and requires caller identity in mapping; see ISO-13 implementation and audit queries above.

Before ISO-07, A could create/edit a section on B's page while selecting A's own character: the old character check did not authorize the parent page. If that reference remains, B's otherwise-authorized dialogue read can expose A's character data. ISO-07 blocks that write now, but neither repairs nor hides a previously saved foreign reference. Do not infer ownership from the route alone, and do not silently delete potentially legitimate authored content during remediation. Inspect derived snapshots separately under ISO-10.

### Verification handoff

No new direct owner-check omission was found in the other reviewed current authoring write paths. This statement assumes normal same-owner stored relationships; it is not a blanket assertion that every historical graph is safe. Existing media lifecycle, upload limits, input validation and authentication-hardening tasks remain separate.

For ISO-11, use two accounts and valid request bodies. For every table row, exercise owner success, foreign root ID, nonexistent ID, and (where applicable) an owned parent paired with a different parent's child, including two parents owned by the same account. Try foreign alternate/spell/type/item IDs, duplicate/foreign reorder IDs and foreign draft action IDs. Confirm rejected writes leave both database state and storage unchanged. Explicitly cover empty lists, narrator sections, sync acknowledgements and archived-base review. Test legacy mismatched rows only in isolated, disposable data with approved setup; do not create them in production. Verify deployed removals separately.

## 10. ISO-10 runtime and media boundary review (2026-10-17)

This is a source review, not a live penetration test or a certification of deployed behavior. No database inspection, API probes, uploads, migrations, tests, or application changes were performed. Checks below assume normally constructed stored relationships; they do not prove historical snapshots or manually modified database rows are consistent. Missing/foreign response consistency remains ISO-11.

### Runtime endpoint coverage

Host controllers use authenticated caller IDs under the API fallback authorization policy. Guest snapshot and hub endpoints deliberately allow anonymous access but validate invitation tokens. A guest token is not host authorization and does not authorize gameplay mutations.

| Operations | Boundary traced in source |
| --- | --- |
| Journey playthrough list/create; playthrough details | `PlaythroughService` checks journey ownership before listing/starting. `JourneyRepository.GetPlaythroughSourceAsync` additionally filters journey ID and owner. `PlaythroughRepository` filters lists/details by user and the requested journey/playthrough. |
| Scene details, start, pending start inventory, resolve, end | Repository scene lookups require scene ID, playthrough ID, and `scene.Playthrough.UserId` together. Start/resolve use `GetSceneForStartAsync`; end uses `GetSceneForEndAsync`; details use `GetSceneDetailsAsync`. No reliance on an independently supplied parent ID alone. |
| Add scene instance, activate journey player, add catalog snapshot character | `GetSceneForCharacterInstanceAddAsync` enforces the same three-part root predicate. Selected scene instance is resolved within `scene.SceneCharacters`; journey player within `scene.Playthrough.JourneyCharacters`; catalog snapshot within `scene.Playthrough.Characters`. |
| Remove participant, correct stats, movement, transform, forfeit, campfire | Service and partial files load the owned scene, validate scene state, and locate participant IDs within its participant collection. Transformation uses the assigned alternate rather than accepting an arbitrary replacement character ID. |
| Attack and counterattack | Attacker and target are selected from the loaded scene's participants. Selected spell comes from the attacker's available spells. Counterattack additionally checks the pending token and designated attacker/target; pass resolves the pending counterattack on the owned scene. Turn/activity and action restrictions are separate gameplay checks, not substitutes for ownership. |
| Add/open chest | Creation resolves loot IDs from the loaded playthrough's consumable/equipment collections. Opening resolves both participant and chest within the owned scene. |
| Consume item | Inventory link ID must exist in the selected participant's journey/scene-character inventory and be unused; no global inventory-ID lookup is used. |
| Trade | Both participants must belong to the loaded scene and satisfy player/activity rules. The inventory link must belong to one of those two characters. Reverse-direction transfers are intentionally supported; a third player's item cannot be selected by ID through this lookup. |
| Resolve event inventory overflow | Pending action and resolution token are scene-bound. Recipient must be another active journey character within the same playthrough. Existing item must belong to the event recipient's inventory. |
| Change current objective | `SceneObjectiveRepository.GetRuntimeAsync` requires scene ID, playthrough ID and owner. `SceneObjectiveService.SelectAsync` checks scene activity and index against that scene's objective sets. |

Primary evidence: `ScenePlaythroughController`, `PlaythroughController`, `ScenePlaythroughService` and its `StartInventory`, `Counterattacks`, `Campfire`, and `Removal` partials; `PlaythroughRepository`; `SceneObjectiveService` / `SceneObjectiveRepository`.

### Snapshot creation and ISO-13 follow-up

`PlaythroughRepository.GetStartAssetsAsync` explicitly filters characters, spells, spell types, consumables and equipment by caller UserId. Character loading uses `IgnoreQueryFilters` for archived references but retains the owner predicate. `PlaythroughService.ValidateStartSource` validates referenced character/spell/item/type IDs against those owned asset sets before saving the new graph, including dialogue-section characters. Snapshot dialogue characters are then resolved through the validated character dictionary, not copied from an unchecked foreign navigation property.

Consequently, the legacy foreign dialogue-character reference described in ISO-13 should reject **new** playthrough creation with `Playthrough.InvalidSourceGraph`, rather than copy that foreign character. This does not repair old templates or prove old snapshots are clean. Existing snapshot provenance remains part of the approved legacy-data investigation under ISO-13; do not silently delete or rewrite saved games.

### Guest and media findings: existing tasks retained

| Area | Evidence and remaining work |
| --- | --- |
| Invitation creation/revocation | `PlaythroughJoinSessionRepository.GetForOwnerAsync/GetOwnedPlaythroughAsync` bind the playthrough to caller UserId. Creation uses 32 random bytes; only SHA-256 token hash is persisted. Rotation replaces the hash. |
| Anonymous HTTP snapshots | Repository requires matching hash, no revocation, future expiry and an incomplete playthrough on each request. `PublicPlaythroughMappings` returns journey characters, active scene characters, current objective and logs rather than the host details DTO. Snapshot query size remains GUEST-05. |
| Existing hub connections | Audit originally found one-time group authorization. October 17 implementation replaces group broadcasts with invitation-validated per-connection delivery and server aborts, plus an idle sweep. **GUEST-01/02 implementation is pending runtime verification**, detailed above. Current notifier still sends only update reasons. Invocation throttling remains GUEST-04; multi-instance delivery remains GUEST-07. |
| Generic image upload | Authentication is required, but `ImagesController` / `ImagesService` do not pass/store owner identity for assets. Extension/MIME/signature/size validation is present in `AzureStorageBlob`; that is not asset ownership or quota enforcement. **MEDIA-03/04 remain open.** |
| Generic image deletion | DELETE accepts a caller-supplied URL without owner/reference validation. `DeletePhotoFromUrl` currently returns success without deleting anything, so this is not a demonstrated foreign-file deletion today. Do not enable actual deletion behind this API without **MEDIA-05**. |
| Direct media access and cleanup | Upload code requests public blob access when creating the container and returns direct URLs. Actual deployed storage policy was not inspected. Host/guest API authorization cannot protect independently public URLs. **MEDIA-01/02** cover the policy and implementation; **MEDIA-06** covers cleanup. `DeleteMediaAsync` is a real filename-based delete helper, not an authorization boundary: callers must retain parent authorization and shared-reference safeguards reviewed under ISO-06. |

No duplicate ISO task was added for findings already covered by GUEST/MEDIA tasks. At the user's explicit request, ISO-10 is closed as a completed source audit; the GUEST/MEDIA remediation tasks and ISO-11 runtime verification remain open. This does not certify runtime isolation or production readiness.

### Required runtime verification

Under ISO-11, use disposable data owned by two accounts and two playthroughs owned by the same account. Exercise every runtime operation above with an owned root, foreign root, nonexistent root, and mismatched scene/playthrough IDs. Substitute participant, attack target, spell, chest, loot item, inventory link and transfer-recipient IDs from other scenes/playthroughs. Check rejected operations leave inventory, stats, turns, logs and storage unchanged. Include stale counterattack/resolution tokens and valid owner success paths.

For guest checks, verify missing/invalid/expired/revoked/rotated/completed invitations fail HTTP snapshot access and hub joins. Keep a deliberately non-cooperating socket connected to verify server-side cessation of updates after GUEST-01/02. Verify anonymous guests cannot call host mutation routes. Check direct media access against the chosen MEDIA-01 policy, not against an assumption that invitation expiry revokes public image URLs.

## Existing strengths

The application already has useful foundations: password hashing, default authenticated API access, owner-scoped queries in many areas, and cryptographically generated, hashed QR tokens.

The main issue is inconsistent enforcement around those foundations, not a need to rewrite the game.

**Highest priority: prevent one customer from reading or changing another customer's content before investing in additional game features or launching payments.**
