import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AccountInfo,  } from '@azure/msal-browser';
import { Subscription } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { User } from '../model/user.model';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

  constructor(private authService: AuthService, private router: Router) { }
  authenticatedUserSubscription: Subscription | undefined
  currentUser: AccountInfo | null = null

   ngOnInit(): void {
    this.authenticatedUserSubscription = this.authService.getCurrentUserListener()
      .subscribe(user => {
        this.currentUser = user
        console.log(this.currentUser)
      })

  }

  //  ngOnDestroy(): void {
  //   this.authenticatedUserSubscription?.unsubscribe()
  // }

  onLoginClicked(){
    console.log('login')
    if(this.currentUser == null)
      this.authService.login()
    else{
      this.authService.logOut()
       .then(_ => {
        this.router.navigate([''])
       })
    }
    
  }

}
