<!-- File: PHASE_10.md -->

# Phase 10: Incremental MVP — Hardening, CI & Deployment

Goal

- Final hardening and operational readiness: CI maturity, migrations, runbooks, and a production deployment path.

Incremental deliverables

- CI: expand workflows to include integration tests, linting, and release build pipelines.
- Migrations: commit EF migrations, add migration-run steps to CI and a rollback strategy.
- Ops: documentation and runbooks for deploying (DB, sockets, scaling), plus basic monitoring alerts.
- Polish: small UX/UX polish items and performance fixes identified from earlier load testing.

Acceptance criteria

- CI runs integration tests and publishes an artifact; documented deployment steps exist and a test deploy succeeds.
