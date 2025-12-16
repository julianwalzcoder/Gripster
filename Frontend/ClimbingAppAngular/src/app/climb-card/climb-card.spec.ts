import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ClimbCard } from './climb-card';
import { ClimbService } from '../services/climb-service';
import { AuthService } from '../services/auth-service';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ActivatedRoute } from '@angular/router';

describe('ClimbCard', () => {
  let component: ClimbCard;
  let fixture: ComponentFixture<ClimbCard>;
  let climbService: jasmine.SpyObj<ClimbService>;
  let authService: jasmine.SpyObj<AuthService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    const climbServiceSpy = jasmine.createSpyObj('ClimbService', [
      'getAverageRating',
      'getUserRating',
      'deleteClimb',
      'updateClimbStatus',
      'setRating'
    ]);
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['getCurrentUserId', 'isAdmin']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    climbServiceSpy.getAverageRating.and.returnValue(of(4.5));
    climbServiceSpy.getUserRating.and.returnValue(of(5));
    authServiceSpy.getCurrentUserId.and.returnValue(1);
    authServiceSpy.isAdmin.and.returnValue(false);
    routerSpy.navigate.and.returnValue(Promise.resolve(true));

    await TestBed.configureTestingModule({
      imports: [ClimbCard],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: ClimbService, useValue: climbServiceSpy },
        { provide: AuthService, useValue: authServiceSpy },
        { provide: Router, useValue: routerSpy },
        {
          provide: ActivatedRoute,
          useValue: {
            params: of({}),
            snapshot: { params: {} }
          }
        }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClimbCard);
    component = fixture.componentInstance;
    climbService = TestBed.inject(ClimbService) as jasmine.SpyObj<ClimbService>;
    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    
    component.climb = {
      routeId: 1,
      gymId: 1,
      grade: 'V5',
      status: 'Not Attempted',
      setDate: null,
      removeDate: null
    };
    
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Initialization', () => {
    it('should load average rating on init', () => {
      expect(climbService.getAverageRating).toHaveBeenCalledWith(1);
      expect(component.avgRating).toBe(4.5);
    });

    it('should load user rating on init when user is logged in', () => {
      expect(climbService.getUserRating).toHaveBeenCalledWith(1, 1);
      expect(component.userRating).toBe(5);
    });

    it('should handle null average rating', () => {
      climbService.getAverageRating.and.returnValue(of(null));
      component.ngOnInit();
      expect(component.avgRating).toBeUndefined();
    });

    it('should handle 404 error for user rating gracefully', () => {
      climbService.getUserRating.and.returnValue(throwError(() => ({ status: 404 })));
      component.ngOnInit();
      expect(component.userRating).toBeUndefined();
    });

    it('should handle other errors for user rating', () => {
      climbService.getUserRating.and.returnValue(throwError(() => ({ status: 500 })));
      spyOn(console, 'error');
      
      component.ngOnInit();
      
      expect(console.error).toHaveBeenCalled();
      expect(component.userRating).toBeUndefined();
    });

    it('should not load user rating when user is not logged in', () => {
      authService.getCurrentUserId.and.returnValue(null);
      climbService.getUserRating.calls.reset();
      
      component.ngOnInit();
      
      expect(climbService.getUserRating).not.toHaveBeenCalled();
    });
  });

  describe('deleteClimb()', () => {
    it('should prompt for confirmation', () => {
      spyOn(window, 'confirm').and.returnValue(false);
      
      component.deleteClimb();
      
      expect(window.confirm).toHaveBeenCalled();
      expect(climbService.deleteClimb).not.toHaveBeenCalled();
    });

    it('should delete climb and emit event on confirmation', () => {
      spyOn(window, 'confirm').and.returnValue(true);
      climbService.deleteClimb.and.returnValue(of(void 0));
      spyOn(component.delete, 'emit');
      
      component.deleteClimb();
      
      expect(climbService.deleteClimb).toHaveBeenCalledWith(1);
      expect(component.delete.emit).toHaveBeenCalledWith(1);
      expect(router.navigate).toHaveBeenCalledWith(['/climbs', 1]);
    });

    it('should handle delete error', () => {
      spyOn(window, 'confirm').and.returnValue(true);
      spyOn(window, 'alert');
      climbService.deleteClimb.and.returnValue(throwError(() => ({ status: 500 })));
      spyOn(console, 'error');
      
      component.deleteClimb();
      
      expect(console.error).toHaveBeenCalled();
      expect(window.alert).toHaveBeenCalledWith('Failed to delete climb');
    });
  });

  describe('editClimb()', () => {
    it('should navigate to edit page', () => {
      component.editClimb(1);
      expect(router.navigate).toHaveBeenCalledWith(['/edit-climb', 1]);
    });
  });

  describe('viewClimbDetails()', () => {
    it('should navigate to climb detail page', () => {
      spyOn(console, 'log');
      
      component.viewClimbDetails();
      
      expect(console.log).toHaveBeenCalledWith('Navigating to climb detail:', 1);
      expect(router.navigate).toHaveBeenCalledWith(['/climb-detail', 1]);
    });
  });

  describe('updateClimbStatus()', () => {
    it('should update status when user is logged in', () => {
      climbService.updateClimbStatus.and.returnValue(of(void 0));
      spyOn(console, 'log');
      
      component.updateClimbStatus(1, 'Top');
      
      expect(climbService.updateClimbStatus).toHaveBeenCalledWith(1, 1, 'Top');
      expect(component.climb.status).toBe('Top');
      expect(console.log).toHaveBeenCalledWith('Status updated successfully');
    });

    it('should alert when user is not logged in', () => {
      authService.getCurrentUserId.and.returnValue(null);
      spyOn(window, 'alert');
      
      component.updateClimbStatus(1, 'Top');
      
      expect(window.alert).toHaveBeenCalledWith('Please log in to update climb status');
      expect(climbService.updateClimbStatus).not.toHaveBeenCalled();
    });

    it('should handle update error', () => {
      climbService.updateClimbStatus.and.returnValue(throwError(() => ({ message: 'Network error' })));
      spyOn(console, 'error');
      spyOn(window, 'alert');
      
      component.updateClimbStatus(1, 'Top');
      
      expect(console.error).toHaveBeenCalled();
      expect(window.alert).toHaveBeenCalledWith('Failed to update climb status. Network error');
    });
  });

  describe('rateClimb()', () => {
    it('should set rating when user is logged in', () => {
      climbService.setRating.and.returnValue(of(void 0));
      climbService.getAverageRating.and.returnValue(of(4.8));
      
      component.rateClimb(5);
      
      expect(component.userRating).toBe(5);
      expect(climbService.setRating).toHaveBeenCalledWith(1, 1, 5);
      expect(climbService.getAverageRating).toHaveBeenCalledWith(1);
    });

    it('should alert when user is not logged in', () => {
      authService.getCurrentUserId.and.returnValue(null);
      spyOn(window, 'alert');
      
      component.rateClimb(5);
      
      expect(window.alert).toHaveBeenCalledWith('Please log in to rate climbs');
      expect(climbService.setRating).not.toHaveBeenCalled();
    });

    it('should handle rating error', () => {
      climbService.setRating.and.returnValue(throwError(() => ({ status: 500 })));
      spyOn(console, 'error');
      spyOn(window, 'alert');
      
      component.rateClimb(5);
      
      expect(console.error).toHaveBeenCalled();
      expect(window.alert).toHaveBeenCalledWith('Rating failed');
    });
  });
});
