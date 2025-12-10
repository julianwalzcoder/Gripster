import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SessionService, SessionItem, SessionRouteItem, UserSessionRoute } from '../services/session-service';
import { AuthService } from '../services/auth-service';

interface DayView {
  date: string;
  routes: { routeId: number; status?: string | null }[];
}

@Component({
  selector: 'app-session-view',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './session-view.html',
  styleUrl: './session-view.css'
})
export class SessionView {
  days: DayView[] = [];

  constructor(private sessionService: SessionService, private auth: AuthService) {}

  ngOnInit(): void {
    const userId = this.auth.getCurrentUserId();
    if (!userId) return;

    this.sessionService.getSessionsForUser(userId).subscribe({
      next: (sessions: SessionItem[]) => {
        console.log('Sessions:', sessions);
        // Initialize days from sessions
        this.days = sessions
          .sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
          .map(s => ({ date: s.date, routes: [] }));

        let anyRoutes = false;

        // For each session, load its routes and attach to the corresponding day
        sessions.forEach(s => {
          this.sessionService.getSessionRoutes(s.id).subscribe({
            next: (routes: SessionRouteItem[]) => {
              console.log(`Routes for session ${s.id}:`, routes);
              const day = this.days.find(d => d.date === s.date);
              if (!day) return;
              const mapped = routes.map(r => ({ routeId: r.routeId, status: r.status ?? null }));
              day.routes = mapped;
              if (mapped.length > 0) anyRoutes = true;
            },
            error: (err: any) => console.warn(`Failed to load routes for session ${s.id}`, err)
          });
        });

        // After a short delay, if no routes were attached, fallback to session logs
        setTimeout(() => {
          if (!anyRoutes) {
            this.loadFromLogs();
          }
        }, 300);
      },
      error: (err: any) => console.error('Failed to load sessions', err)
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
}