import { Component } from '@angular/core';
import { AuthService } from '../auth.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-lobby',
    templateUrl: './lobby.component.html',
    styleUrls: ['./lobby.component.css'],
    standalone: true,
    imports: [FormsModule, CommonModule]
})
export class LobbyComponent {
    status = '';
    constructor(private auth: AuthService) { }

    async joinQueue() {
        const token = this.auth.getToken();
        if (!token) { this.status = 'Not logged in'; return; }
        try {
            const resp = await fetch((window as any).API_URL + '/api/matchmaking/join', {
                method: 'POST', headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` }
            });
            const body = await resp.json();
            if (body.status === 'queued') this.status = 'Queued';
            else if (body.status === 'matched') this.status = `Matched: ${body.matchId}`;
        } catch (err) { this.status = 'Error joining queue'; }
    }
}
