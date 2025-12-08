import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface GymOption {
  id: number;
  name: string;
}

@Injectable({ providedIn: 'root' })
export class GymService {
  private baseUrl = 'http://localhost:5098';
  constructor(private http: HttpClient) {}

  getGyms(): Observable<GymOption[]> {
    return this.http.get<GymOption[]>(`${this.baseUrl}/api/gym`);
  }
}
