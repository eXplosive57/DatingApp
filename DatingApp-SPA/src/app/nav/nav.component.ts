import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {}; // memorizza nome utente e password che passiamo

  constructor(private authService: AuthService) { }

  ngOnInit() {
  }

  login() {
    this.authService.login(this.model).subscribe(next => { // specificando next, dicamo che ho effettuato il login con successo
      console.log('Logged in successfully');
    }, error => {
      console.log(error);
    });
  }

  loggedIn() { // metodo che dice se siamo loggati
    const token = localStorage.getItem('token');
    return !!token; // se nella variabile token c'è qualcosa darà true, altrmienti darà false
  }

  logout() {    // metodo per sloggare
    localStorage.removeItem('token');
    console.log('logged out');
  }

}
