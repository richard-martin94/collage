import { Component, OnInit, signal } from '@angular/core';
import { Photo } from '../Models/photo.model';
import { delay } from 'rxjs';
import { PhotoService } from '../Services/photo.service';
import { API_CONFIG } from '../api.config';


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
  private apiUrl = <string>(API_CONFIG.baseUrl);

  constructor(private photoService: PhotoService) {}

  ngOnInit() {
    this.scanPhotos();
  }

  scanPhotos()
  {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.photoService
      .getAllPhotoInfo()
      .pipe(delay(1000))
      .subscribe({
        next: (data) => {
          const mappedPhotos = data.map(photo => ({
            ...photo,
            imageUrl: `${this.apiUrl}/photos/${photo.id}`
          }));
          this.photos.set(mappedPhotos);
          this.isLoading.set(false);
        },
        error: (err) => {
          this.errorMessage.set('Failed to load bucket scan info see error for more details: ' + err.message);
          this.isLoading.set(false);
        },
      });
  }
}
