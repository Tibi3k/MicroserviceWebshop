import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
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

  ngOnInit(): void {
    this.form = new FormGroup({
      name: new FormControl(null, {
        validators: [Validators.required, Validators.minLength(3)],
      })
    })
  }

  onAddCategory(){
    if(this.form.valid){
      this.backendService.AddCategory(this.form.value.name)
       .subscribe(res => {
          this.router.navigate([''])
       })
    }
  }

}
