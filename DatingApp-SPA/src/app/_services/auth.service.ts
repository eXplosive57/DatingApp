import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {map} from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = 'http://localhost:5000/api/auth/';

constructor(private http: HttpClient) { }

login(model: any) {     // metodo login (riceviamo il token dal server per poi salvarlo in locale)
  return this.http.post(this.baseUrl + 'login', model)
    .pipe(
      map((resposne: any) => {
        const user = resposne;
        if (user) {
          localStorage.setItem('token', user.token);
        }
      })
    );
}

register(model: any) {   // metodo registrazione
  return this.http.post(this.baseUrl + 'register', model);
}

}
