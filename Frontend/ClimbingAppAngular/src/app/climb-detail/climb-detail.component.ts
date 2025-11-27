import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ClimbService } from '../services/climb-service';
import { Climb } from '../model/climb';

@Component({
  selector: 'app-climb-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './climb-detail.component.html',
  styleUrls: ['./climb-detail.component.css']
})
export class ClimbDetailComponent implements OnInit {
  climbID!: number;
  climb!: Climb;
  loading = true;
  error: string | null = null;
  
  // Default user ID (should be replaced with auth service)
  private readonly defaultUserId = 1;
  
  constructor(
    private climbService: ClimbService,
    private route: ActivatedRoute,
    private router: Router
  ) {}
  
  ngOnInit(): void {
    // Get the ID from the route parameter
    this.climbID = Number(this.route.snapshot.paramMap.get('id'));
    console.log('Climb ID from route:', this.climbID);
    this.loadClimb();
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
    const effectiveUserId = this.climb.userId ?? this.defaultUserId;
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
}
