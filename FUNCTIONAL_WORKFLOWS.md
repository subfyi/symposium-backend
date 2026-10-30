# Functional migration status

## Implemented

- Members can create and edit their own draft submissions. Owner/approval/payment,
  publication metadata and year cannot be forged through member payloads.
- Approved and deleted submissions are locked for member edits. Owners can still
  discuss approved submissions using the separate comment endpoints.
- Anonymous reads return approved papers only, without private author emails,
  submitter details or comments. Member lists show their own submissions; public
  presentation lists remain public-only.
- Author collections are synchronized within a transaction and retain their
  explicit order. Cross-submission author IDs are rejected.
- Pipe-separated keywords create deduplicated `tagcon` parameters. PostgreSQL
  transaction locking serializes concurrent keyword creation without changing
  uniqueness rules for unrelated legacy parameters.
- Profile name aliases and institution/faculty/department/address/contact and
  typed parameter fields round-trip. Self-edit cannot change roles or passwords.
  Password changes use `/user/me/password` and require the current password.
- Private comments support list/create/update/delete. Members may only change
  their own comments; administrators can moderate. Omitted comments are preserved
  during submission updates, preventing stale forms from deleting new replies.
- Approval transitions and new comments enqueue email plus user-visible
  notifications in the same database transaction. In-app read state is user-scoped.
- Submission language catalogs are separate from UI translations. `typelanguage`
  and presentation/publish/topic `lang` references now have compatible contracts.
- New uploaded files are owner-bound and mapped explicitly from `files` to legacy
  submission attachment records. Foreign uploads cannot be attached by a member.

## Schema and email operations

Apply `CompleteSubmissionWorkflows` after `InitialStorage` using the existing
Storage design-time EF commands. Never modify an applied initial migration.
The migration adds profile fields, author ordering, file ownership/mapping,
submission comments, language catalogs and notification/outbox tables.

The API hosts the notification dispatcher. `notifications:dispatchEnabled` defaults
to true. Pending mail retries indefinitely with exponential backoff capped at six
hours. A database lease coordinates concurrent API instances. Delivery is
**at least once**: a process failure after SMTP acceptance but before committing
`sent_at` may deliver a duplicate. Configure working production SMTP and keep
`smtp:enableSsl=true`; disabling TLS is intended for local test fixtures only.
Set `notifications:dispatchEnabled=false` when rehearsing imports to prevent mail.

Existing `gorev_icerik` comments must be imported into `submission_comments` with
article/author ID reconciliation; they are not automatically copied by a schema
migration. Existing languages and attachment IDs require the same explicit import
mapping. Do not expose the old uploads tree as an unrestricted public directory.

## Verification

Latest local verification: 23 xUnit tests, 55 HTTP requests on a fresh PostgreSQL
database with both migrations, SMTP failure/retry confirmed via outbox state,
fixture WebDAV/CDN byte round-trip, SQL projection compatibility and admin build
passed. Linux-x64 self-contained publish also passed. Playwright performed actual profile/submission/comment saves at 1440px
and 390px, including light/dark views and admin catalogs.

```bash
dotnet test Symposium.slnx --no-restore
dotnet run --project tests/ArticleCompatibility
SYMPOSIUM_TEST_DATABASE='Host=127.0.0.1;Port=55438;Database=symposium_functional_test;Username=YOUR_LOCAL_ROLE' node tests/http-smoke.mjs
```

The smoke test only accepts a local database ending `_test`. It migrates and writes
test records, starts disposable SMTP and WebDAV/CDN fixtures on ports 15025/15999,
injects an SMTP failure, and verifies retry, real upload bytes, profile/password,
member isolation, comments, notifications and catalog contracts over HTTP.

Optional browser verification uses a running admin on `SYMPOSIUM_UI_URL`, configured
with `VITE_ENDPOINT=http://127.0.0.1:15088/api`, client ID `integration`, and public
client value `integration-client`. Set `PLAYWRIGHT_MODULE` to an installed Playwright
ES module (or install Playwright normally). The same smoke test then verifies real
member login, profile save, submission save, comments, catalogs and notifications
at desktop/mobile sizes. Test ports must be free. No production service is used.

## Remaining release work

This completes the identified functional gaps, not the production cutover.
Production MySQL-to-PostgreSQL import/reconciliation, imported document delivery,
production SMTP/WebDAV credentials, deployment account/service configuration,
backup/rollback rehearsal and staging sign-off remain separate release gates.
