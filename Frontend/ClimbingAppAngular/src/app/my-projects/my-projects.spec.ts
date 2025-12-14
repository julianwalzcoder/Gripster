import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MyProjects } from './my-projects';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

describe('MyProjects', () => {
  let component: MyProjects;
  let fixture: ComponentFixture<MyProjects>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MyProjects],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MyProjects);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
