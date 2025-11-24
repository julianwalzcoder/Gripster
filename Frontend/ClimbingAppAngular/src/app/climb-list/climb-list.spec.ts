import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClimbList } from './climb-list';

describe('ClimbList', () => {
  let component: ClimbList;
  let fixture: ComponentFixture<ClimbList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClimbList]
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
