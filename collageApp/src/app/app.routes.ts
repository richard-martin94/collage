import { Routes } from '@angular/router';
import { AdminComponent } from './Components/admin.component';
//component import here
import { appConfig } from './app.config';

export const routes: Routes = [
  {path: 'admin', component: AdminComponent }
];
