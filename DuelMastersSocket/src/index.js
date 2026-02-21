const express = require('express');
const http = require('http');
const { Server } = require('socket.io');
const jwt = require('jsonwebtoken');
// we'll validate tokens by calling the API `/api/auth/me` endpoint so the server
// and API remain the single source of truth for authentication.
const API_URL = process.env.API_URL || 'http://localhost:5000';

const app = express();
const server = http.createServer(app);
const io = new Server(server, {
    cors: {
        origin: '*'
    }
});

const PORT = process.env.PORT || 3001;
const JWT_SECRET = process.env.JWT_SECRET || 'dev-secret';

app.get('/', (req, res) => {
    res.json({ status: 'DuelMasters Socket Server' });
});

// Auth middleware: validate token by calling the API's /api/auth/me endpoint.
io.use(async (socket, next) => {
    const token = socket.handshake.auth && socket.handshake.auth.token;
    if (!token) return next(new Error('Authentication error'));
    try {
        const res = await fetch(`${API_URL}/api/auth/me`, {
            headers: { Authorization: `Bearer ${token}` }
        });
        if (!res.ok) return next(new Error('Authentication error'));
        const body = await res.json();
        // expect { player: { id, username, displayName } }
        socket.user = body.player || body;
        // store token for forwarding actions
        socket.authToken = token;
        // normalize id
        socket.userId = socket.user?.id ?? socket.user?.Id ?? socket.user?.sub;
        next();
    } catch (err) {
        next(new Error('Authentication error'));
    }
});

// Rooms: one room per match (match:<id>)
io.on('connection', (socket) => {
    const user = socket.user;
    console.log(`socket connected: ${socket.id} user:${user?.sub || 'unknown'}`);

    socket.on('joinMatch', ({ matchId }) => {
        const room = `match:${matchId}`;
        // call API to register participant and then join room
        (async () => {
            try {
                const res = await fetch(`${API_URL}/api/matches/${matchId}/join`, {
                    method: 'POST',
                    headers: { Authorization: `Bearer ${socket.authToken}` }
                });
                if (res.ok) {
                    const body = await res.json();
                    socket.join(room);
                    io.to(room).emit('playerJoined', { playerId: socket.userId, socketId: socket.id });
                    socket.emit('joinAck', { status: 'joined', participant: body });
                } else {
                    const txt = await res.text();
                    socket.emit('joinAck', { status: 'error', detail: txt });
                }
            } catch (err) {
                socket.emit('joinAck', { status: 'error', detail: String(err) });
            }
        })();
    });

    socket.on('leaveMatch', ({ matchId }) => {
        (async () => {
            const room = `match:${matchId}`;
            try {
                const res = await fetch(`${API_URL}/api/matches/${matchId}/leave`, {
                    method: 'POST',
                    headers: { Authorization: `Bearer ${socket.authToken}` }
                });
                if (res.ok) {
                    socket.leave(room);
                    io.to(room).emit('playerLeft', { playerId: socket.userId, socketId: socket.id });
                    socket.emit('leaveAck', { status: 'left' });
                } else {
                    const txt = await res.text();
                    socket.emit('leaveAck', { status: 'error', detail: txt });
                }
            } catch (err) {
                socket.emit('leaveAck', { status: 'error', detail: String(err) });
            }
        })();
    });

    socket.on('matchEvent', async ({ matchId, event }) => {
        const room = `match:${matchId}`;
        // forward action to API so the server-authoritative engine runs
        try {
            const resp = await fetch(`${API_URL}/api/matches/${matchId}/actions`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${socket.authToken}`
                },
                body: JSON.stringify({ actionType: event.type ?? 'noop', payload: event.payload ?? event })
            });
            if (resp.ok) {
                const result = await resp.json();
                // broadcast resulting events to the room
                io.to(room).emit('matchEvent', { playerId: socket.userId, result });
            } else {
                const errText = await resp.text();
                socket.emit('error', { message: 'Action rejected by server', detail: errText });
            }
        } catch (err) {
            socket.emit('error', { message: 'Action forwarding failed', detail: String(err) });
        }
    });

    socket.on('disconnect', (reason) => {
        console.log(`socket disconnected: ${socket.id} reason:${reason}`);
    });
});

server.listen(PORT, () => {
    console.log(`DuelMasters Socket Server listening on ${PORT}`);
});
