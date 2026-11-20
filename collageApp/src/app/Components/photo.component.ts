import {
  afterRenderEffect,
  Component,
  DestroyRef,
  ElementRef,
  inject,
  OnInit,
  signal,
  viewChild,
} from '@angular/core';
import { map, delay, forkJoin, Observable, of, firstValueFrom } from 'rxjs';
import { Photo } from '../Models/photo.model';
import { PhotoService } from '../Services/photo.service';
import { NgClass } from '@angular/common';

/*
* observer, loadNextPage, fetchNextPage flow inspired from
* https://www.lightgalleryjs.com/demos/infinite-scrolling/
*/

@Component({
  selector: 'app-photo',
  standalone: true,
  templateUrl: './photo.component.html',
  styleUrls: ['./photo.component.css'],
  imports: [NgClass],
})
export class PhotoComponent implements OnInit {
  photos = signal<Photo[]>([]);
  galleryItems = signal<Photo[]>([]);
  done = signal<boolean>(false);
  scanning = signal<boolean>(false);
  isLoading = signal<boolean>(false);
  pointer = signal<number>(0);

  private status = viewChild.required<ElementRef<HTMLElement>>('status');

  constructor(private photoService: PhotoService) {
    const observer = new IntersectionObserver(
      ([entry]) => {
        if (entry.isIntersecting) {
          void this.loadNextPage();
        }
      },
      { rootMargin: '0% 0% -50px 0%' },
    );

    afterRenderEffect(() => {
      this.scanning();
      observer.disconnect();
      if (!this.done()) {
        observer.observe(this.status().nativeElement);
      }
    });
    inject(DestroyRef).onDestroy(() => observer.disconnect());
  }

  private async loadNextPage() {
    if (this.isLoading()) return;
    if (this.scanning()) return;
    this.isLoading.set(true);
    try {
      const next = await this.fetchNextPage();

      if (next === null) {
        this.done.set(true);
        return;
      }
      this.galleryItems.update((current) => [...current, ...next]);
      this.pointer.set(this.pointer() + 6);
      if (this.pointer() === this.photos().length) {
        this.done.set(true);
      }
    } catch (error) {
      console.error(error);
    } finally {
      this.isLoading.set(false);
    }
  }

  private async fetchNextPage(): Promise<Photo[] | null> {
    if (this.pointer() >= this.photos().length) return null;

    return await firstValueFrom(
      this.getNextPresignedSet(this.photos().slice(this.pointer(), this.pointer() + 6)),
    );
  }

  getNextPresignedSet(photos: Photo[]): Observable<Photo[]> {
    if (photos.length == 0) return of([]);

    const requests = photos.map((photo) =>
      this.photoService.getPresignedUrlForPhoto(photo.id, 'GET').pipe(
        delay(500),
        map((presignedUrl) => ({ ...photo, imageUrl: presignedUrl })),
      ),
    );
    return forkJoin(requests);
  }

  ngOnInit() {
    try {
      this.scanPhotos();
    } catch (error) {
      console.error(error);
    }
  }

  scanPhotos() {
    this.scanning.set(true);
    return this.photoService.getAllPhotoInfo().subscribe({
      next: (photoInfo) => {
        this.photos.set(this.fisherYates(photoInfo));
        this.scanning.set(false);
      },
    });
  }

  fisherYates(photos: Photo[]): Photo[] {
    /*fisherYates shuffling algorithm:
    for i from n − 1 down to 1 do
      j ← random integer such that 0 ≤ j ≤ i
    exchange a[j] and a[i]
    */
    let i = photos.length - 1;
    while (i > 0) {
      let temp = photos[i];
      let j = Math.floor(Math.random() * (i + 1));
      photos[i] = photos[j];
      photos[j] = temp;
      i--;
    }
    return photos;
  }
}
