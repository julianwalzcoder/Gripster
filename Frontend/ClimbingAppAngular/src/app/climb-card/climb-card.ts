import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Climb } from '../model/climb';
import { Router } from '@angular/router';  
import { ClimbService } from '../services/climb-service';

@Component({
  selector: 'app-climb-card',
  imports: [CommonModule],
  templateUrl: './climb-card.html',
  styleUrl: './climb-card.css',
})
export class ClimbCard {
  constructor(private router: Router, private climbService: ClimbService) {}

  @Input() climb!: Climb;
  @Output() delete = new EventEmitter<number>();

  deleteClimb() {
    if (this.climb) {
      this.delete.emit(this.climb.id);
    }
  }

  editClimb(id: number) {
    // Navigate to the edit page for the climb
    this.router.navigate(['/edit-climb', id]);
  }
}
