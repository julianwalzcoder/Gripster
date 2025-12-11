import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ClimbCard } from '../climb-card/climb-card';
import { ClimbService } from '../services/climb-service';
import { Climb } from '../model/climb';

@Component({
  selector: 'app-my-climbs',
  standalone: true,
  imports: [
    CommonModule,
    ClimbCard,
    FormsModule,
    MatFormFieldModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './my-climbs.html',
  styleUrl: './my-climbs.css',
})
export class MyClimbs implements OnInit {
  climbs: Climb[] = [];
  filteredClimbs: Climb[] = [];

  selectedStatus: string = 'all';
  selectedGrade: string = 'all';
  availableGrades: string[] = [];
  availableStatuses: string[] = ['all', 'Top', 'Flash', 'Attempted'];

  constructor(private climbService: ClimbService) {}

  ngOnInit(): void {
    this.loadMyClimbs();
  }

  loadMyClimbs(): void {
    this.climbService.getClimbs().subscribe({
      next: (climbs) => {
        this.climbs = climbs.filter(c =>
          (c.status ?? '') === 'Top' || (c.status ?? '') === 'Flash'
        );
        this.filteredClimbs = this.climbs;
        this.extractAvailableGrades();
        this.applyFilters();
      },
      error: (error) => {
        console.error('Error loading climbs:', error);
      }
    });
  }

  extractAvailableGrades(): void {
    const grades = new Set(this.climbs.map(c => (c.grade ?? '')));
    this.availableGrades = ['all', ...Array.from(grades).sort().filter(g => g !== '')];
  }

  applyFilters(): void {
    this.filteredClimbs = this.climbs.filter(climb => {
      const status = climb.status ?? '';
      const grade = climb.grade ?? '';
      const statusMatch = this.selectedStatus === 'all' || status === this.selectedStatus;
      const gradeMatch = this.selectedGrade === 'all' || grade === this.selectedGrade;
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
    if (confirm('Are you sure you want to delete this climb?')) {
      this.climbService.deleteClimb(id).subscribe({
        next: () => this.loadMyClimbs(),
        error: (error) => {
          console.error('Error deleting climb:', error);
          alert('Failed to delete climb');
        }
      });
    }
  }
}
