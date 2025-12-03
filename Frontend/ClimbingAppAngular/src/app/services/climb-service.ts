import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Climb } from '../model/climb';
import { AuthService } from './auth-service';

@Injectable({
  providedIn: 'root',
})
export class ClimbService {
  baseUrl = 'http://localhost:5098';
  private authService = inject(AuthService);
  
  constructor(private http: HttpClient) { }

  getClimbs(): Observable<Climb[]> {
    const currentUserId = this.authService.getCurrentUserId();
    const selectedGymId = localStorage.getItem('selectedGymId');
    
    // If gym is selected, fetch sessions filtered by gym; otherwise fetch all
    const endpoint = selectedGymId 
      ? `${this.baseUrl}/usersession/gym/${selectedGymId}`
      : `${this.baseUrl}/usersession`;
    
    return this.http.get<any[]>(endpoint).pipe(
      map(sessions => {
        console.log('Raw sessions from API:', sessions);
        
        // Create a map to store unique routes by routeId
        const uniqueRoutes = new Map<number, any>();
        
        sessions.forEach(session => {
          const routeId = session.routeID ?? session.routeid;
          const sessionUserId = session.userID ?? session.userid;
          
          // If we haven't seen this route yet, or if this is the current user's session, add/update it
          if (!uniqueRoutes.has(routeId) || sessionUserId === currentUserId) {
            uniqueRoutes.set(routeId, session);
          }
        });
        
        // Convert map values back to array and map to Climb objects
        return Array.from(uniqueRoutes.values()).map(session => {
          console.log('Mapping session:', session);
          return {
            userId: currentUserId, // Always use the current logged-in user's ID
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
  setRating(userId: number, routeId: number, rating: number | null): Observable<any> {
    return this.http.post(`${this.baseUrl}/UserRoute/${userId}/${routeId}/rating`, rating);
  }

  getUserRating(userId: number, routeId: number): Observable<number | null> {
    return this.http.get<number | null>(
      `${this.baseUrl}/UserRoute/${userId}/${routeId}/rating`
    );
  }
  getGymID(){
      return localStorage.getItem('selectedGymId');
  }
}

