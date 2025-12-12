import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

export interface GymOption {
  id: number;
  name: string;
}

@Injectable({ providedIn: 'root' })
export class GymService {
  private readonly selectedGymId$ = new BehaviorSubject<number | null>(null);
  private readonly selectedGymName$ = new BehaviorSubject<string>('All Gyms');

  constructor(private http: HttpClient) {
    const savedId = localStorage.getItem('selectedGymId');
    const savedName = localStorage.getItem('selectedGymName');

    if (savedId) {
      const id = Number(savedId);
      this.selectedGymId$.next(id);
      if (savedName) {
        this.selectedGymName$.next(savedName);
      } else {
        // No name stored -> fetch from API and update
        this.getGym(id).subscribe({
          next: (g) => {
            if (g?.name) {
              this.selectedGymName$.next(g.name);
              localStorage.setItem('selectedGymName', g.name);
            }
          },
          error: () => { /* ignore, keep default 'All Gyms' */ }
        });
      }
    }
  }

  setSelectedGym(id: number | null, name: string) {
    this.selectedGymId$.next(id);
    this.selectedGymName$.next(name);
    if (id != null) {
      localStorage.setItem('selectedGymId', String(id));
      localStorage.setItem('selectedGymName', name);
    } else {
      localStorage.removeItem('selectedGymId');
      localStorage.removeItem('selectedGymName');
    }
  }

  getSelectedGymId$() { return this.selectedGymId$.asObservable(); }
  getSelectedGymName$() { return this.selectedGymName$.asObservable(); }
  getSelectedGymIdValue() { return this.selectedGymId$.value; }

  private baseUrl = 'http://localhost:5098';
  getGyms(): Observable<GymOption[]> { return this.http.get<GymOption[]>(`${this.baseUrl}/api/gym`); }
  getGym(id: number): Observable<GymOption> { return this.http.get<GymOption>(`${this.baseUrl}/api/gym/${id}`); }
}
