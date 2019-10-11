import { Injectable } from '@angular/core';
import * as alertify from 'alertifyjs';

@Injectable({
  providedIn: 'root'
})
export class AlertifyService {

constructor() { }

  confirm(message: string, okCallback: () => any) {   // metodo che notifca una conferma
    alertify.confirm(message, (e: any) => {
      if (e) {                      // se c'è un evento (l'utente preme sul pulsante ok)
        okCallback();
      } else {}
    });
  }

  success(message: string) {      // metodo che notifca un SUCCESSO
    alertify.success(message);
  }

  error(message: string) {        // metodo che notifca un ERRORE
    alertify.error(message);
  }

  warning(message: string) {      // metodo che notifca un AVVISO
    alertify.warning(message);
  }

  message(message: string) {      // metodo che notifca una MESSAGGIO
    alertify.message(message);
  }

}
