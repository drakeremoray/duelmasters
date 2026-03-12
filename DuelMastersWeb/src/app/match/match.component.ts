import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { io } from 'socket.io-client';
import { AuthService } from '../auth.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-match',
    templateUrl: './match.component.html',
    styleUrls: ['./match.component.css'],
    standalone: true,
    imports: [FormsModule, CommonModule]
})
export class MatchComponent {
    matchId = '';
    joined = false;
    log = '';
    socket: any = null;

    constructor(private route: ActivatedRoute, private auth: AuthService) {
        this.matchId = this.route.snapshot.paramMap.get('id') || '';
    }

    join() {
        const token = this.auth.getToken();
        if (!token) { this.log = 'Not logged in'; return; }
        this.socket = io((window as any).SOCKET_URL || 'http://localhost:3001', { auth: { token } });
        this.socket.on('connect', () => {
            this.log += 'connected\n';
            this.socket.emit('joinMatch', { matchId: this.matchId });
        });
        this.socket.on('joinAck', (d: any) => { this.log += 'joinAck: ' + JSON.stringify(d) + '\n'; this.joined = true; });
        this.socket.on('matchEvent', (e: any) => { this.log += 'matchEvent: ' + JSON.stringify(e) + '\n'; });
        this.socket.on('error', (e: any) => { this.log += 'socket error: ' + JSON.stringify(e) + '\n'; });
    }

    sendDraw() {
        if (!this.socket) return;
        this.socket.emit('matchEvent', { matchId: this.matchId, event: { type: 'draw', payload: {} } });
    }
}
