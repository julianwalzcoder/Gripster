import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { SessionService } from './session-service';

describe('SessionService', () => {
  let service: SessionService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [SessionService]
    });
    service = TestBed.inject(SessionService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch sessions for user', () => {
    const mockUserId = 8;
    const mockSessions = [
      { id: 1, userId: 8, routeId: 15, status: 'Flash', loggedAt: '2025-01-15T10:30:00Z' }
    ];

    service.getSessionsForUser(mockUserId).subscribe(sessions => {
      expect(sessions.length).toBe(1);
      expect((sessions[0] as any).routeId).toBe(15);
    });

    const req = httpMock.expectOne(r => r.url.includes(`/api/session/user/${mockUserId}`));
    expect(req.request.method).toBe('GET');
    req.flush(mockSessions);
  });

  it('should handle error when fetching sessions', () => {
    const mockUserId = 8;

    service.getSessionsForUser(mockUserId).subscribe(
      () => fail('should have failed'),
      error => expect(error.status).toBe(500)
    );

    const req = httpMock.expectOne(r => r.url.includes(`/api/session/user/${mockUserId}`));
    req.flush('Server error', { status: 500, statusText: 'Internal Server Error' });
  });
});
