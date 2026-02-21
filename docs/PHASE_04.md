<!-- File: PHASE_04.md -->

# Phase 4: Socket Server Foundations

- Set up Express socket server for real-time communication.
- Implement connection lifecycle, authentication, and presence tracking.
- Integrate socket server with `DuelMastersApi` for token validation.

Completed work:

- Scaffolded `DuelMastersSocket` server (Express + socket.io).
- Added JWT auth middleware and presence handling (rooms: `match:<id>`).
- Implemented server ↔ API integration: socket server validates client tokens by calling `GET /api/auth/me` on the API and uses the returned player as the socket identity. This avoids duplicating JWT secrets in two places.

Run notes:

- Start the API first (default: `http://localhost:5000`).
- Start socket server: set `API_URL` if your API runs elsewhere and `JWT_SECRET` for other usages.

Next: Phase 5 kickoff below.
