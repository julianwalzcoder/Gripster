import { Component } from '@angular/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ClimbService } from '../services/climb-service';
import { AuthService } from '../services/auth-service';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { GymService, GymOption } from '../services/gym-service';

interface GradeOption {
    id: number;      // This will be stored in the DB
    display: string; // French grade
    vScale: string;  // V-scale equivalent
}

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
        private gymService: GymService
    ) { }

    currentAdmin: User = JSON.parse(localStorage.getItem('loggedInUser')!);

    // Form Controls
    gyms: GymOption[] = []; // fetched from API

    grades: GradeOption[] = [
        { id: 1, display: '3', vScale: 'VB' },
        { id: 2, display: '4', vScale: 'V0' },
        { id: 3, display: '5', vScale: 'V1' },
        { id: 4, display: '6A', vScale: 'V2' },
        { id: 5, display: '6B', vScale: 'V3' },
        { id: 6, display: '6C', vScale: 'V4' },
        { id: 7, display: '7A', vScale: 'V5' },
        { id: 8, display: '7B', vScale: 'V6' },
        { id: 9, display: '7C', vScale: 'V7' },
        { id: 10, display: '8A', vScale: 'V8' }
    ];

    gradingScaleControl = new FormControl('French'); // default
    gradeControl = new FormControl(null, Validators.required);

    setDate: FormControl<string | null> = new FormControl(new Date().toISOString().substring(0, 10));
    removeDate: FormControl<string | null> = new FormControl(null);

    climbFormGroup: FormGroup = new FormGroup({
        gymId: new FormControl<number | null>(null, [Validators.required]),
        gradeId: new FormControl<number | null>(null, [Validators.required]),
        setDate: new FormControl<string | null>(null),
        removeDate: new FormControl<string | null>(null)
    });

    ngOnInit() {
        this.gymService.getGyms().subscribe({
            next: (gyms) => this.gyms = gyms,
            error: (err) => {
                console.error('Failed to load gyms', err);
                this.gyms = [];
            }
        });
    }

    successMessage: string | null = null;

    addClimb() {
        if (!this.climbFormGroup.valid) return;
        
        const adminId = this.authService.getAdminId();
        if (!adminId) {
            console.error('No admin ID found. Please ensure you are logged in as an admin.');
            return;
        }

        const formValue = this.climbFormGroup.value;

        this.climbService.addClimb({
            routeId: 0,
            gymId: formValue.gymId!,
            gradeId: formValue.gradeId!,
            grade: formValue.grade!,
            status: '',
            setDate: formValue.setDate ? new Date(formValue.setDate) : undefined as unknown as Date,
            removeDate: formValue.removeDate ? new Date(formValue.removeDate) : undefined as unknown as Date,
            adminId: adminId
        }).subscribe({
            next: () => {
                this.successMessage = 'Climb added successfully!';
                // Redirect to climbs page for the selected gym
                setTimeout(() => {
                    this.router.navigate(['/climbs/', formValue.gymId]);
                }, 1000);
            },
            error: (err: any) => console.error('Error creating climb:', err)
        });
    }
}