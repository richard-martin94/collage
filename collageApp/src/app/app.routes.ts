import { Routes } from '@angular/router';
import { AdminComponent } from './Components/admin.component';
import { PhotoComponent } from './Components/photo.component';

export const routes: Routes = [
  { path: '', component: PhotoComponent },
  { path: 'supercalifragilisticexpialidocious', component: AdminComponent },
];
