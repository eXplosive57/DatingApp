import { Injectable } from '@angular/core';
import { User } from '../_models/user';
import { Resolve, Router, ActivatedRouteSnapshot} from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { of } from 'rxjs';

@Injectable()
export class MemberListResolver implements Resolve<User[]> {
    constructor(private userService: UserService,
        // tslint:disable-next-line: align
        private router: Router, private alertify: AlertifyService) {}


    // tslint:disable-next-line: align
    resolve(route: ActivatedRouteSnapshot): Observable<User[]> { // accesso ai dati prima di attivare un route
        // tslint:disable-next-line: no-string-literal
        return this.userService.getUsers().pipe(
            catchError(error => {
                this.alertify.error('Problem retrieving data');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}
