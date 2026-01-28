import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { ServiceResponse } from '../../models/response/service-response';
import { environment } from '../../../environment/environment';

@Injectable({
  providedIn: 'root'
})
export class HttpServiceService {

  constructor(private http: HttpClient) { }


  /* SERVICE LOGIN  */

  get<T>(url: string): Observable<ServiceResponse<T>> {
    return this.http.get<ServiceResponse<T>>(`${environment.endPoint}/${url}`)
      .pipe(
        catchError(error => {
          console.error(`Erro ao executar GET em ${url}`, error);
          return throwError(() => error);
        })
      );
  }

    post<T>(url: string, body: any): Observable<T> {
    return this.http.post<T>(`${environment.endPoint}/${url}`, body, { responseType: 'json' })
      .pipe(
        catchError(error => {
          console.error(`Erro ao executar POST em ${url}`, error);
          return throwError(() => error);
        })
      );
  }

    put<T>(url: string, body: any): Observable<T> {
    return this.http.put<T>(`${environment.endPoint}/${url}`, body, { responseType: 'json' })
      .pipe(
        catchError(error => {
          console.error(`Erro ao executar PUT em ${url}`, error);
          return throwError(() => error);
        })
      );
  }

  delete<T>(url: string): Observable<ServiceResponse<T>> {
    return this.http.delete<ServiceResponse<T>>(`${environment.endPoint}/${url}`)
      .pipe(
        catchError(error => {
          console.error(`Erro ao executar DELETE em ${url}`, error);
          return throwError(() => error);
        })
      );
  }

}
