import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';


interface GymOption {
    id: number;
    name: string;
}

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

    gyms: GymOption[] = [
        { id: 1, name: 'CPH Sydhavn ' },
        { id: 2, name: 'CPH Vanløse' },
        { id: 3, name: 'CPH Valby' },
        { id: 4, name: 'CPH Østerbro' },
        { id: 5, name: 'Malmö' }
    ];

    gymControl = new FormControl<number | null>(null, [Validators.required]);

    form = new FormGroup({
        gymId: this.gymControl
    });

    constructor(private router: Router) {
        // Load previous selection if present
        const savedId = localStorage.getItem('selectedGymId');
        if (savedId) {
            this.gymControl.setValue(Number(savedId));
        }
    }

    saveGym() {
        if (this.form.invalid || this.gymControl.value == null) return;


        localStorage.setItem('selectedGymId', String(this.gymControl.value));

        // after selecting gym, go to that gym's climbs
        this.router.navigate(['/climbs', this.gymControl.value]);

    }
}

export class AppComponent { }

