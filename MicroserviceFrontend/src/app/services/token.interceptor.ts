import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpEvent, HttpResponse, HttpRequest, HttpHandler } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class AuthorizationInterceptor implements HttpInterceptor {
  intercept(httpRequest: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    let Authorization = localStorage.getItem("JWT")
    if(Authorization == null)
        return next.handle(httpRequest);
    return next.handle(httpRequest.clone({setHeaders: { Authorization } }))
  }
}