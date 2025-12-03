// export interface Climb {
//     id: number;
//   gymID: number;
//   gradeID: number;
//   setDate: string;
//   removeDate?: string;
//   adminID: number;
// }
export interface Climb {
  userId?: number;
  routeId: number;
  grade: string;
  status: string;
  gymId: number;
  setDate: Date;
  removeDate?: Date;
  adminId: number;
  
  // Alias for compatibility
  climbId?: number;  // Will be routeId
}
