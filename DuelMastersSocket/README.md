# DuelMasters Socket Server

Minimal Node/Express + socket.io server for DuelMasters real-time transport.

Getting started:

- Install dependencies: `npm install`
- Start server: `JWT_SECRET=your_jwt_secret npm start`

Connections:
- Clients must connect with a JWT in the `auth` payload: `io(url, { auth: { token } })`
- Events:
  - `joinMatch` { matchId }
  - `leaveMatch` { matchId }
  - `matchEvent` { matchId, event }
# DuelMastersSocket (scaffold)

This folder will contain the Node/Express socket server for real-time transport.

Next steps:
- Initialize a Node project, add socket handling, and implement message shape validation.
