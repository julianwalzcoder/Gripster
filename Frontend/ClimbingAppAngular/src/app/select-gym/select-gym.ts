import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';
import { GymService, GymOption } from '../services/gym-service';

@Component({
    selector: 'app-select-gym',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        MatFormFieldModule,
        MatSelectModule,
        MatButtonModule
    ],
    templateUrl: './select-gym.html',
    styleUrl: './select-gym.css'
})
export class SelectGym {

    gyms: GymOption[] = [];

    gymControl = new FormControl<number | null>(null, [Validators.required]);

    form = new FormGroup({
        gymId: this.gymControl
    });

    constructor(private router: Router, private gymService: GymService) {
        // Load previous selection if present
        const savedId = localStorage.getItem('selectedGymId');
        if (savedId) {
            this.gymControl.setValue(Number(savedId));
        }
    }

    ngOnInit(): void {
        this.gymService.getGyms().subscribe({
            next: (gyms) => this.gyms = gyms,
            error: (err) => {
                console.error('Failed to load gyms', err);
                this.gyms = [];
            }
        });
    }

    saveGym() {
        if (this.form.invalid || this.gymControl.value == null) return;

        localStorage.setItem('selectedGymId', String(this.gymControl.value));

        // Navigate to climbs page - the service will read selectedGymId from localStorage
        this.router.navigate(['/climbs/', this.gymControl.value]);
    }
}

export class AppComponent { }


