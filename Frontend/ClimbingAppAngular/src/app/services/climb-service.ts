import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { Climb } from '../model/climb';
@Injectable({
  providedIn: 'root',
})
export class ClimbService {
  baseUrl = 'http://localhost:5098';
  constructor(private http: HttpClient) { }
  getClimbs(): Observable<Climb[]> {
    return this.http.get<Climb[]>(`${this.baseUrl}/climb`);
  }
  getClimb(id: number): Observable<Climb> {
    return this.http.get<Climb>(`${this.baseUrl}/climb/${id}`);
  }
  createClimb(climb: Climb): Observable<any> {
    return this.http.post(`${this.baseUrl}/climb`, climb);
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
