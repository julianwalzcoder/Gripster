import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Climb } from '../model/climb';
import { ClimbCard } from '../climb-card/climb-card';
import { ClimbService } from '../services/climb-service';

@Component({
  selector: 'app-climb-list',
  imports: [CommonModule, ClimbCard],
  templateUrl: './climb-list.html',
  styleUrl: './climb-list.css',
})
export class ClimbList implements OnInit {
  climbs: Climb[] = [];
  loading = true;
  error: string | null = null;
  
  constructor(private climbService: ClimbService) {}
  
  ngOnInit(): void {
    this.loadClimbs();
  }

  loadClimbs(): void {
    this.loading = true;
    console.log('Fetching climbs from:', this.climbService.baseUrl);
    this.climbService.getClimbs().subscribe({
      next: (climbs) => {
        console.log('Received climbs:', climbs);
        this.climbs = climbs;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error fetching climbs:', error);
        this.error = error.message || 'Failed to load climbs';
        this.loading = false;
      }
    });
  }

  onDeleteClimb(id: number): void {
    if (confirm('Are you sure you want to delete this climb?')) {
      this.climbService.deleteClimb(id).subscribe({
        next: () => {
          this.climbs = this.climbs.filter(climb => climb.id !== id);
        },
        error: (error) => {
          console.error('Error deleting climb:', error);
          alert('Failed to delete climb');
        }
      });
    }
  }
}


