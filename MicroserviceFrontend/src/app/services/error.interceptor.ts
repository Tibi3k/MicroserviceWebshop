import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { catchError, Observable, retry, throwError } from "rxjs";

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  intercept(httpRequest: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(httpRequest)
        .pipe(
            catchError((error: HttpErrorResponse) => {
                let errorMsg = '';
                if(error.error instanceof ErrorEvent)
                    errorMsg = `Error: ${error.error.message}!`;
                if(error.status == 500)
                    errorMsg = 'Something went wrong!'
                return throwError(() => errorMsg)
            })
        )
  }
}