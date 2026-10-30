import { Component, OnInit, signal } from '@angular/core';
import { Photo } from '../Models/photo.model';
import { delay, switchMap, forkJoin, map } from 'rxjs';
import { PhotoService } from '../Services/photo.service';


@Component({
  selector: 'app-photo',
  standalone: true,
  templateUrl: './photo.component.html',
  styleUrls: ['./photo.component.css'],
})
export class PhotoComponent implements OnInit {

  photos = signal<Photo[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');

  constructor(private photoService: PhotoService) {}

  ngOnInit() {
    this.getPhotos();
  }

  getPhotos()
  {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.scanPhotos().pipe(delay(1000),
        switchMap((photos) => {
          if(photos.length == 0 ) return forkJoin([]);

          const requests = photos.map(photo =>
            this.photoService.getPresignedUrlForPhoto(photo.id).pipe(
              map(presignedUrl => ({...photo, imageUrl: presignedUrl }))
            ));
          return forkJoin(requests);
        })
      ).subscribe({
        next: (mappedPhotos) => {
          this.photos.set(mappedPhotos);
          this.isLoading.set(false);
        },
        error: (err) => {
          this.errorMessage.set(
            'Failed to load bucket scan info see error for more details: ' + err.message,
          );
          this.isLoading.set(false);
        },
      });
  }

  scanPhotos()
  {
    this.isLoading.set(true);
    this.errorMessage.set('');

    return this.photoService.getAllPhotoInfo().pipe(
      map((allPhotos) => {
        const luckyPhotos = [...allPhotos].sort(()=> 0.5 - Math.random());
        return luckyPhotos.slice(0, 2);
      })
    );
  }
}
