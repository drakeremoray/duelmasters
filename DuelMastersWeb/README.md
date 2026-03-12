# DuelMastersWeb (Ionic/Angular Frontend)

This is the Ionic Angular frontend for DuelMasters online TCG.

## Setup

```bash
npm install
```

## Running the Application

### With ng serve (Angular CLI):
```bash
npm start
# or
npx ng serve --host 0.0.0.0 --port 4201
```
Open browser to: `http://localhost:4201`

### With Ionic Serve:
```bash
npm run ionic:serve
# or
ionic serve --port 8101
```
Open browser to: `http://localhost:8101`

## Pages & Routes

- `/login` - Login/Registration page (public)
- `/` - Lobby (authenticated) - Join match queue
- `/match/:id` - Match view (authenticated) - Live gameplay
- `/decks` - Deck management (authenticated)
- `/cards` - Card library (authenticated)

## Architecture

- **Framework**: Angular 18 with Ionic 8
- **State**: Component-based with services
- **Authentication**: JWT-based auth via API
- **Realtime**: Socket.io for match updates
- **Styling**: Ionic components with custom CSS

## Key Features

- Mobile-responsive UI with Ionic components
- Real-time multiplayer matches via WebSocket
- Deck management and card browsing
- Login/authentication flow
- Match viewer with live updates

## Dependencies

- `@angular/core`: ^18.0.0
- `@ionic/angular`: ^8.8.1
- `socket.io-client`: ^4.7.2
- `rxjs`: ^7.8.0
