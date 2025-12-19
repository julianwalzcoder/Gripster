import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { ClimbService } from '../services/climb-service';
import { AuthService } from '../services/auth-service';
import { Climb } from '../model/climb';

@Component({
  selector: 'app-climb-detail',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule, MatChipsModule],
  templateUrl: './climb-detail.component.html',
  styleUrls: ['./climb-detail.component.css']
})
export class ClimbDetailComponent implements OnInit {
  climbID!: number;
  climb!: Climb;
  loading = true;
  error: string | null = null;
    avgRating?: number; // Durchschnitt für diese Route
  userRating?: number; // aktuelle Bewertung des Users
  
  constructor(
    private climbService: ClimbService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private router: Router
  ) {}
  
  ngOnInit(): void {
    // Get the ID from the route parameter
    this.climbID = Number(this.route.snapshot.paramMap.get('id'));
    console.log('Climb ID from route:', this.climbID);
    this.loadClimb();
    
  if (this.climbID) {
      this.climbService.getAverageRating(this.climbID).subscribe({
        next: (avg: number | null) => this.avgRating = avg ?? undefined,
        error: () => this.avgRating = undefined
      });
      
      const userId = this.authService.getCurrentUserId();
      if (userId) {
        this.climbService.getUserRating(userId, this.climbID).subscribe({
          next: (rating: number | null) => this.userRating = rating ?? undefined,
          error: (err) => {
            // 404 is expected if user hasn't rated yet
            if (err.status === 404) {
              this.userRating = undefined;
            } else {
              console.error('Error fetching user rating:', err);
              this.userRating = undefined;
            }
          }
        });
      }
    }
  }

  loadClimb(): void {
    this.loading = true;
    console.log('Fetching climb from:', this.climbService.baseUrl);
    console.log('Fetching climb with ID:', this.climbID);
    this.climbService.getClimb(this.climbID).subscribe({
      next: (climb) => {
        console.log('Climb data received:', climb);
        this.climb = climb;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error fetching climb:', error);
        this.error = error.message || 'Failed to load climb';
        this.loading = false;
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/climbs']);
  }

  onDeleteClimb(id: number): void {
    if (confirm('Are you sure you want to delete this climb?')) {
      this.climbService.deleteClimb(id).subscribe({
        next: () => {
          this.router.navigate(['/climbs']);
        },
        error: (error) => {
          console.error('Error deleting climb:', error);
          alert('Failed to delete climb');
        }
      });
    }
  }

  updateClimbStatus(userId: number, routeId: number, status: string): void {
    const effectiveUserId = this.authService.getCurrentUserId() ?? 1;
    console.log('Update status clicked:', effectiveUserId, routeId, status);
    this.climbService.updateClimbStatus(effectiveUserId, routeId, status).subscribe({
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

  updateClimbStatusWithDefault(routeId: number, status: string): void {
    const userId = this.authService.getCurrentUserId() ?? 1;
    this.updateClimbStatus(userId, routeId, status);
  }

 rateClimb(rating: number): void {
    const userId = this.authService.getCurrentUserId();
    if (!userId) { 
      alert('Please log in to rate climbs'); 
      return; 
    }
    this.userRating = rating; // sofortiges UI-Feedback
    this.climbService.setRating(userId, this.climb.routeId, rating).subscribe({
      next: () => {
        // Durchschnitt optional neu laden
        this.climbService.getAverageRating(this.climb.routeId).subscribe(avg => this.avgRating = avg ?? undefined);
      },
      error: (err) => {
        console.error('Rating failed', err);
        alert('Rating failed');
      }
    });
  }

}