import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EditClimbStatus } from './edit-climb-status';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

describe('EditClimbStatus', () => {
  let component: EditClimbStatus;
  let fixture: ComponentFixture<EditClimbStatus>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditClimbStatus],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditClimbStatus);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
