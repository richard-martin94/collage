import { Component, signal } from '@angular/core';
import { HttpClient  } from '@angular/common/http';
import { PhotoService } from '../Services/photo.service';
import { switchMap } from 'rxjs';


@Component({
  selector: 'app-admin',
  standalone: true,
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css'],
})
export class AdminComponent {

  selectedFile = signal<File | null>(null);
  adminKey = signal<string>('');

  constructor(private photoService: PhotoService, private http: HttpClient ) {}

  onPasswordSet(event: Event)
  {
    this.adminKey.set((event.target as HTMLInputElement).value);
  }

  onFileSelected(event: Event){
    const file = (event.target as HTMLInputElement).files?.[0];

    if (file && file?.type == 'image/jpeg')
      this.selectedFile.set(file);
    else
      this.selectedFile.set(null);
  }

  upload(input: HTMLInputElement)
  {
    const file = this.selectedFile();

    if (file) {
      this.photoService
        .getPresignedUrlForPhoto(file.name, 'PUT')
        .pipe(
          switchMap((url) =>
            this.http.put(url, file, { headers: { 'X-Admin-Key': this.adminKey() } }),
          ),
        )
        .subscribe({
          next: () => {
            this.selectedFile.set(null);
            input.value = '';
          },
        });
    }
  }
}
