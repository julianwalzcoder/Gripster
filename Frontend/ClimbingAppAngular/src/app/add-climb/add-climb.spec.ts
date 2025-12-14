import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AddClimb } from './add-climb';
import { ClimbService } from '../services/climb-service';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { GymService } from '../services/gym-service';
import { GradeService } from '../services/grade-service';
import { AuthService } from '../services/auth-service';
import { MatSnackBar } from '@angular/material/snack-bar';

describe('AddClimb', () => {
  let component: AddClimb;
  let fixture: ComponentFixture<AddClimb>;
  let climbService: jasmine.SpyObj<ClimbService>;
  let router: jasmine.SpyObj<Router>;
  let gymService: jasmine.SpyObj<GymService>;
  let gradeService: jasmine.SpyObj<GradeService>;
  let snackBar: jasmine.SpyObj<MatSnackBar>;

  beforeEach(async () => {
    const climbServiceSpy = jasmine.createSpyObj('ClimbService', ['addClimb']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    const gymServiceSpy = jasmine.createSpyObj('GymService', ['getGyms']);
    const gradeServiceSpy = jasmine.createSpyObj('GradeService', ['getGrades']);
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['isLoggedIn']);
    const snackBarSpy = jasmine.createSpyObj('MatSnackBar', ['open']);

    // Mock localStorage
    spyOn(localStorage, 'getItem').and.returnValue(JSON.stringify({ id: 1, name: 'Admin', role: 'admin' }));

    gymServiceSpy.getGyms.and.returnValue(of([{ id: 1, name: 'Gym 1' }]));
    gradeServiceSpy.getGrades.and.returnValue(of([{ id: 1, name: 'V1' }]));

    await TestBed.configureTestingModule({
      imports: [AddClimb],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: ClimbService, useValue: climbServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: GymService, useValue: gymServiceSpy },
        { provide: GradeService, useValue: gradeServiceSpy },
        { provide: AuthService, useValue: authServiceSpy },
        { provide: MatSnackBar, useValue: snackBarSpy }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddClimb);
    component = fixture.componentInstance;
    climbService = TestBed.inject(ClimbService) as jasmine.SpyObj<ClimbService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    gymService = TestBed.inject(GymService) as jasmine.SpyObj<GymService>;
    gradeService = TestBed.inject(GradeService) as jasmine.SpyObj<GradeService>;
    snackBar = TestBed.inject(MatSnackBar) as jasmine.SpyObj<MatSnackBar>;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Form Validation', () => {
    it('should initialize with an invalid form', () => {
      expect(component.climbFormGroup.valid).toBeFalsy();
    });

    it('should have invalid form when required fields are empty', () => {
      component.climbFormGroup.patchValue({ gymId: null, gradeId: null });
      expect(component.climbFormGroup.valid).toBeFalsy();
    });

    it('should validate gymId as required', () => {
      const gymIdControl = component.climbFormGroup.get('gymId');
      gymIdControl?.setValue(null);
      expect(gymIdControl?.hasError('required')).toBeTruthy();
      
      gymIdControl?.setValue(1);
      expect(gymIdControl?.hasError('required')).toBeFalsy();
    });

    it('should validate gradeId as required', () => {
      const gradeIdControl = component.climbFormGroup.get('gradeId');
      gradeIdControl?.setValue(null);
      expect(gradeIdControl?.hasError('required')).toBeTruthy();
      
      gradeIdControl?.setValue(12);
      expect(gradeIdControl?.hasError('required')).toBeFalsy();
    });

    it('should allow removeDate to be empty (optional)', () => {
      component.climbFormGroup.setValue({
        gymId: 1,
        gradeId: 12,
        setDate: '2025-01-15',
        removeDate: null
      });
      
      expect(component.climbFormGroup.valid).toBeTruthy();
    });

    it('should have valid form when all required fields are correctly filled', () => {
      component.climbFormGroup.setValue({
        gymId: 1,
        gradeId: 12,
        setDate: '2025-01-15',
        removeDate: null
      });
      
      expect(component.climbFormGroup.valid).toBeTruthy();
    });
  });

  describe('addClimb() Method', () => {
    it('should not call service when form is invalid', () => {
      component.climbFormGroup.setValue({
        gymId: null,
        gradeId: null,
        setDate: null,
        removeDate: null
      });

      component.addClimb();

      expect(climbService.addClimb).not.toHaveBeenCalled();
      expect(router.navigate).not.toHaveBeenCalled();
    });

    it('should call climbService.addClimb with correct data when form is valid', () => {
      climbService.addClimb.and.returnValue(of(void 0));
      
      component.climbFormGroup.setValue({
        gymId: 1,
        gradeId: 12,
        setDate: '2025-01-15',
        removeDate: null
      });

      component.addClimb();

      expect(climbService.addClimb).toHaveBeenCalledWith(jasmine.objectContaining({
        routeId: 0,
        gymId: 1,
        gradeId: 12,
        status: '',
        setDate: jasmine.any(String),
        removeDate: null
      }));
    });

    it('should convert setDate string to ISO format', () => {
      climbService.addClimb.and.returnValue(of(void 0));
      
      component.climbFormGroup.setValue({
        gymId: 1,
        gradeId: 12,
        setDate: '2025-01-15',
        removeDate: null
      });

      component.addClimb();

      const callArgs = climbService.addClimb.calls.mostRecent().args[0];
      expect(callArgs.setDate).toContain('2025-01-15');
    });

    it('should set removeDate to null when empty', () => {
      climbService.addClimb.and.returnValue(of(void 0));
      
      component.climbFormGroup.setValue({
        gymId: 1,
        gradeId: 12,
        setDate: '2025-01-15',
        removeDate: null
      });

      component.addClimb();

      const callArgs = climbService.addClimb.calls.mostRecent().args[0];
      expect(callArgs.removeDate).toBeNull();
    });

    it('should include routeId and status in payload', () => {
      climbService.addClimb.and.returnValue(of(void 0));
      
      component.climbFormGroup.setValue({
        gymId: 1,
        gradeId: 12,
        setDate: '2025-01-15',
        removeDate: null
      });

      component.addClimb();

      const callArgs = climbService.addClimb.calls.mostRecent().args[0];
      expect(callArgs.routeId).toBe(0);
      expect(callArgs.status).toBe('');
    });

    it('should show snackbar and navigate on successful service response', (done) => {
      climbService.addClimb.and.returnValue(of(void 0));
      
      component.climbFormGroup.setValue({
        gymId: 1,
        gradeId: 12,
        setDate: '2025-01-15',
        removeDate: null
      });

      component.addClimb();

      expect(snackBar.open).toHaveBeenCalledWith('Climb added', 'OK', { duration: 3000, verticalPosition: 'bottom' });
      
      setTimeout(() => {
        expect(router.navigate).toHaveBeenCalledWith(['/climbs/', 1]);
        done();
      }, 1100);
    });

    it('should handle error when service fails', () => {
      const errorResponse = { status: 500, message: 'Server error' };
      climbService.addClimb.and.returnValue(throwError(() => errorResponse));
      spyOn(console, 'error');
      
      component.climbFormGroup.setValue({
        gymId: 1,
        gradeId: 12,
        setDate: '2025-01-15',
        removeDate: null
      });

      component.addClimb();

      expect(console.error).toHaveBeenCalledWith('Error creating climb:', errorResponse);
      expect(router.navigate).not.toHaveBeenCalled();
    });
  });

  describe('Form Initialization', () => {
    it('should load gyms on init', () => {
      expect(gymService.getGyms).toHaveBeenCalled();
      expect(component.gyms.length).toBeGreaterThan(0);
    });

    it('should load grades on init', () => {
      expect(gradeService.getGrades).toHaveBeenCalled();
      expect(component.grades.length).toBeGreaterThan(0);
    });

    it('should initialize setDate with today\'s date', () => {
      const setDateValue = component.climbFormGroup.get('setDate')?.value;
      expect(setDateValue).toBeTruthy();
    });
  });
});
