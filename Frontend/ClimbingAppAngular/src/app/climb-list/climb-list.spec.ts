import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ClimbList } from './climb-list';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

describe('ClimbList', () => {
  let component: ClimbList;
  let fixture: ComponentFixture<ClimbList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClimbList],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClimbList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
