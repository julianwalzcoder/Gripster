import { SessionView } from './session-view';

describe('SessionView grouping', () => {
  it('keeps latest per session per local day', () => {
    // mimic internal logic
    const dayKeyLocal = (d: Date) => `${d.getFullYear()}-${String(d.getMonth()+1).padStart(2,'0')}-${String(d.getDate()).padStart(2,'0')}`;
    const logs = [
      { sessionID: 100, routeID: 27, status: 'Attempted', LoggedAt: '2025-12-12T08:00:00+01:00' },
      { sessionID: 100, routeID: 27, status: 'Top',       LoggedAt: '2025-12-12T20:00:00+01:00' },
      { sessionID: 100, routeID: 28, status: 'Flash',     LoggedAt: '2025-12-12T21:00:00+01:00' }, // same session/day, later
      { sessionID: 100, routeID: 27, status: 'Attempted', LoggedAt: '2025-12-13T09:00:00+01:00' }  // next day
    ] as any[];

    const byDate = new Map<string, Map<number, { sessionId: number; routeId: number; status: string | null; loggedAtMs: number }>>();
    logs.forEach(l => {
      const sessionId = l.sessionID;
      const routeId = l.routeID;
      const loggedAt = new Date(l.LoggedAt);
      const day = dayKeyLocal(loggedAt);
      const perSession = byDate.get(day) ?? new Map<number, any>();
      const prev = perSession.get(sessionId);
      const currMs = loggedAt.getTime();
      if (!prev || currMs >= prev.loggedAtMs) {
        perSession.set(sessionId, { sessionId, routeId, status: l.status ?? null, loggedAtMs: currMs });
      }
      byDate.set(day, perSession);
    });

    const day12 = byDate.get('2025-12-12');
    const day13 = byDate.get('2025-12-13');
    expect(day12?.get(100)?.status).toBe('Flash'); // latest of the day for session 100
    expect(day12?.get(100)?.routeId).toBe(28);
    expect(day13?.get(100)?.status).toBe('Attempted');
  });
});