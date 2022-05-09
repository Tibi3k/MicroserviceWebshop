import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpEvent, HttpResponse, HttpRequest, HttpHandler } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable()
export class AuthorizationInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) {}

  intercept(httpRequest: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // let token = this.authService.getCurrentUserListener().value
    // console.log('token' + token)
    // if(token == null || token?.idToken == null)
    //     return next.handle(httpRequest);
    // let Authorization = token.idToken
    // return next.handle(httpRequest.clone({setHeaders: {'Authorization': `Bearer ${Authorization}` } }))
    return next.handle(httpRequest)
  }
}