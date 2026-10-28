import { Routes } from '@angular/router';
import { PhotoComponent } from './Components/photo.component';
//component import here
import { appConfig } from './app.config';

export const routes: Routes = [
  {path: 'photos', component: PhotoComponent}
];
