import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SessionService, SessionItem, SessionRouteItem, UserSessionRoute } from '../services/session-service';
import { AuthService } from '../services/auth-service';

interface DayView {
  date: string;
  routes: { routeId: number; status: string | null }[];
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

  private dayKeyLocal(d: Date): string {
    const y = d.getFullYear();
    const m = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return `${y}-${m}-${day}`;
  }

  private loadSessionLogs(userId: number) {
    this.sessionService.getSessionsForUser(userId).subscribe({
      next: (logs: any[]) => {
        console.log('[SV] raw logs sample:', logs.slice(0, 5));
        const byDate = new Map<string, Map<number, { routeId: number; status: string | null; loggedAtMs: number; gymId?: number }>>();

        logs
          .map(l => {
            const rawRouteId = l.routeID ?? l.routeId;
            const routeId = rawRouteId != null ? Number(rawRouteId) : undefined;
            const status: string | null = l.status ?? null;
            const gymId: number | undefined = l.gymID ?? l.gymId;
            const tsStr: string | undefined = l.loggedAt ?? l.LoggedAt ?? l.updatedAt ?? l.UpdatedAt ?? l.timestamp;
            const ts = tsStr ? new Date(tsStr) : undefined;
            return { routeId, status, ts, gymId };
          })
          .forEach(x => {
            if (typeof x.routeId !== 'number' || !x.ts || isNaN(x.ts.getTime())) {
              console.warn('[SV] skip invalid', x);
              return;
            }
          });

        // Log key distribution before grouping
        logs.forEach(l => {
          const r = l.routeID ?? l.routeId;
          const tsStr = l.loggedAt ?? l.LoggedAt ?? l.updatedAt ?? l.UpdatedAt ?? l.timestamp;
          const ts = tsStr ? new Date(tsStr) : null;
          const day = ts ? this.dayKeyLocal(ts) : 'invalid';
          console.debug('[SV] key', { routeIdRaw: r, routeIdNum: r != null ? Number(r) : null, tsStr, day });
        });

        // Group: latest per (routeId, local day)
        logs
          .map(l => {
            const routeId = l.routeID != null ? Number(l.routeID) : (l.routeId != null ? Number(l.routeId) : undefined);
            const status: string | null = l.status ?? null;
            const tsStr: string | undefined = l.loggedAt ?? l.LoggedAt ?? l.updatedAt ?? l.UpdatedAt ?? l.timestamp;
            const ts = tsStr ? new Date(tsStr) : undefined;
            return { routeId, status, ts };
          })
          .filter(x => typeof x.routeId === 'number' && x.ts && !isNaN(x.ts.getTime()))
          .sort((a, b) => a.ts!.getTime() - b.ts!.getTime())
          .forEach(x => {
            const day = this.dayKeyLocal(x.ts!);
            const perRoute = byDate.get(day) ?? new Map<number, { routeId: number; status: string | null; loggedAtMs: number }>();
            const prev = perRoute.get(x.routeId!);
            const currMs = x.ts!.getTime();
            if (!prev || currMs >= prev.loggedAtMs) {
              perRoute.set(x.routeId!, { routeId: x.routeId!, status: x.status, loggedAtMs: currMs });
            }
            byDate.set(day, perRoute);
          });

        this.days = Array.from(byDate.entries())
          .sort((a, b) => new Date(b[0]).getTime() - new Date(a[0]).getTime())
          .map(([date, perRoute]) => ({
            date,
            routes: Array.from(perRoute.values()).map(r => ({ routeId: r.routeId, status: r.status }))
          }));

        console.log('[SV] days summary:', this.days.map(d => ({ date: d.date, routes: d.routes.length })));
        console.table(this.days.flatMap(d => d.routes.map(r => ({ date: d.date, routeId: r.routeId, status: r.status }))));

        // Sanity check: count per routeId per day from raw logs
        const counts = new Map<string, number>();
        logs.forEach(l => {
          const day = this.dayKeyLocal(new Date(l.loggedAt ?? l.LoggedAt ?? l.updatedAt ?? l.UpdatedAt ?? l.timestamp));
          const rid = Number(l.routeID ?? l.routeId);
          const key = `${day}:${rid}`;
          counts.set(key, (counts.get(key) ?? 0) + 1);
        });
        console.table(Array.from(counts.entries()).map(([key, count]) => ({ key, count })));
      },
      error: (err: any) => console.error('Failed to load session logs', err)
    });
  }

  private loadFromLogs() {
    this.sessionService.getMySessionLogs().subscribe({
      next: (logs: UserSessionRoute[]) => {
        const byDate = new Map<string, Map<number, { routeId: number; status: string | null }>>();

        (logs as any[])
          .map(l => {
            const rawRouteId = l.routeID ?? l.routeId;
            const routeId = rawRouteId != null ? Number(rawRouteId) : undefined; // force number
            const status: string | null = l.status ?? null;
            const tsStr: string | undefined = l.loggedAt ?? l.LoggedAt ?? l.updatedAt ?? l.UpdatedAt;
            const ts = tsStr ? new Date(tsStr) : undefined;
            return { routeId, status, ts };
          })
          .filter(x => typeof x.routeId === 'number' && x.ts && !isNaN(x.ts.getTime()))
          .sort((a, b) => a.ts!.getTime() - b.ts!.getTime())
          .forEach(x => {
            const day = this.dayKeyLocal(x.ts!);
            const perRoute = byDate.get(day) ?? new Map<number, { routeId: number; status: string | null }>();
            perRoute.set(x.routeId!, { routeId: x.routeId!, status: x.status });
            byDate.set(day, perRoute);
          });

        this.days = Array.from(byDate.entries())
          .sort((a, b) => new Date(b[0]).getTime() - new Date(a[0]).getTime())
          .map(([date, perRoute]) => ({
            date,
            routes: Array.from(perRoute.values())
          }));
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