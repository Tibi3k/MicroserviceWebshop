import { HttpClient } from '@angular/common/http';
import { Injectable, OnInit } from '@angular/core';
import { MsalService,  } from '@azure/msal-angular';
import { AuthenticationResult,PublicClientApplication,EventType,AccountInfo  } from '@azure/msal-browser';
import { Observable, of, map, BehaviorSubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { User } from '../model/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService implements OnInit {

  private currentUser: BehaviorSubject<AccountInfo | null> = new BehaviorSubject<AccountInfo | null>(null)
  constructor(
    private http: HttpClient,
    private azureAuthService: MsalService 
    ) {}

  ngOnInit(): void {
    this.azureAuthService.initialize()

    this.azureAuthService.instance.addEventCallback((event: any) => {
      // set active account after redirect
      if (event.eventType === EventType.LOGIN_SUCCESS && event.payload.account) {
        const account = event.payload.account;
        this.azureAuthService.instance.setActiveAccount(account);
        this.currentUser.next(this.azureAuthService.instance.getActiveAccount())
        console.log(this.azureAuthService.instance.getActiveAccount())
        console.log('auth compolete')
      }
    });
  }

  isUserAdmin(){
    if(this.currentUser.value == null)
      return false
    if(this.currentUser.value.idTokenClaims?.['jobTitle'] == 'Admin')
      return true
    return false
  }


   getCurrentUserListener(){
     return this.currentUser
   }

   logOut(): Promise<void>{
    var res = this.azureAuthService.instance.logoutPopup({account: this.azureAuthService.instance.getActiveAccount()})
    res.then(result => {
      console.log('logout:' + result)
      this.currentUser.next(null)
    })
    return res
   }

  getActiveAccount(): AccountInfo | null{
    return this.azureAuthService.instance.getActiveAccount()
  }

  tryForLogin(){
    const accounts = this.azureAuthService.instance.getAllAccounts()
    if (accounts.length > 0 && !this.isAuthTokenExpired(accounts[0])) {
      this.azureAuthService.instance.setActiveAccount(accounts[0]);
      this.currentUser.next(this.azureAuthService.instance.getActiveAccount())
    }
  }



  login(){
    this.tryForLogin()
    console.log('get active account', this.azureAuthService.instance.getActiveAccount());

    this.azureAuthService.instance.handleRedirectPromise().then(authResult=>{
      console.log('auth started')
      if(authResult != null)
        this.currentUser.next(authResult?.account)
      const account = this.azureAuthService.instance.getActiveAccount();
      if(!account || this.isAuthTokenExpired(account)){
        this.azureAuthService.instance.loginPopup()
        .then(result => {
          this.azureAuthService.instance.setActiveAccount(result.account)
          this.currentUser.next(result.account)
        })
        .catch()
      }
    }).catch(err=>{
      // TODO: Handle errors
      console.log(err);
    });
  }

  isAuthTokenExpired(account: AccountInfo){
    return account.idTokenClaims?.exp! < Math.floor(Date.now()/1000)
  }

  
}
