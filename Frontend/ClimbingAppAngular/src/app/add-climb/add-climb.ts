import { Component } from '@angular/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { FormControl, FormGroup, ReactiveFormsModule, Validators, FormBuilder, AbstractControl, ValidatorFn } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ClimbService } from '../services/climb-service';
import { AuthService } from '../services/auth-service';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { GymService, GymOption } from '../services/gym-service';
import { GradeService, GradeOption } from '../services/grade-service';
import type { AddClimbRequest } from '../services/climb-service';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';

interface User {
    id: number;
    name: string;
    role: 'admin';
}

@Component({
    selector: 'add-climb',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        MatFormFieldModule,
        MatInputModule,
        MatButtonModule,
        MatSelectModule,
        MatCardModule
    ],
    templateUrl: './add-climb.html',
    styleUrls: ['./add-climb.css']
})

export class AddClimb {
    constructor(
        private climbService: ClimbService, 
        private router: Router,
        private authService: AuthService,
        private gymService: GymService,
        private gradeService: GradeService,
        private fb: FormBuilder,
        private snackBar: MatSnackBar,
    ) { }

    currentAdmin: User = JSON.parse(localStorage.getItem('loggedInUser')!);

    // Form Controls
    gyms: GymOption[] = []; // fetched from API
    grades: GradeOption[] = []; // fetched from API

    gradingScaleControl = new FormControl('French'); // default
    gradeControl = new FormControl(null, Validators.required);

    setDate: FormControl<string | null> = new FormControl(new Date().toISOString().substring(0, 10));
    removeDate: FormControl<string | null> = new FormControl(null);

    climbFormGroup!: FormGroup;

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

        removeCtrl?.setErrors(null);

        if (!setDate || !removeDate) return null;

        if (removeDate < setDate) {
            removeCtrl?.setErrors({ dateOrder: true });
            return { dateOrder: true };
        }
        return null;
    };

    ngOnInit(): void {
        this.climbFormGroup = this.fb.group({
            gymId: [null, Validators.required],
            gradeId: [null, Validators.required],
            setDate: [this.todayYmd()],
            removeDate: [null]
        });
        this.climbFormGroup.addValidators(this.dateOrderValidator);

        this.gymService.getGyms().subscribe({
            next: (gyms) => this.gyms = gyms,
            error: (err) => {
                console.error('Failed to load gyms', err);
                this.gyms = [];
            }
        });

        this.gradeService.getGrades().subscribe({
            next: (grades) => this.grades = grades,
            error: (err) => {
                console.error('Failed to load grades', err);
                this.grades = [];
            }
        });
    }

    successMessage: string | null = null;

    addClimb(): void {
        if (this.climbFormGroup.invalid || this.climbFormGroup.hasError('dateOrder')) return;

        const formValue = this.climbFormGroup.value;

        const payload: AddClimbRequest = {
            routeId: 0,
            gymId: Number(formValue.gymId),
            gradeId: Number(formValue.gradeId),
            status: '',
            setDate: formValue.setDate ? new Date(formValue.setDate).toISOString() : null,
            removeDate: formValue.removeDate ? new Date(formValue.removeDate).toISOString() : null
        };

        this.climbService.addClimb(payload).subscribe({
            next: () => {
                this.snackBar.open('Climb added', 'OK', { duration: 3000, verticalPosition: 'bottom' });
                setTimeout(() => this.router.navigate(['/climbs/', formValue.gymId]), 1000);
            },
            error: (err: any) => console.error('Error creating climb:', err)
        });
    }

    private todayYmd(): string {
        const d = new Date();
        const pad = (n: number) => String(n).padStart(2, '0');
        return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`;
    }
}