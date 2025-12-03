import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Climb } from '../model/climb';

@Injectable({
  providedIn: 'root',
})
export class ClimbService {
  baseUrl = 'http://localhost:5098';
  constructor(private http: HttpClient) { }

  getClimbs(): Observable<Climb[]> {
    return this.http.get<any[]>(`${this.baseUrl}/usersession`).pipe(
      map(sessions => {
        console.log('Raw sessions from API:', sessions);
        return sessions.map(session => {
          console.log('Mapping session:', session);
          return {
            userId: session.userID ?? session.userid,
            routeId: session.routeID ?? session.routeid,
            grade: session.gradeFbleau ?? session.gradefbleau,
            status: session.status,
            gymId: session.gymID ?? session.gymid,
            setDate: session.setDate ?? session.setdate,
            removeDate: session.removeDate ?? session.removedate,
            adminId: session.adminID ?? session.adminid,
            climbId: session.routeID ?? session.routeid
          } as Climb;
        });
      })
    );
  }

  getAverageRating(routeId: number): Observable<number | null> {
    return this.http.get<number | null>(
      `${this.baseUrl}/api/ClimbingRoute/average-rating/${routeId}`
    );
  }

  getClimb(id: number): Observable<Climb> {
    return this.http.get<any[]>(`${this.baseUrl}/usersession`).pipe(
      map(sessions => {
        console.log('All sessions:', sessions);
        const session = sessions.find(s => (s.routeID ?? s.routeid) === id);
        if (!session) {
          throw new Error('Climb not found');
        }
        console.log('Found session:', session);
        return {
          userId: session.userID ?? session.userid,
          routeId: session.routeID ?? session.routeid,
          grade: session.gradeFbleau ?? session.gradefbleau,
          status: session.status,
          gymId: session.gymID ?? session.gymid,
          setDate: session.setDate ?? session.setdate,
          removeDate: session.removeDate ?? session.removedate,
          adminId: session.adminID ?? session.adminid,
          climbId: session.routeID ?? session.routeid
        };
      })
    );
  }

  // ADMIN: create new climb (route)
  addClimb(climb: Climb): Observable<any> {
    return this.http.post(`${this.baseUrl}/climb`, climb); // POST /climb
  }

  // ADMIN: edit existing climb (route data)
  updateClimbAdmin(climb: Climb): Observable<any> {
    const id = climb.climbId ?? climb.routeId;
    return this.http.put(`${this.baseUrl}/climb/${id}`, climb); // PUT /climb/{id}
  }

  // ADMIN: delete climb
  deleteClimb(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/climb/${id}`);
  }

  // USER: update personal status on a climb
  updateClimbStatus(userID: number, routeID: number, status: string): Observable<any> {
    return this.http.post(
      `${this.baseUrl}/UserRoute/${userID}/${routeID}/${status}`,
      {}
    );
  }

  // USER: rate a climb
  setRating(userId: number, routeId: number, rating: number | null): Observable<void> {
    return this.http.post<void>(
      `${this.baseUrl}/UserRoute/${userId}/${routeId}/rating`,
      { rating },
      { headers: { 'Content-Type': 'application/json' } }
    );
  }

  getUserRating(userId: number, routeId: number): Observable<number | null> {
    return this.http.get<number | null>(
      `${this.baseUrl}/UserRoute/${userId}/${routeId}/rating`
    );
  }
}
