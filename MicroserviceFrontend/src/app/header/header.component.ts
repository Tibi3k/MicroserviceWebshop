import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { User } from '../services/user.model';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit, OnDestroy {

  constructor(private authService: AuthService, private router: Router) { }
  authenticatedUserSubscription: Subscription | undefined
  currentUser: User | null = null

   ngOnInit(): void {
    this.authenticatedUserSubscription = this.authService.getCurrentUserListener()
      .subscribe(user => {
        this.currentUser = user
      })

  }

   ngOnDestroy(): void {
    this.authenticatedUserSubscription?.unsubscribe()
  }

  onLoginClicked(){
    if(this.currentUser == null)
      this.router.navigate(['login'])
    else{
      this.authService.logOut()
       .subscribe(_ => {
        this.router.navigate([''])
       })
    }
    
  }

}
