import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ClimbCard } from './climb-card';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

describe('ClimbCard', () => {
  let component: ClimbCard;
  let fixture: ComponentFixture<ClimbCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClimbCard],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClimbCard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
