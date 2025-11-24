import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditClimb } from './edit-climb';

describe('EditClimb', () => {
  let component: EditClimb;
  let fixture: ComponentFixture<EditClimb>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditClimb]
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
