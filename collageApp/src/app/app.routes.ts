import { Routes } from '@angular/router';
import { PhotoComponent } from './Components/photo.component';
import { AdminComponent } from './Components/admin.component';
//component import here
import { appConfig } from './app.config';

export const routes: Routes = [
  {path: 'photos', component: PhotoComponent},
  {path: 'admin', component: AdminComponent }
];
