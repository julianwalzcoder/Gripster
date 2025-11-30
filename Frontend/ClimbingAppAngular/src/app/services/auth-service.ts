import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
    baseUrl = 'http://localhost:5098/api/auth';
    private TOKEN_KEY = 'jwt_token';

    constructor(private http: HttpClient) { }

    login(username: string, password: string) {
        return this.http.post<any>(`${this.baseUrl}/login`, { username, password })
            .pipe(
                tap(res => {
                    localStorage.setItem(this.TOKEN_KEY, res.token);
                })
            );
    }

    logout() {
        localStorage.removeItem(this.TOKEN_KEY);
    }

    isLoggedIn(): boolean {
        return !!localStorage.getItem(this.TOKEN_KEY);
    }

    getToken(): string | null {
        return localStorage.getItem(this.TOKEN_KEY);
    }
}
