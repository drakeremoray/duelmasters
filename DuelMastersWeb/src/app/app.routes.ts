import { Routes } from '@angular/router';
import { AppComponent } from './app/app.component';
import { LoginComponent } from './login/login.component';
import { LobbyComponent } from './lobby/lobby.component';
import { MatchComponent } from './match/match.component';
import { DecksComponent } from './pages/decks/decks.component';
import { CardsComponent } from './pages/cards/cards.component';
import { AuthGuard } from './auth.guard';

export const routes: Routes = [
    { path: '', component: LobbyComponent, canActivate: [AuthGuard] },
    { path: 'login', component: LoginComponent },
    { path: 'match/:id', component: MatchComponent, canActivate: [AuthGuard] },
    { path: 'decks', component: DecksComponent, canActivate: [AuthGuard] },
    { path: 'cards', component: CardsComponent, canActivate: [AuthGuard] }
];
