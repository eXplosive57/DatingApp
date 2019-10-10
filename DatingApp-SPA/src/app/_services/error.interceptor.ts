import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpErrorResponse, HTTP_INTERCEPTORS } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

@Injectable()
// tslint:disable-next-line: max-line-length
export class ErrorInterceptor implements HttpInterceptor {    // questa funzione traduce gli headers dalla console per darci una risposta semplice dell'errore

  intercept(
    req: import('@angular/common/http').HttpRequest<any>,
    next: import('@angular/common/http').HttpHandler
  ): import('rxjs').Observable<import('@angular/common/http').HttpEvent<any>> {
    return next.handle(req).pipe(
        catchError(error => {
            if (error.status === 401) {
                return throwError(error.statusText);
            }
            if (error instanceof HttpErrorResponse) {
                const applicationError = error.headers.get('Application-Error'); // prendo l'errore dagli header
                if (applicationError) {
                    return throwError(applicationError); // si applica agli errori 500
                }
                const serverError = error.error;
                let modalStateErrors = '';
                if (serverError.errors && typeof serverError.errors === 'object') {
                    for (const key in serverError.errors) {
                        if (serverError.errors[key]) {
                        modalStateErrors += serverError.errors[key] + '\n';
                    }
                }
            }
                return throwError(modalStateErrors || serverError || 'Server Error'); // restituisce l'errore in riga semplificata
        }
    })
    );
  }
}

export const ErrorInterceptorProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass: ErrorInterceptor,
    multi: true
};
