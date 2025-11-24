import { TestBed } from '@angular/core/testing';

import { ClimbService } from './climb-service';

describe('ClimbService', () => {
  let service: ClimbService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ClimbService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
