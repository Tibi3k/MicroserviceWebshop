import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
    form: FormGroup
  
    constructor(public authService: AuthService, private router: Router) { 
  
      this.form = new FormGroup({
        username: new FormControl(null, {validators: [Validators.required, Validators.minLength(3)]
        }),
        password: new FormControl(null, {validators: [Validators.required]}),
      });
    }
    username = ''
    lastPasswordInvalid = false
  
    onLogin(){
      if(this.form.valid){
        this.authService.authenticateUser(this.form.value.username, this.form.value.password)
        .subscribe({
          next: (user) => this.router.navigate(['']),
          error: (error) => this.lastPasswordInvalid = true
        })
      }
    }
  }