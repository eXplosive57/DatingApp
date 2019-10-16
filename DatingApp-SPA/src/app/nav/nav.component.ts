import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {}; // memorizza nome utente e password che passiamo
  photoUrl: string;

  constructor(public authService: AuthService, private alertify: AlertifyService, private router: Router) { }

  ngOnInit() {
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
  }

  login() {
    this.authService.login(this.model).subscribe(next => { // specificando next, dicamo che ho effettuato il login con successo
      this.alertify.success('Loggato con successo');
    }, error => {
      this.alertify.error(error);
    },  () => {
      this.router.navigate(['/members']);   // ci permette di indirizzarci al component membri dopo la fase di login
    });
  }

  loggedIn() { // metodo che dice se siamo loggati
    return this.authService.loggedIn();
  }

  logout() {    // metodo per sloggare
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authService.decodedToken = null;
    this.authService.currentUser = null;
    this.alertify.message('logged out');
    this.router.navigate(['/home']);    // ci permette di indirizzarci al component home dopo essersi sloggati
  }

}
