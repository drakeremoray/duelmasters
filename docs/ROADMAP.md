<!-- File: ROADMAP.md -->

# DuelMasters Web App — Development Roadmap

## Phase 1: Project Setup
- Initialize repositories for backend (C# API), frontend (Ionic Angular), and socket server (Node/Express).
- Set up PostgreSQL database and basic schema.

## Phase 2: Core Domain Modeling
- Define database models for players, cards, decks, matches, and game state.
- Implement initial migrations and seed data.

## Phase 3: C# API Foundations
- Build REST endpoints for authentication, user management, and deck management.
- Implement JWT-based authentication.

## Phase 4: Socket Server Foundations
- Set up Express socket server for real-time communication.
- Implement connection lifecycle, authentication, and presence tracking.

## Phase 5: Game Logic & Rules Engine
- Implement core turn-based game logic and rules in C# API.
- Expose endpoints for command validation and state queries.

## Phase 6: Real-Time Matchmaking & Room Management
- Add matchmaking queues and lobby management to socket server.
- Implement room creation, join/leave, and spectator support.

## Phase 7: Frontend MVP
- Scaffold Ionic Angular app with authentication, lobby, and match views.
- Integrate with REST API for user/deck management.

## Phase 8: Real-Time Gameplay Integration
- Connect frontend to socket server for real-time match updates.
- Implement command/ack pattern and state synchronization.

## Phase 9: Observability & Reliability
- Add logging, error handling, and correlation IDs across all services.
- Implement reconnect/resync logic and presence updates.

## Phase 10: Testing, Polish & Deployment
- Write unit/integration tests for all layers.
- Polish UI/UX, optimize performance, and deploy to production environments.