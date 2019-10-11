import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {map} from 'rxjs/operators';
import {JwtHelperService} from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = 'http://localhost:5000/api/auth/';
  jwHelper = new JwtHelperService();
  decodedToken: any;

constructor(private http: HttpClient) { }

login(model: any) {     // metodo login (riceviamo il token dal server per poi salvarlo in locale)
  return this.http.post(this.baseUrl + 'login', model)
    .pipe(
      map((resposne: any) => {
        const user = resposne;
        if (user) {
          localStorage.setItem('token', user.token);
          this.decodedToken = this.jwHelper.decodeToken(user.token);
          console.log(this.decodedToken);
        }
      })
    );
}

register(model: any) {   // metodo registrazione
  return this.http.post(this.baseUrl + 'register', model);
}

loggedIn() {
  const token = localStorage.getItem('token');
  return !this.jwHelper.isTokenExpired(token);     // metodo che dice se il token è scaduto oppure non c'è
}

}
