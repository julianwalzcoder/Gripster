import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { Climb } from '../model/climb';
import { ClimbService } from '../services/climb-service';

@Component({
  selector: 'app-climb-card',
  standalone: true,
  imports: [CommonModule, RouterModule, MatCardModule, MatButtonModule, MatChipsModule, MatIconModule],
  templateUrl: './climb-card.html',
  styleUrl: './climb-card.css',
})
export class ClimbCard {
  constructor(
    private router: Router,
    private climbService: ClimbService,
  ) {}

  @Input() climb!: Climb;
  @Output() delete = new EventEmitter<number>();

  // Default user ID (should be replaced with auth service)
  private readonly defaultUserId = 1;

  avgRating?: number; // Durchschnitt für diese Route
  userRating?: number; // aktuelle Bewertung des Users

  ngOnInit(): void {
    if (this.climb?.routeId) {
      this.climbService.getAverageRating(this.climb.routeId).subscribe({
        next: (avg: number | null) => this.avgRating = avg ?? undefined,
        error: () => this.avgRating = undefined
      });
      // User-Rating laden
      if (this.climb.userId) {
        this.climbService.getUserRating(this.climb.userId, this.climb.routeId).subscribe({
          next: (rating: number | null) => this.userRating = rating ?? undefined,
          error: () => this.userRating = undefined
        });
      }
    }
  }

  deleteClimb() {
    if (this.climb) {
      this.delete.emit(this.climb.routeId);
    }
  }

  editClimb(id: number) {
    this.router.navigate(['/edit-climb', id]);
  }

  viewClimbDetails(): void {
    console.log('Navigating to climb detail:', this.climb.routeId);
    this.router.navigate(['/climb-detail', this.climb.routeId]).then(
      success => console.log('Navigation successful:', success),
      error => console.error('Navigation error:', error)
    );
  }

  updateClimbStatus(routeId: number, status: string): void {
    const userId = this.climb.userId ?? this.defaultUserId;
    console.log('Update status clicked:', userId, routeId, status);
    this.climbService.updateClimbStatus(userId, routeId, status).subscribe({
      next: () => {
        console.log('Status updated successfully');
        this.climb.status = status;
      },
      error: (err: any) => {
        console.error('Error updating status:', err);
        alert('Failed to update climb status. ' + err.message);
      }
    });
  }

  rateClimb(rating: number): void {
    const userId = this.climb.userId;
    if (!userId) { alert('Bitte einloggen'); return; }
    this.userRating = rating; // sofortiges UI-Feedback
    this.climbService.setRating(userId, this.climb.routeId, rating).subscribe({
      next: () => {
        // Durchschnitt optional neu laden
        this.climbService.getAverageRating(this.climb.routeId).subscribe(avg => this.avgRating = avg ?? undefined);
      },
      error: (err) => {
        console.error('Rating failed', err);
        alert('Raiting failed');
      }
    });
  }

  debugClick(): void {
    console.log('Link clicked! Navigating to climb:', this.climb.climbId);
    alert('Navigating to climb ' + this.climb.climbId);
  }
}
