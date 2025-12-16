import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SelectGymComponent } from './select-gym';
import { GymService } from '../services/gym-service';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

describe('SelectGymComponent', () => {
  let component: SelectGymComponent;
  let fixture: ComponentFixture<SelectGymComponent>;
  let gymService: jasmine.SpyObj<GymService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    const gymServiceSpy = jasmine.createSpyObj('GymService', ['getGyms', 'setSelectedGym']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    spyOn(localStorage, 'getItem').and.returnValue('1');
    spyOn(localStorage, 'setItem');

    gymServiceSpy.getGyms.and.returnValue(of([
      { id: 1, name: 'Gym 1' },
      { id: 2, name: 'Gym 2' }
    ]));

    await TestBed.configureTestingModule({
      imports: [SelectGymComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: GymService, useValue: gymServiceSpy },
        { provide: Router, useValue: routerSpy }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SelectGymComponent);
    component = fixture.componentInstance;
    gymService = TestBed.inject(GymService) as jasmine.SpyObj<GymService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Initialization', () => {
    it('should load gyms on init', () => {
      expect(gymService.getGyms).toHaveBeenCalled();
      expect(component.gyms.length).toBe(2);
    });

    it('should load saved gym selection from localStorage', () => {
      expect(localStorage.getItem).toHaveBeenCalledWith('selectedGymId');
      expect(component.gymControl.value).toBe(1);
    });

    it('should handle error when loading gyms fails', () => {
      gymService.getGyms.and.returnValue(throwError(() => ({ status: 500 })));
      spyOn(console, 'error');
      
      component.ngOnInit();
      
      expect(console.error).toHaveBeenCalled();
      expect(component.gyms).toEqual([]);
    });
  });

  describe('Form Validation', () => {
    it('should initialize with required validator', () => {
      component.gymControl.setValue(null);
      expect(component.gymControl.hasError('required')).toBeTruthy();
    });

    it('should be valid when gym is selected', () => {
      component.gymControl.setValue(1);
      expect(component.gymControl.valid).toBeTruthy();
    });

    it('should have invalid form when gym is not selected', () => {
      component.gymControl.setValue(null);
      expect(component.form.invalid).toBeTruthy();
    });
  });

  describe('saveGym() Method', () => {
    it('should not save when form is invalid', () => {
      component.gymControl.setValue(null);
      
      component.saveGym();
      
      expect(gymService.setSelectedGym).not.toHaveBeenCalled();
      expect(router.navigate).not.toHaveBeenCalled();
    });

    it('should call setSelectedGym and navigate when form is valid', () => {
      component.gymControl.setValue(1);
      
      component.saveGym();
      
      expect(gymService.setSelectedGym).toHaveBeenCalledWith(1, 'Gym 1');
      expect(router.navigate).toHaveBeenCalledWith(['/climbs', 1]);
    });

    it('should use "All Gyms" as default name when gym not found', () => {
      component.gymControl.setValue(999);
      
      component.saveGym();
      
      expect(gymService.setSelectedGym).toHaveBeenCalledWith(999, 'All Gyms');
    });
  });

  describe('onSelect() Method', () => {
    it('should call setSelectedGym and navigate with gym data', () => {
      const gym = { id: 2, name: 'Gym 2' };
      
      component.onSelect(gym);
      
      expect(gymService.setSelectedGym).toHaveBeenCalledWith(2, 'Gym 2');
      expect(router.navigate).toHaveBeenCalledWith(['/climbs', 2]);
    });
  });
});
