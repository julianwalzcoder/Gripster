import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { ClimbService } from '../services/climb-service';
import { Climb } from '../model/climb';
import { ClimbCard } from '../climb-card/climb-card';

@Component({
  selector: 'app-my-projects',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    ClimbCard
  ],
  templateUrl: './my-projects.html',
  styleUrl: './my-projects.css'
})
export class MyProjects implements OnInit {
  climbs: Climb[] = [];
  filteredClimbs: Climb[] = [];

  availableStatuses: string[] = ['all', 'Top', 'Flash', 'Attempted'];
  availableGrades: string[] = ['all'];

  selectedStatus = 'all';
  selectedGrade = 'all';
  searchTerm = '';
  sortBy: 'setDate' | 'grade' | 'routeId' = 'setDate';
  sortDirection: 'asc' | 'desc' = 'asc';

  constructor(private climbService: ClimbService) {}

  ngOnInit(): void {
    this.loadClimbs();
  }

  loadClimbs(): void {
    this.climbService.getClimbs().subscribe({
      next: (climbs) => {
        // If projects should only show “Attempted”, filter here:
        this.climbs = (climbs ?? []).filter(c => (c.status ?? '').toLowerCase() === 'attempted');
        const grades = Array.from(new Set(this.climbs.map(c => (c.grade ?? '').trim()))).filter(g => g);
        this.availableGrades = ['all', ...grades.sort()];
        this.applyFiltersAndSort();
      },
      error: (err) => console.error('Error loading projects', err)
    });
  }

  onStatusChange(): void { this.applyFiltersAndSort(); }
  onGradeChange(): void { this.applyFiltersAndSort(); }
  onSearchChange(): void { this.applyFiltersAndSort(); }
  onSortByChange(): void { this.applyFiltersAndSort(); }
  toggleSortDirection(): void {
    this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    this.applyFiltersAndSort();
  }
  resetFilters(): void {
    this.selectedStatus = 'all';
    this.selectedGrade = 'all';
    this.searchTerm = '';
    this.sortBy = 'setDate';
    this.sortDirection = 'asc';
    this.applyFiltersAndSort();
  }

  private applyFiltersAndSort(): void {
    let result = this.climbs.filter(c => {
      const statusMatch = this.selectedStatus === 'all' || (c.status ?? '').toLowerCase() === this.selectedStatus.toLowerCase();
      const gradeMatch = this.selectedGrade === 'all' || (c.grade ?? '').toLowerCase() === this.selectedGrade.toLowerCase();
      return statusMatch && gradeMatch;
    });

    const term = this.searchTerm.trim().toLowerCase();
    if (term) {
      result = result.filter(c =>
        (c.grade ?? '').toLowerCase().includes(term) ||
        (c.status ?? '').toLowerCase().includes(term) ||
        String(c.routeId).includes(term)
      );
    }

    result.sort((a, b) => {
      let aVal: any;
      let bVal: any;
      switch (this.sortBy) {
        case 'setDate':
          aVal = a.setDate ?? '';
          bVal = b.setDate ?? '';
          break;
        case 'grade':
          aVal = (a.grade ?? '').toLowerCase();
          bVal = (b.grade ?? '').toLowerCase();
          break;
        case 'routeId':
          aVal = a.routeId;
          bVal = b.routeId;
          break;
      }
      if (typeof aVal === 'string' && typeof bVal === 'string') {
        if (aVal < bVal) return this.sortDirection === 'asc' ? -1 : 1;
        if (aVal > bVal) return this.sortDirection === 'asc' ? 1 : -1;
        return 0;
      }
      const cmp = (aVal as number) - (bVal as number);
      return this.sortDirection === 'asc' ? cmp : -cmp;
    });

    this.filteredClimbs = result;
  }

  onDeleteClimb(id: number): void {
    if (confirm('Are you sure you want to delete this climb?')) {
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
}

