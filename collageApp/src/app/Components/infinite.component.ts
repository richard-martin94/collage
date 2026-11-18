import {
  LgGalleryComponent,
  LgGalleryItemDirective,
  type LgGalleryItem,
} from '@lightgallery/angular';
import { Component, OnInit, signal } from '@angular/core';
import { Photo } from '../Models/photo.model';
import { delay, forkJoin, map, switchMap } from 'rxjs';
import { PhotoService } from '../Services/photo.service';

@Component({
  selector: 'app-infinite',
  template: `
    <div class="galleryRow">
      @for (photo of photos(); track photo.id) {
        <div class="galleryColumn">
          <img alt="" src="{{ photo.imageUrl }}" />
        </div>
      }
    </div>
  `,
})
export class InfiniteComponent {
  photos = signal<Photo[]>([]);

  constructor(private photoService: PhotoService) {
  }

}
