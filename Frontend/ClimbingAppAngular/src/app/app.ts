import { Component, signal } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { ClimbCard } from './climb-card/climb-card';
import { ClimbList } from './climb-list/climb-list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatMenuModule } from '@angular/material/menu';
import { AuthService } from './services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, ClimbCard, ClimbList, MatToolbarModule, MatButtonModule, MatIconModule, MatMenuModule, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  constructor(public auth: AuthService, private router: Router) { }
  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
  protected readonly title = signal('ClimbingAppAngular');
}
