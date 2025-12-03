import { Routes } from '@angular/router';
import { ClimbList } from './climb-list/climb-list';
import { ClimbDetailComponent } from './climb-detail/climb-detail.component';
import { MyClimbs } from './my-climbs/my-climbs';
import { MyProjects } from './my-projects/my-projects';
import { AuthGuard } from './guards/auth.guard';
import { Login } from './login/login';
import { SelectGym } from './select-gym/select-gym';
import { AddClimb } from './add-climb/add-climb';
import { EditClimb } from './edit-climb/edit-climb';
import { AuthGuard } from './guards/auth.guard';
import { AdminGuard } from './guards/admin.guard'; 

export const routes: Routes = [

  // All logged‑in users
  { path: 'login', component: Login },
  { path: 'climbs/:gymId', component: ClimbList, canActivate: [AuthGuard] },
  { path: 'climb-detail/:id', component: ClimbDetailComponent, canActivate: [AuthGuard] },
  { path: 'my-climbs', component: MyClimbs, canActivate: [AuthGuard] },
  { path: 'my-projects', component: MyProjects, canActivate: [AuthGuard] },
  { path: 'select-gym', component: SelectGym, canActivate: [AuthGuard] },

  // Admin only
  { path: 'climb-add', component: AddClimb, canActivate: [AuthGuard, AdminGuard] },
  { path: 'climb-edit', component: EditClimb, canActivate: [AuthGuard, AdminGuard] },

  // Default
  { path: '', redirectTo: '/climbs/1', pathMatch: 'full' },
];
