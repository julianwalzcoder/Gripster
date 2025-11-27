import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { Climb } from '../model/climb';
import { ClimbService } from '../services/climb-service';

@Component({
  selector: 'app-climb-card',
  imports: [CommonModule, RouterModule],
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

  debugClick(): void {
    console.log('Link clicked! Navigating to climb:', this.climb.climbId);
    alert('Navigating to climb ' + this.climb.climbId);
  }
}
