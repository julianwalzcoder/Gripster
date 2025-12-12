import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { GymService } from './services/gym-service';
import { AuthService } from './services/auth-service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterModule, MatToolbarModule, MatMenuModule, MatIconModule, MatButtonModule],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class AppComponent {
  currentGymName = 'All Gyms';
  currentGymId: number | null = null;

  constructor(private gymService: GymService, public auth: AuthService) { // <-- expose as public
    this.gymService.getSelectedGymName$().subscribe(name => this.currentGymName = name);
    this.gymService.getSelectedGymId$().subscribe(id => this.currentGymId = id);
  }

  gymLink(): any[] {
    return ['/climbs', this.currentGymId ?? 1];
  }

  logout(): void {
    this.auth.logout();
  }
}
