import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { RequestState } from '../model/request-state.model';
import { BackendService } from '../services/backend.service';

@Component({
  selector: 'app-create-category',
  templateUrl: './create-category.component.html',
  styleUrls: ['./create-category.component.css']
})
export class CreateCategoryComponent implements OnInit {

  constructor(
    private backendService: BackendService,
    private router: Router
  ) { }

  form!: FormGroup
  errorMsg = "Somthing went wrong!"
  state = RequestState
  loadState = RequestState.success

  ngOnInit(): void {
    this.form = new FormGroup({
      name: new FormControl(null, {
        validators: [Validators.required, Validators.minLength(3)],
      })
    })
  }

  onAddCategory(){
    if(this.form.valid){
      this.loadState = this.state.loading
      this.backendService.AddCategory(this.form.value.name)
       .subscribe({
         next: res => {
           this.loadState = this.state.success
           this.router.navigate([''])
         },
         error: error => {
           this.loadState = this.state.error
           this.errorMsg = error
         }
      })
    }
  }

}
