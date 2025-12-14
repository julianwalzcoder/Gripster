import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MyClimbs } from './my-climbs';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

describe('MyClimbs', () => {
  let component: MyClimbs;
  let fixture: ComponentFixture<MyClimbs>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MyClimbs],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MyClimbs);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
