import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EditClimb } from './edit-climb';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

describe('EditClimb', () => {
  let component: EditClimb;
  let fixture: ComponentFixture<EditClimb>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditClimb],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        {
          provide: ActivatedRoute,
          useValue: {
            params: of({}),
            snapshot: { 
              params: {},
              paramMap: {
                get: (key: string) => null
              }
            }
          }
        }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditClimb);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
