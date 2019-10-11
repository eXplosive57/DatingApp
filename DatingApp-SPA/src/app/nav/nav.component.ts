import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {}; // memorizza nome utente e password che passiamo

  constructor(public authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  login() {
    this.authService.login(this.model).subscribe(next => { // specificando next, dicamo che ho effettuato il login con successo
      this.alertify.success('Loggato con successo');
    }, error => {
      this.alertify.error(error);
    });
  }

  loggedIn() { // metodo che dice se siamo loggati
    return this.authService.loggedIn();
  }

  logout() {    // metodo per sloggare
    localStorage.removeItem('token');
    this.alertify.message('logged out');
  }

}
