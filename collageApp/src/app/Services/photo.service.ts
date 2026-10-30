import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../api.config';
import { Photo } from '../Models/photo.model';

@Injectable({
  providedIn: 'root'
})
export class PhotoService
{
  private apiUrl = `${API_CONFIG.baseUrl}/photos`

  constructor(private http: HttpClient) {}

  //GET all photo information from bucket, IE scan the bucket
  getAllPhotoInfo(): Observable<Photo[]>
  {
    return this.http.get<Photo[]>(this.apiUrl);
  }

  //get presigned url for photo
  getPresignedUrlForPhoto(photoId: string): Observable<string>
  {
    return this.http.get<string>(`${this.apiUrl}/presigned/${photoId}`);
  }

}
