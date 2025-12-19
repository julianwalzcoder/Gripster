import { Component, signal } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { ClimbCard } from './climb-card/climb-card';
import { ClimbList } from './climb-list/climb-list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatMenuModule } from '@angular/material/menu';
import { AuthService } from './services/auth-service';
import { ClimbService } from './services/climb-service';
import { CommonModule, Location } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet, RouterLink, ClimbCard, ClimbList,
    MatToolbarModule, MatButtonModule, MatIconModule, MatMenuModule, CommonModule
  ],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App {
  popupText: string | null = null;

  constructor(
    public climb: ClimbService,
    public auth: AuthService,
    private router: Router,
    private location: Location
  ) { }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  showText(type: string) {
    if (type === 'terms') {
      this.popupText = `
        <h1>Terms of Service</h1>
        <p>Welcome to Gripster! These terms explain the rules for using our services at Copenhagen Business School (CBS).</p>
      `;
    }
    if (type === 'privacy') {
      this.popupText = `
        <h1>Privacy Policy</h1>
        <p>We value your privacy. This page explains how Gripster collects, stores, and uses your personal data at CBS.</p>
      `;
    }
    if (type === 'eu') {
      this.popupText = `
        <h1>EU Legal Information</h1>
        <p>This section outlines the EU regulations we comply with, including GDPR, when operating at CBS.</p>
      `;
    }
  }

  closePopup() {
    this.popupText = null;
  }
}
