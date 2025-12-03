import { Component } from '@angular/core';
import { AuthService } from '../services/auth-service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  templateUrl: './login.html',
  imports: [FormsModule, CommonModule, MatIconModule, MatCardModule, MatFormFieldModule, MatInputModule]
})

export class Login {
  isRegisterMode = false;

  username = '';
  password = '';
  name = '';
  mail = '';
  street = '';
  streetNumber?: number;
  postcode?: number;
  city = '';

  error: string | null = null;

  constructor(private auth: AuthService, private router: Router) { }

  onSubmit() {
    this.error = null;

    if (this.isRegisterMode) {
      this.auth.register({
        username: this.username,
        password: this.password,
        mail: this.mail,
        name: this.name,
        street: this.street,
        streetNumber: this.streetNumber,
        postcode: this.postcode,
        city: this.city
      }).subscribe({
        next: () => this.router.navigate(['/select-gym']),
        error: () => this.error = 'Registration failed'
      });
    } else {
      this.auth.login(this.username, this.password).subscribe({
        next: () => this.router.navigate(['/select-gym']),
        error: () => this.error = 'Login failed'
      });
    }
  }

  switchMode() {
    this.error = null;
    this.isRegisterMode = !this.isRegisterMode;
  }
}

