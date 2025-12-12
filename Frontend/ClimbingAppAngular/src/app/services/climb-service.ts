import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, forkJoin } from 'rxjs';
import { map } from 'rxjs/operators';
import { Climb } from '../model/climb';
import { AuthService } from './auth-service';

@Injectable({
  providedIn: 'root',
})
export class ClimbService {
  baseUrl = 'http://localhost:5098';
  private authService = inject(AuthService);

  constructor(private http: HttpClient) {}

  getClimbs(): Observable<Climb[]> {
    const currentUserId = this.authService.getCurrentUserId();
    const selectedGymId = localStorage.getItem('selectedGymId');
    const endpoint = selectedGymId
      ? `${this.baseUrl}/usersession/gym/${selectedGymId}`
      : `${this.baseUrl}/usersession`;

    return this.http.get<any[]>(endpoint).pipe(
      map(sessions => {
        const uniqueRoutes = new Map<number, any>();
        sessions.forEach(session => {
          const routeId = session.routeID ?? session.routeid;
          const sessionUserId = session.userID ?? session.userid;
          if (!uniqueRoutes.has(routeId) || sessionUserId === currentUserId) {
            uniqueRoutes.set(routeId, session);
          }
        });

        return Array.from(uniqueRoutes.values()).map(session => ({
          routeId: Number(session.routeID ?? session.routeid),
          gradeId: session.gradeID ?? session.gradeid ?? null, // Climb likely allows null
          grade: session.gradeFbleau ?? session.gradefbleau ?? '',
          status: session.status ?? null,                      // never undefined
          gymId: Number(session.gymID ?? session.gymid ?? 0),  // never undefined
          setDate: session.setDate ?? session.setdate ?? null,
          removeDate: session.removeDate ?? session.removedate ?? null,
          adminId: session.adminID ?? session.adminid ?? null,
          climbId: Number(session.routeID ?? session.routeid)
        } as Climb));
      })
    );
  }

  getAverageRating(routeId: number): Observable<number | null> {
    return this.http.get<number | null>(
      `${this.baseUrl}/api/ClimbingRoute/average-rating/${routeId}`
    );
  }

  getClimb(id: number): Observable<RouteDetails> {
    const session$ = this.http.get<any[]>(`${this.baseUrl}/usersession`);
    const route$ = this.http.get<any>(`${this.baseUrl}/api/route/${id}`);

    return forkJoin({ sessions: session$, route: route$ }).pipe(
      map(({ sessions, route }) => {
        const s = sessions.find(x => (x.routeID ?? x.routeid) === id);
        if (!s) throw new Error('Climb not found');

        return {
          routeId: s.routeID ?? s.routeid,
          gymId: s.gymID ?? s.gymid,
          gradeId: (s.gradeID ?? s.gradeid ?? route?.gradeID ?? route?.GradeID ?? undefined) as number | undefined,
          grade: s.gradeFbleau ?? s.gradefbleau ?? null,
          status: s.status ?? null,
          setDate: route?.setDate ?? route?.SetDate ?? s.setDate ?? s.setdate ?? null,
          removeDate: route?.removeDate ?? route?.RemoveDate ?? s.removeDate ?? s.removedate ?? null,
          adminId: s.adminID ?? s.adminid ?? null,
          climbId: s.routeID ?? s.routeid
        } as RouteDetails;
      })
    );
  }

  // ADMIN: create new climb (route)
  addClimb(payload: AddClimbRequest) {
    return this.http.post<void>(`${this.baseUrl}/api/route`, payload);
  }

  // ADMIN: edit existing climb (route data)
  updateClimbAdmin(dto: {
    id: number;
    gymID: number;
    gradeID: number;
    setDate: string | null;
    removeDate: string | null;
    adminID: number;
  }) {
    return this.http.put<void>(`${this.baseUrl}/api/route/${dto.id}`, dto);
  }
  
  // ADMIN: delete climb
  deleteClimb(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/climb/${id}`);
  }

  // USER: update personal status on a climb
  updateClimbStatus(userID: number, routeID: number, status: string): Observable<any> {
    console.log('[updateClimbStatus] POST', userID, routeID, status);
    return this.http.post(
      `${this.baseUrl}/UserRoute/${userID}/${routeID}/status/${encodeURIComponent(status)}`,
      {}
    );
  }

  // USER: rate a climb
  setRating(userId: number, routeId: number, rating: number | null): Observable<any> {
    return this.http.post(`${this.baseUrl}/UserRoute/${userId}/${routeId}/rating`, rating);
  }

  getUserRating(userId: number, routeId: number): Observable<number | null> {
    return this.http.get<number | null>(
      `${this.baseUrl}/UserRoute/${userId}/${routeId}/rating`
    );
  }
  getGymID() {
    return localStorage.getItem('selectedGymId');
  }
}

// Rename local interface to avoid conflict with imported Climb
export interface RouteDetails {
  routeId: number;
  gradeId?: number;
  grade?: string | null;
  status?: string | null;
  gymId?: number;
  setDate?: string | null;
  removeDate?: string | null;
  adminId?: number;
  climbId: number;
  userId?: number; // add optional to satisfy templates
}

export interface AddClimbRequest {
  routeId: number;
  gymId: number;
  gradeId: number;
  setDate: string | null;
  removeDate: string | null;
  status?: string;
}

