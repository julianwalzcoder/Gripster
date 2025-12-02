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
  templateUrl: './login.html',
imports: [FormsModule, CommonModule, MatIconModule, MatCardModule, MatFormFieldModule, MatInputModule]
})
export class Login {
  username = '';
  password = '';
  error: string | null = null;

  constructor(private auth: AuthService, private router: Router) { }

  onSubmit() {
    this.auth.login(this.username, this.password).subscribe({
      next: () => this.router.navigate(['/climbs']),
      error: err => this.error = 'Login failed'
    });
  }
}

