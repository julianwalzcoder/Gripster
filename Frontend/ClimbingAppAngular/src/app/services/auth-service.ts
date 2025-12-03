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

    register(payload: {
        name?: string;
        username: string;
        mail?: string;
        password: string;
        street?: string;
        streetNumber?: number;
        postcode?: number;
        city?: string;
    }) {
        return this.http.post<{ token: string; username: string; role: string }>(
            `${this.baseUrl}/register`,   
            payload
        ).pipe(
            tap(res => {
                localStorage.setItem(this.TOKEN_KEY, res.token); 
                localStorage.setItem('username', res.username);
                localStorage.setItem('role', res.role);
                this.loggedIn.next(true);                      
            })
        );
    }

    login(username: string, password: string) {
        return this.http.post<{ token: string; username: string; role: string }>(
            `${this.baseUrl}/login`,
            { username, password }
        ).pipe(
            tap(res => {
                localStorage.setItem(this.TOKEN_KEY, res.token);
                localStorage.setItem('username', res.username);
                localStorage.setItem('role', res.role);
                this.loggedIn.next(true);
            })
        );
    }

    logout() {
        localStorage.removeItem(this.TOKEN_KEY);
        localStorage.removeItem('username');
        localStorage.removeItem('role');
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