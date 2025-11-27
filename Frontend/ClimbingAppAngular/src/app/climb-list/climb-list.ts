import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ClimbCard } from '../climb-card/climb-card';
import { ClimbService } from '../services/climb-service';
import { Climb } from '../model/climb';

@Component({
  selector: 'app-climb-list',
  imports: [CommonModule, ClimbCard, FormsModule],
  templateUrl: './climb-list.html',
  styleUrl: './climb-list.css',
})
export class ClimbList implements OnInit {
  climbs: Climb[] = [];
  filteredClimbs: Climb[] = [];
  
  // Filter properties
  selectedStatus: string = 'all';
  selectedGrade: string = 'all';
  availableGrades: string[] = [];
  availableStatuses: string[] = ['all', 'Top', 'Flash', 'Attempted'];

  constructor(private climbService: ClimbService) {}

  ngOnInit(): void {
    this.loadClimbs();
  }

  loadClimbs(): void {
    this.climbService.getClimbs().subscribe({
      next: (climbs) => {
        this.climbs = climbs;
        this.filteredClimbs = climbs;
        this.extractAvailableGrades();
        this.applyFilters();
      },
      error: (error) => {
        console.error('Error loading climbs:', error);
      }
    });
  }

  extractAvailableGrades(): void {
    const grades = new Set(this.climbs.map(c => c.grade));
    this.availableGrades = ['all', ...Array.from(grades).sort()];
  }

  applyFilters(): void {
    this.filteredClimbs = this.climbs.filter(climb => {
      const statusMatch = this.selectedStatus === 'all' || climb.status === this.selectedStatus;
      const gradeMatch = this.selectedGrade === 'all' || climb.grade === this.selectedGrade;
      return statusMatch && gradeMatch;
    });
  }

  onStatusChange(): void {
    this.applyFilters();
  }

  onGradeChange(): void {
    this.applyFilters();
  }

  resetFilters(): void {
    this.selectedStatus = 'all';
    this.selectedGrade = 'all';
    this.applyFilters();
  }

  onDeleteClimb(id: number): void {
    this.climbService.deleteClimb(id).subscribe({
      next: () => {
        this.loadClimbs();
      },
      error: (error) => {
        console.error('Error deleting climb:', error);
        alert('Failed to delete climb');
      }
    });
  }
}


