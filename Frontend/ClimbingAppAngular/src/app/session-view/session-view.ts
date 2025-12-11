import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SessionService, SessionItem, SessionRouteItem, UserSessionRoute } from '../services/session-service';
import { AuthService } from '../services/auth-service';

interface DayView {
  date: string;
  routes: { routeId: number; status?: string | null }[];
}

@Component({
  selector: 'app-session-view',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './session-view.html',
  styleUrl: './session-view.css'
})
export class SessionView {
  days: DayView[] = [];

  constructor(private sessionService: SessionService, private auth: AuthService) {}

  ngOnInit(): void {
    const userId = this.auth.getCurrentUserId();
    if (!userId) return;

    this.loadSessionLogs(userId);
  }

  private loadSessionLogs(userId: number) {
    this.sessionService.getSessionsForUser(userId).subscribe({
      next: (logs: any[]) => {
        console.log('Session logs:', logs);
        // Group logs by date (extract date from loggedAt)
        const byDate = new Map<string, { routeId: number; status?: string | null }[]>();
        logs.forEach(l => {
          // Extract date portion from loggedAt (format: YYYY-MM-DD)
          const date = l.loggedAt ? new Date(l.loggedAt).toISOString().substring(0, 10) : null;
          if (!date) return;
          const arr = byDate.get(date) ?? [];
          arr.push({ routeId: l.routeId, status: l.status });
          byDate.set(date, arr);
        });
        // Build days sorted by date desc
        this.days = Array.from(byDate.entries())
          .sort((a, b) => new Date(b[0]).getTime() - new Date(a[0]).getTime())
          .map(([date, routes]) => ({ date, routes }));
        
        console.log('Processed days:', this.days);
      },
      error: (err: any) => console.error('Failed to load session logs', err)
    });
  }

  private loadFromLogs() {
    console.log('Falling back to user session logs...');
    this.sessionService.getMySessionLogs().subscribe({
      next: (logs: UserSessionRoute[]) => {
        console.log('Session logs:', logs);
        // Group logs by date (YYYY-MM-DD extracted from loggedAt)
        const byDate = new Map<string, { routeId: number; status?: string | null }[]>();
        logs.forEach(l => {
          const date = (l.loggedAt ?? '').substring(0, 10);
          if (!date) return;
          const arr = byDate.get(date) ?? [];
          arr.push({ routeId: l.routeID, status: l.status });
          byDate.set(date, arr);
        });
        // Build days sorted by date desc
        this.days = Array.from(byDate.entries())
          .sort((a, b) => new Date(b[0]).getTime() - new Date(a[0]).getTime())
          .map(([date, routes]) => ({ date, routes }));
      },
      error: (err: any) => console.error('Failed to load session logs', err)
    });
  }

  getStatusClass(status: string | null | undefined): string {
    if (!status) return 'status-default';
    const normalized = status.toLowerCase();
    if (normalized === 'flash') return 'status-flash';
    if (normalized === 'top') return 'status-top';
    if (normalized === 'attempted') return 'status-attempted';
    return 'status-default';
  }

  getStatusDisplay(status: string | null | undefined): string {
    return status ?? 'Not Set';
  }
}