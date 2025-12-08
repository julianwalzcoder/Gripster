import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface SessionItem {
  id: number;
  date: string; // ISO date
  name?: string | null;
  feedback?: string | null;
}

export interface SessionRouteItem {
  sessionId: number;
  routeId: number;
  tries?: number | null;
  status?: string | null;
}

@Injectable({ providedIn: 'root' })
export class SessionService {
  private baseUrl = 'http://localhost:5098';

  constructor(private http: HttpClient) {}

  getSessionsForUser(userId: number): Observable<SessionItem[]> {
    return this.http.get<SessionItem[]>(`${this.baseUrl}/api/session/user/${userId}`);
  }

  createSession(payload: { userId: number; date: string; name?: string | null; feedback?: string | null }): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(`${this.baseUrl}/api/session`, payload);
  }

  getSessionRoutes(sessionId: number): Observable<SessionRouteItem[]> {
    return this.http.get<SessionRouteItem[]>(`${this.baseUrl}/api/session/${sessionId}/routes`);
  }

  upsertSessionRoute(sessionId: number, payload: { userId: number; routeId: number; tries?: number | null; status?: string | null }): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/api/session/${sessionId}/routes`, payload);
  }
}
