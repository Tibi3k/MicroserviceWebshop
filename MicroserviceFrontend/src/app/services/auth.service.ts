import { HttpClient } from '@angular/common/http';
import { Injectable, OnInit } from '@angular/core';
import { Observable, of, map, BehaviorSubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { User } from '../model/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService implements OnInit {

  private API_URL: string = environment.apiUrl
  private currentUser: BehaviorSubject<User | null> = new BehaviorSubject<User | null>(null)
  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    let token = localStorage.getItem("JWT");


  }

  addJWTtoLocalStrorage(token: string){
    localStorage.setItem("JWT", token);
  }

  authenticateUser(username: string, password: string): Observable<User>{
    return this.http.post<User>(this.API_URL + 'auth',{
      username: username,
      password: password
    },{observe: 'response'}).pipe(map(response => {
      this.addJWTtoLocalStrorage(response.headers.get('Authorization')!)
      return response.body!
    })) 
  }

  registerUser(username: string, password: string, EmailAddress: string): Observable<User>{
    return this.http.post<User>(this.API_URL + 'auth/register', {
      username: username,
      EmailAddress: EmailAddress,
      password: password
    }).pipe(map(user => {
      this.currentUser.next(user)
      return user
    }))
  }


  getCurrentUserListener(){
    return this.currentUser
  }

  logOut(): Observable<unknown>{
    localStorage.clear()
    this.currentUser.next(null)
    return new Observable
  }

  
}
