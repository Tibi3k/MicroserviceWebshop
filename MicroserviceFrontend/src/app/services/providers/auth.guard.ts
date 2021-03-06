import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from "@angular/router";
import { Observable, } from 'rxjs'
import { Injectable } from "@angular/core";
import { AuthService } from "../auth.service";


@Injectable()
export class AuthGuard implements CanActivate{

  constructor(public authService: AuthService, private router: Router) {}

    canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
    ): boolean | UrlTree | Observable<boolean | UrlTree> | Promise<boolean | UrlTree> {
        if(this.authService.getCurrentUserListener().value == null){
          this.authService.login()
          return false;
        }
        return true
  }
}