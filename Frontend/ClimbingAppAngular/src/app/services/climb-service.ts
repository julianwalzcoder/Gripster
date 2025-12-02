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
          };
        });
      })
    );
  }

  getAverageRating(routeId: number): Observable<number | null> {
    return this.http.get<number | null>('http://localhost:5098/api/ClimbingRoute/average-rating/' + routeId);
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

  createClimb(climb: Climb): Observable<any> {
    return this.http.post(`${this.baseUrl}/climb`, climb);
  }

  updateClimbStatus(userID: number, routeID: number, status: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/UserRoute/${userID}/${routeID}/${status}`, {});
  }

  updateClimb(climb: Climb): Observable<any> {
    return this.http.put(`${this.baseUrl}/climb`, climb);
  }

  deleteClimb(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/climb/${id}`);
  }

  addClimb(climb: Climb): Observable<any> {
    return this.http.post(`${this.baseUrl}/climb`, climb);
  }
}
