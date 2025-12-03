import { Component } from '@angular/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ClimbService } from '../services/climb-service';

@Component({
    selector: 'app-add-climb',
    imports: [MatButtonModule, MatFormFieldModule, MatInputModule, ReactiveFormsModule, CommonModule],
    templateUrl: './add-climb.html',
    styleUrl: './add-climb.css'
})
export class AddClimb {
    constructor(private climbService: ClimbService, private router: Router) { }


    // Form Controls
    gymId: FormControl = new FormControl('', [
        Validators.required,
        Validators.pattern('\\d+')
    ]);


    gradeId: FormControl = new FormControl('', [
        Validators.required,
        Validators.pattern('\\d+')
    ]);

    setDate: FormControl<string | null> = new FormControl(null);
    removeDate: FormControl<string | null> = new FormControl(null);


    adminId: FormControl<string | null> = new FormControl('', [
        Validators.pattern('\\d*')
    ]);


    climbFormGroup: FormGroup = new FormGroup({
        gymId: this.gymId,
        gradeId: this.gradeId,
        setDate: this.setDate,
        removeDate: this.removeDate,
        adminId: this.adminId
    });


    addClimb() {
        if (!this.climbFormGroup.valid) {
            console.log('Data not valid');
            return;
        }


        this.climbService.addClimb({
            id: 0,
            gymId: Number(this.gymId.value),
            gradeId: Number(this.gradeId.value),
            setDate: this.setDate.value ? new Date(this.setDate.value) : null,
            removeDate: this.removeDate.value ? new Date(this.removeDate.value) : null,
            adminId: this.adminId.value ? Number(this.adminId.value) : null
        }).subscribe({
            next: () => {
                console.log('Climb created');
                this.router.navigate(['/climbs']);
            },
            error: (err) => console.error('Something went wrong: ' + err)
        });
    }
}
