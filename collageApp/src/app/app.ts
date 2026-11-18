import { Component, OnInit, signal } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { map, switchMap, delay, forkJoin } from 'rxjs'
import { Photo } from './Models/photo.model';
import { PhotoService } from './Services/photo.service';
import { LgGalleryComponent, LgGalleryItemDirective, type LgGalleryItem, } from '@lightgallery/angular';

//try to get all on one page, might not work idk
const firstPage = signal<LgGalleryItem[]>([{}])

declare function fetchNextPage(): Promise<LgGalleryItem[] | null>;

@Component({
  imports: [RouterOutlet, RouterLink],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
})
export class App implements OnInit {
  protected readonly title = signal('collageApp');
  photos = signal<Photo[]>([]);

  constructor(private photoService: PhotoService) {
    const template = `
    <div class="galleryRow">
      @for (photo of photos(); track photo.id) {
        <div class="galleryColumn">
          <img alt="" src="{{ photo.imageUrl }}" />
        </div>
      }
    </div>
    <p #status>{{ done() ? "That's everything." : 'Scroll for more' }}</p>
    `;
  }

  ngOnInit() {}

  getPhotos() {
    this.scanPhotos()
      .pipe(
        delay(1000),
        switchMap((photos) => {
          if (photos.length == 0) return forkJoin([]);

          const requests = photos.map((photo) =>
            this.photoService
              .getPresignedUrlForPhoto(photo.id, 'GET')
              .pipe(map((presignedUrl) => ({ ...photo, imageUrl: presignedUrl }))),
          );
          return forkJoin(requests);
        }),
      )
      .subscribe({
        next: (mappedPhotos) => {
          return this.photos.set(mappedPhotos);
        },
        error: (err) => {
          console.error(err);
        },
      });
  }

  scanPhotos() {
    return this.photoService.getAllPhotoInfo().pipe(
      map((allPhotos) => {
        const luckyPhotos = [...allPhotos].sort(() => 0.5 - Math.random());
        this.photos.set(luckyPhotos);
        return luckyPhotos.slice(0, 6);
      }),
    );
  }
}
