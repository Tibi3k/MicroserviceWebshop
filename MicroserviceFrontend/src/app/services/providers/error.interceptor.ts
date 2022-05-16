import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { catchError, Observable, retry, throwError } from "rxjs";

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  intercept(httpRequest: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(httpRequest)
        .pipe(
            catchError((error: HttpErrorResponse) => {
                console.log('error', error)
                
                let errorMsg = 'Something went wrong, please try again later'
                if(error.error instanceof ErrorEvent){
                    let errorMsg = 'Error: service is currently unavailable!';
                    return throwError(() => errorMsg)
                } else {
                    console.log(error)
                    errorMsg = 'Something went wrong, please try again later'
                }
                return throwError(() => errorMsg)
            })
        )
  }
}