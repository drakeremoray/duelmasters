import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

import { AppComponent } from './app/app.component';
import { DecksComponent } from './pages/decks/decks.component';
import { CardsComponent } from './pages/cards/cards.component';
import { LoginComponent } from './login/login.component';
import { LobbyComponent } from './lobby/lobby.component';
import { MatchComponent } from './match/match.component';
import { AuthGuard } from './auth.guard';
import { AuthService } from './auth.service';

const routes: Routes = [
    { path: '', component: LobbyComponent, canActivate: [AuthGuard] },
    { path: 'login', component: LoginComponent },
    { path: 'match/:id', component: MatchComponent, canActivate: [AuthGuard] }
];

@NgModule({
    declarations: [AppComponent, LoginComponent, LobbyComponent, MatchComponent, DecksComponent, CardsComponent],
    imports: [BrowserModule, FormsModule, RouterModule.forRoot(routes)],
    providers: [AuthService, AuthGuard],
    bootstrap: [AppComponent]
})
export class AppModule { }
