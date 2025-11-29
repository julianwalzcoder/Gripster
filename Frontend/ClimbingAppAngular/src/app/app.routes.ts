import { Routes } from '@angular/router';
import { ClimbList } from './climb-list/climb-list';
import { ClimbDetailComponent } from './climb-detail/climb-detail.component';
import { MyClimbs } from './my-climbs/my-climbs';

export const routes: Routes = [
  { path: '', redirectTo: '/climbs', pathMatch: 'full' },
  { path: 'climbs', component: ClimbList },
  { path: 'climb-detail/:id', component: ClimbDetailComponent },
  { path: 'my-climbs', component: MyClimbs }
];
