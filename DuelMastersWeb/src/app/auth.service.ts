import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class AuthService {
    getToken(): string | null { return localStorage.getItem('dm_token'); }
    isLoggedIn(): boolean { return !!this.getToken(); }
    setToken(token: string) { localStorage.setItem('dm_token', token); }
    clear(): void { localStorage.removeItem('dm_token'); }
}
