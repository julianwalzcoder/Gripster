import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ClimbList } from './climb-list';
import { ClimbService } from '../services/climb-service';
import { AuthService } from '../services/auth-service';
import { of, throwError } from 'rxjs';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { Climb } from '../model/climb';
import { ActivatedRoute } from '@angular/router';

describe('ClimbList', () => {
  let component: ClimbList;
  let fixture: ComponentFixture<ClimbList>;
  let climbService: jasmine.SpyObj<ClimbService>;
  let authService: jasmine.SpyObj<AuthService>;

  const mockClimbs: Climb[] = [
    { routeId: 1, gymId: 1, grade: 'V5', status: 'Top', setDate: '2025-01-15', removeDate: null },
    { routeId: 2, gymId: 1, grade: 'V7', status: 'Flash', setDate: '2025-01-14', removeDate: null },
    { routeId: 3, gymId: 1, grade: 'V5', status: 'Attempted', setDate: '2025-01-13', removeDate: null }
  ];

  beforeEach(async () => {
    const climbServiceSpy = jasmine.createSpyObj('ClimbService', [
      'getClimbs',
      'getAverageRating',
      'getUserRating',
      'deleteClimb',
      'updateClimbStatus',
      'setRating'
    ]);
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['getCurrentUserId', 'isAdmin']);

    climbServiceSpy.getClimbs.and.returnValue(of(mockClimbs));
    climbServiceSpy.getAverageRating.and.returnValue(of(4.5));
    climbServiceSpy.getUserRating.and.returnValue(of(5));
    authServiceSpy.getCurrentUserId.and.returnValue(1);
    authServiceSpy.isAdmin.and.returnValue(false);

    await TestBed.configureTestingModule({
      imports: [ClimbList],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: ClimbService, useValue: climbServiceSpy },
        { provide: AuthService, useValue: authServiceSpy },
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

    fixture = TestBed.createComponent(ClimbList);
    component = fixture.componentInstance;
    climbService = TestBed.inject(ClimbService) as jasmine.SpyObj<ClimbService>;
    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Initialization', () => {
    it('should load climbs on init', () => {
      expect(climbService.getClimbs).toHaveBeenCalled();
      expect(component.climbs.length).toBe(3);
      expect(component.filteredClimbs.length).toBe(3);
    });

    it('should populate available grades', () => {
      expect(component.availableGrades).toContain('all');
      expect(component.availableGrades).toContain('V5');
      expect(component.availableGrades).toContain('V7');
    });

    it('should handle error when loading climbs fails', () => {
      climbService.getClimbs.and.returnValue(throwError(() => ({ status: 500 })));
      spyOn(console, 'error');
      
      component.loadClimbs();
      
      expect(console.error).toHaveBeenCalled();
    });

    it('should handle null climbs response', () => {
      climbService.getClimbs.and.returnValue(of(null as any));
      
      component.loadClimbs();
      
      expect(component.climbs).toEqual([]);
    });
  });

  describe('Filtering', () => {
    it('should filter by status', () => {
      component.selectedStatus = 'Top';
      component.onStatusChange();
      
      expect(component.filteredClimbs.length).toBe(1);
      expect(component.filteredClimbs[0].status).toBe('Top');
    });

    it('should filter by grade', () => {
      component.selectedGrade = 'V5';
      component.onGradeChange();
      
      expect(component.filteredClimbs.length).toBe(2);
      expect(component.filteredClimbs.every(c => c.grade === 'V5')).toBeTruthy();
    });

    it('should filter by search term', () => {
      component.searchTerm = 'V7';
      component.onSearchChange();
      
      expect(component.filteredClimbs.length).toBe(1);
      expect(component.filteredClimbs[0].grade).toBe('V7');
    });

    it('should filter by route ID search', () => {
      component.searchTerm = '2';
      component.onSearchChange();
      
      expect(component.filteredClimbs.some(c => c.routeId === 2)).toBeTruthy();
    });

    // it('should combine multiple filters', () => {
    //   component.selectedStatus = 'Top';
    //   component.selectedGrade = 'V5';
    //   component.applyFiltersAndSort();
      
    //   expect(component.filteredClimbs.length).toBe(1);
    //   expect(component.filteredClimbs[0].status).toBe('Top');
    //   expect(component.filteredClimbs[0].grade).toBe('V5');
    // });
  });

  describe('Sorting', () => {
    it('should sort by setDate ascending', () => {
      component.sortBy = 'setDate';
      component.sortDirection = 'asc';
      component.onSortByChange();
      
      expect(component.filteredClimbs[0].setDate).toBe('2025-01-13');
      expect(component.filteredClimbs[2].setDate).toBe('2025-01-15');
    });

    it('should sort by setDate descending', () => {
      component.sortBy = 'setDate';
      component.sortDirection = 'desc';
      component.onSortByChange();
      
      expect(component.filteredClimbs[0].setDate).toBe('2025-01-15');
      expect(component.filteredClimbs[2].setDate).toBe('2025-01-13');
    });

    it('should sort by grade', () => {
      component.sortBy = 'grade';
      component.sortDirection = 'asc';
      component.onSortByChange();
      
      expect(component.filteredClimbs[0].grade).toBe('V5');
      expect(component.filteredClimbs[2].grade).toBe('V7');
    });

    it('should sort by routeId', () => {
      component.sortBy = 'routeId';
      component.sortDirection = 'asc';
      component.onSortByChange();
      
      expect(component.filteredClimbs[0].routeId).toBe(1);
      expect(component.filteredClimbs[2].routeId).toBe(3);
    });

    it('should toggle sort direction', () => {
      component.sortDirection = 'asc';
      component.toggleSortDirection();
      expect(component.sortDirection).toBe('desc');
      
      component.toggleSortDirection();
      expect(component.sortDirection).toBe('asc');
    });
  });

  describe('resetFilters()', () => {
    it('should reset all filters to default values', () => {
      component.selectedStatus = 'Top';
      component.selectedGrade = 'V5';
      component.searchTerm = 'test';
      component.sortBy = 'grade';
      component.sortDirection = 'desc';
      
      component.resetFilters();
      
      expect(component.selectedStatus).toBe('all');
      expect(component.selectedGrade).toBe('all');
      expect(component.searchTerm).toBe('');
      expect(component.sortBy).toBe('setDate');
      expect(component.sortDirection).toBe('asc');
      expect(component.filteredClimbs.length).toBe(3);
    });
  });

  describe('onDeleteClimb()', () => {
    it('should remove climb from list', () => {
      component.onDeleteClimb(1);
      
      expect(component.climbs.length).toBe(2);
      expect(component.climbs.find(c => c.routeId === 1)).toBeUndefined();
      expect(component.filteredClimbs.length).toBe(2);
    });
  });
});
