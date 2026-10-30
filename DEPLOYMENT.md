# Versioned Gitea deployment

Repository: `akinayturan/symposium-backend` on `y.robinsoft.org`.

This backend uses the same versioned release contract as Autosurf:

1. Push the reviewed branch to Gitea.
2. Publish one `vX.Y.Z` tag on that commit.
3. Gitea Actions tests and publishes the self-contained linux-x64 API with that version.
4. The artifact is streamed to `deploy-app app VERSION backend appsettings.json`.
5. The Gitea release is created only after deployment succeeds.

`build/version.json` records the version, tag and exact commit in the deployed
artifact. Runtime configuration is excluded from the archive and preserved by
the server-side deployment tool. No database migration is executed by the workflow.
Version-directory switching/rollback are provided by the remote `deploy-app`,
not by Git, and its server contract must be verified before the first release.

## Release command

From a clean branch, preview the next patch version without changing Git:

```bash
bash scripts/release.sh
```

Publish the first explicit release, or a subsequent version bump:

```bash
bash scripts/release.sh --version 0.1.0 --push
bash scripts/release.sh --minor --push
bash scripts/release.sh --major --push
```

The helper follows Autosurf's `dorel` tag-bump convention but defaults to dry run,
never merges/pulls working files, verifies that the branch is already pushed to
`origin`, and pushes only the newly created annotated tag. A plain branch push
does not deploy. Do not run `--push` until the production cutover is approved.

## Server prerequisites

The `robinsoft` runner needs Bash, Git, .NET 10 SDK, OpenSSH and the configured
noninteractive SSH identity. Configure repository secrets:

- `DEPLOY_SSH_TARGET`: verified deployment `user@hostname`.
- `DEPLOY_SSH_KNOWN_HOSTS`: independently verified SSH host keys.
- `DEPLOY_APPSETTINGS_PATH`: absolute path to the nonempty remote `appsettings.json`
  that `deploy-app` preserves for the `app` deployment.

The target account must provide `deploy-app`; the `backend` service must start
`Symposium.Api.Server` from the selected release. Provision PostgreSQL, private
configuration, SMTP, WebDAV/CDN and public documents separately. Apply reviewed
migrations explicitly after backup/rehearsal, not as a side effect of deployment.
`GENERATE_EF_SCRIPT=true` optionally bundles review-only idempotent migration SQL.

Before release: reconcile Laravel data/IDs and attachments, verify staging HTTP
workflows, approve migration/rollback and confirm `/api/healthz` through the proxy.
See `FUNCTIONAL_WORKFLOWS.md` for functional coverage and migration requirements.
