import { TestBed } from '@angular/core/testing';
import { ClimbService } from './climb-service';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';

describe('ClimbService', () => {
  let service: ClimbService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(ClimbService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

describe('ClimbService.updateClimbStatus', () => {
  let service: ClimbService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(ClimbService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  it('sends a single POST per call', () => {
    service.updateClimbStatus(1, 42, 'Top').subscribe();
    const reqs = httpMock.match(r => r.method === 'POST' && r.url.includes('/UserRoute/1/42/status/Top'));
    expect(reqs.length).toBe(1);
    reqs[0].flush({});
    httpMock.verify();
  });
});
