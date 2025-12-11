import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';

export interface GradeOption {
  id: number;
  display: string; // FBleau
  vScale: string;
}

@Injectable({ providedIn: 'root' })
export class GradeService {
  private baseUrl = 'http://localhost:5098';
  constructor(private http: HttpClient) {}

  getGrades(): Observable<GradeOption[]> {
    return this.http
      .get<Array<{ id: number; fBleau?: string; vScale?: string }>>(`${this.baseUrl}/api/grade`)
      .pipe(
        map(rows =>
          (rows ?? []).map(r => ({
            id: r.id,
            display: r.fBleau ?? '',
            vScale: r.vScale ?? ''
          }))
        )
      );
  }
}