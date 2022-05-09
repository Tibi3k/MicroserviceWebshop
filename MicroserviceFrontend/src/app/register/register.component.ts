import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  form: FormGroup
  
  constructor(public authService: AuthService, private router: Router) { 
    this.form = new FormGroup({
      username: new FormControl(null, {validators: [Validators.required, Validators.minLength(3), Validators.max(30)]
      }),
      email: new FormControl(null, {validators: [Validators.required, Validators.minLength(3), Validators.email, Validators.max(30)]
      }),
      password: new FormControl(null, {validators: [Validators.required, Validators.minLength(7), Validators.max(30)]}),
      passwordAgain: new FormControl(null, {validators: [Validators.required, Validators.minLength(7),Validators.max(30)]}),
    });
  }


  username = ''
  lastPasswordInvalid = false

  onRegister(){
    // if(this.form.valid){
    //   this.authService.registerUser(this.form.value.username, this.form.value.password, this.form.value.email)
    //   .subscribe({
    //     next: (user) => this.router.navigate(['']),
    //     error: (error) => this.lastPasswordInvalid = true
    //   })
    // }
  }
}