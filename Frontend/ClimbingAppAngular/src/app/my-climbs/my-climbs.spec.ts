import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MyClimbs } from './my-climbs';

describe('MyClimbs', () => {
  let component: MyClimbs;
  let fixture: ComponentFixture<MyClimbs>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MyClimbs]
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
