import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, tap } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
    baseUrl = 'http://localhost:5098/api/auth';
    private TOKEN_KEY = 'jwt_token';
    private loggedIn = new BehaviorSubject<boolean>(this.hasToken());

    constructor(private http: HttpClient) { }

    private hasToken(): boolean {
        return !!localStorage.getItem(this.TOKEN_KEY);
    }

    login(username: string, password: string) {
        return this.http.post<any>(`${this.baseUrl}/login`, { username, password })
            .pipe(
                tap(res => {
                    localStorage.setItem(this.TOKEN_KEY, res.token);
                    this.loggedIn.next(true);
                })
            );
    }

    logout() {
        localStorage.removeItem(this.TOKEN_KEY);
        this.loggedIn.next(false);
    }

    isLoggedIn(): boolean {
        return this.loggedIn.value;
    }
    get isLoggedIn$() {
        return this.loggedIn.asObservable();
    }

    getToken(): string | null {
        return localStorage.getItem(this.TOKEN_KEY);
    }
}
