import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AccountInfo,  } from '@azure/msal-browser';
import { Subscription } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { User } from '../model/user.model';
import { BasketService } from '../services/basket.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit, OnDestroy {

  constructor(private authService: AuthService, private router: Router, private basketService: BasketService) { }
  authenticatedUserSubscription: Subscription | undefined
  basketModifiedSubscription: Subscription | undefined
  currentUser: AccountInfo | null = null
  basketCount = 0;
  date = Date.now()

  // interval = setInterval(() => {
  //   if(this.currentUser != null)
  //     this.basketService.getBasketSize()
  //       .subscribe(result => this.basketCount = result)
  // },1000)

   ngOnInit(): void {
    this.authenticatedUserSubscription = this.authService.getCurrentUserListener()
      .subscribe(user => {
        console.log("header user:",user)
        this.currentUser = user
        console.log(this.currentUser)
      })
  }

  ngOnDestroy(): void {
    // if(this.interval)
    //   clearInterval(this.interval)
   }

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
