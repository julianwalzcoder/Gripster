import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { ClimbService } from '../services/climb-service';
import { Climb } from '../model/climb';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';

@Component({
  selector: 'app-edit-climb',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatSnackBarModule
  ],

  templateUrl: './edit-climb.html',
  styleUrls: ['./edit-climb.css']
})
export class EditClimb implements OnInit {
  climbFormGroup!: FormGroup;
  climb!: Climb;
  gyms: { id: number; name: string }[] = [];
  grades: { id: number; display: string }[] = [];

  constructor(
    private fb: FormBuilder,
    private climbService: ClimbService,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar,
    private router: Router
  ) { }

  ngOnInit(): void {
    const climbId = Number(this.route.snapshot.paramMap.get('id'));
    this.climbService.getClimb(climbId).subscribe(climb => {
      this.climb = climb;
      this.initForm();
    });

    this.loadGyms();
    this.loadGrades();
  }

  initForm(): void {
    this.climbFormGroup = this.fb.group({
      gymId: [this.climb.gymId, Validators.required],
      gradeId: [this.climb.gradeId, Validators.required],
      setDate: [this.climb.setDate],
      removeDate: [this.climb.removeDate],
      adminId: [this.climb.adminId]
    });
  }

  loadGyms(): void {
    this.gyms = [
      { id: 1, name: 'CPH Sydhavn' },
      { id: 2, name: 'CPH Vanløse' },
      { id: 3, name: 'CPH Valby' },
      { id: 4, name: 'CPH Østerbro' },
      { id: 5, name: 'Malmö' }
    ];
  }

  loadGrades(): void {
    this.grades = [
      { id: 1, display: '3' },
      { id: 2, display: '4' },
      { id: 3, display: '5' },
      { id: 4, display: '6A' },
      { id: 5, display: '6B' },
      { id: 6, display: '6C' },
      { id: 7, display: '7A' },
      { id: 8, display: '7B' },
      { id: 9, display: '7C' },
      { id: 10, display: '8A' }
    ];
  }

  updateClimb(): void {
    if (!this.climbFormGroup.valid) return;

    const form = this.climbFormGroup.value;

    const dto = {
      id: this.climb.routeId, // send routeId as Id to backend
      gymID: form.gymId!,
      gradeID: form.gradeId!,
      setDate: form.setDate
        ? new Date(form.setDate).toISOString()
        : new Date(this.climb.setDate).toISOString(),
      removeDate: form.removeDate
        ? new Date(form.removeDate).toISOString()
        : (this.climb.removeDate
          ? new Date(this.climb.removeDate).toISOString()
          : null),
      adminID: this.climb.adminId
    };

    this.climbService.updateClimbAdmin(dto).subscribe({
      next: () => {
        this.snackBar.open('Climb updated', 'OK', {
          duration: 3000,
          verticalPosition: 'bottom'
        });
        this.router.navigate(['/climbs', this.climb.gymId]);
      },
      error: (err: unknown) => {
        console.error('Failed to update climb', err);
        this.snackBar.open('Failed to update climb', 'Dismiss', {
          duration: 4000,
          panelClass: ['snack-error']
        });
      }
    });
  }
}
