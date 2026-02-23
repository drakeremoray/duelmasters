import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css']
})
export class LoginComponent {
    username = '';
    password = '';
    message = '';
    returnUrl = '/';

    constructor(private auth: AuthService, private router: Router, private route: ActivatedRoute) {
        const q = this.route.snapshot.queryParamMap.get('returnUrl');
        if (q) this.returnUrl = q;
    }

    async onLogin(e: Event) {
        e.preventDefault();
        try {
            const resp = await fetch((window as any).API_URL + '/api/auth/login', {
                method: 'POST', headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ username: this.username, password: this.password })
            });
            if (!resp.ok) { this.message = 'Login failed'; return; }
            const body = await resp.json();
            this.auth.setToken(body.token);
            this.router.navigateByUrl(this.returnUrl);
        } catch (err) { this.message = 'Login error'; }
    }
}
