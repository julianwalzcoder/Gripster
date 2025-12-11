import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl, ValidatorFn } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { ClimbService } from '../services/climb-service';
import { RouteDetails } from '../services/climb-service';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { GymService, GymOption } from '../services/gym-service';
import { GradeService, GradeOption } from '../services/grade-service';
import { forkJoin } from 'rxjs';

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
  climb!: RouteDetails; // ensure this is RouteDetails (grade: string | null)
  gyms: GymOption[] = [];
  grades: GradeOption[] = [];

  compareIds = (a: unknown, b: unknown) => Number(a) === Number(b);

  constructor(
    private fb: FormBuilder,
    private climbService: ClimbService,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar,
    private router: Router,
    private gymService: GymService,
    private gradeService: GradeService
  ) { }

  ngOnInit(): void {
    this.initForm();
    const climbId = Number(this.route.snapshot.paramMap.get('id'));

    forkJoin({
      gyms: this.gymService.getGyms(),
      grades: this.gradeService.getGrades(),
      climb: this.climbService.getClimb(climbId)
    }).subscribe(({ gyms, grades, climb }) => {
      this.gyms = gyms ?? [];
      this.grades = grades ?? [];
      this.climb = climb;

      // derive gradeId if missing by matching display
      let gradeId = this.climb.gradeId;
      if (gradeId === undefined || gradeId === null) {
        const match = this.grades.find(g => (g.display ?? '').toLowerCase() === (this.climb.grade ?? '').toLowerCase());
        if (match) gradeId = match.id;
      }

      this.climbFormGroup.patchValue({
        gymId: Number(climb.gymId),
        gradeId: gradeId ?? null,
        setDate: this.toDateInput(climb.setDate),
        removeDate: this.toDateInput(climb.removeDate),
        adminId: Number(climb.adminId ?? 0)
      });
    });
  }

  private toDateInput(value: string | Date | null | undefined): string | null {
    if (!value) return null;
    const d = new Date(value);
    const pad = (n: number) => n.toString().padStart(2, '0');
    // return local date as YYYY-MM-DD for <input type="date">
    return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`;
  }

  private toIsoOrNull(value: string | Date | null | undefined): string | null {
    if (!value) return null;
    const d = new Date(value);
    return isNaN(d.getTime()) ? null : d.toISOString();
  }

  private parseDate(value: string | Date | null | undefined): Date | null {
    if (!value) return null;
    const d = new Date(value);
    return isNaN(d.getTime()) ? null : d;
  }

  private dateOrderValidator: ValidatorFn = (control: AbstractControl) => {
    const group = control as FormGroup;
    const setDate = this.parseDate(group.get('setDate')?.value);
    const removeCtrl = group.get('removeDate');
    const removeDate = this.parseDate(removeCtrl?.value);

    // clear previous error
    removeCtrl?.setErrors(null);

    if (!setDate || !removeDate) return null;

    if (removeDate < setDate) {
      // set error on the removeDate control so its mat-error shows
      removeCtrl?.setErrors({ dateOrder: true });
      return { dateOrder: true };
    }
    return null;
  };

  initForm(): void {
    this.climbFormGroup = this.fb.group({
      gymId: [null, Validators.required],
      gradeId: [null, Validators.required],
      setDate: [null],       // string 'YYYY-MM-DD' or null
      removeDate: [null],    // string 'YYYY-MM-DD' or null
      adminId: [null]
    });
    this.climbFormGroup.addValidators(this.dateOrderValidator);
  }

  loadGyms(): void {
    this.gymService.getGyms().subscribe({
      next: gyms => this.gyms = gyms ?? [],
      error: err => {
        console.error('[EditClimb] loadGyms error', err);
        this.gyms = [];
      }
    });
  }

  loadGrades(): void {
    this.gradeService.getGrades().subscribe({
      next: grades => this.grades = grades ?? [],
      error: err => {
        console.error('[EditClimb] loadGrades error', err);
        this.grades = [];
      }
    });
  }

  updateClimb(): void {
    if (!this.climbFormGroup.valid || this.climbFormGroup.hasError('dateOrder')) return;
    if (!this.climb || !this.climbFormGroup.valid) return;
    const form = this.climbFormGroup.value;

    const setDateIso =
      form.setDate ? this.toIsoOrNull(form.setDate)
                   : (this.climb.setDate ? this.toIsoOrNull(this.climb.setDate) : new Date().toISOString());

    const removeDateIso =
      form.removeDate != null && form.removeDate !== ''
        ? this.toIsoOrNull(form.removeDate)
        : this.toIsoOrNull(this.climb.removeDate);

    const dto = {
      id: this.climb.routeId,
      gymID: Number(form.gymId),
      gradeID: Number(form.gradeId),
      setDate: setDateIso,
      removeDate: removeDateIso,
      adminID: Number(this.climb.adminId)
    };

    this.climbService.updateClimbAdmin(dto).subscribe({
      next: () => {
        this.snackBar.open('Climb updated', 'OK', { duration: 3000, verticalPosition: 'bottom' });
        this.router.navigate(['/climbs', this.climb.gymId]);
      },
      error: (err: unknown) => {
        console.error('Failed to update climb', err);
        this.snackBar.open('Failed to update climb', 'Dismiss', { duration: 4000, panelClass: ['snack-error'] });
      }
    });
  }
}
