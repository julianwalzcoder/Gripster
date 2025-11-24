import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClimbCard } from './climb-card';

describe('ClimbCard', () => {
  let component: ClimbCard;
  let fixture: ComponentFixture<ClimbCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClimbCard]
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
