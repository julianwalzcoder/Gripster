import { Routes } from '@angular/router';
import { ClimbList } from './climb-list/climb-list';
import { EditClimb } from './edit-climb/edit-climb';
export const routes: Routes = [
    {path: 'climbs', component: ClimbList},
    {path: 'edit-climb/:id', component: EditClimb}
];
