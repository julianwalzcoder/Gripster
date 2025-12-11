export interface Climb {
  routeId: number;
  gymId: number;
  gradeId?: number;
  grade: string | null;          // allow null
  status: string | null;         // allow null
  setDate: string | null;
  removeDate: string | null;
  adminId?: number | null;
  climbId?: number;
}
