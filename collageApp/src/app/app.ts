import { Component, OnInit, signal } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { PhotoComponent } from './Components/photo.component';
import { Photo } from './Models/photo.model';
import { delay, switchMap, forkJoin, map } from 'rxjs';
import { PhotoService } from './Services/photo.service';

@Component({
  imports: [RouterOutlet, RouterLink],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
})
export class App implements OnInit {
  protected readonly title = signal('collageApp');
  photos = signal<Photo[]>([]);

  constructor(private photoService: PhotoService) {}

  ngOnInit() {

  }

  scanPhotos() {
    return this.photoService.getAllPhotoInfo().pipe(
      map((allPhotos) => {
        const luckyPhotos = [...allPhotos].sort(() => 0.5 - Math.random());
        return luckyPhotos.slice(0, 2);
      }),
    );
  }
}
