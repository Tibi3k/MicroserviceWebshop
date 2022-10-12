import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { Form, UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { Product, ProductCreate } from '../../model/product.model';
import { AuthService } from '../../services/auth.service';
import { BackendService } from '../../services/backend.service';

@Component({
  selector: 'app-create-product',
  templateUrl: './create-product.component.html',
  styleUrls: ['./create-product.component.css'],
})
export class CreateProductComponent implements OnInit {
  form!: UntypedFormGroup
  constructor(
    public route: ActivatedRoute,
    public authService: AuthService,
    private backendService: BackendService,
    private router: Router
  ) {}

  mode = 'create'
  categories: Array<any> = ['asdf', 'dfasdf', 'asdfasdf']
  productId: Number | null = null

  ngOnInit() {
    this.createForm()
    this.backendService.getAllCategories()
      .subscribe(categories => this.categories = categories)
    this.route.paramMap.subscribe((paramMap: ParamMap) => {
      if (paramMap.has('productId')) {
        this.productId =  Number.parseInt(paramMap.get('productId')!);
        this.mode = 'edit'
        this.backendService.getProductById(this.productId)
          .subscribe(product => {
            this.loadItemToForm(product)
          })
      }
    })
  }

  onSaveProduct(){
    if(this.mode == 'edit'){
      let product = this.getExistingProductFromForm()
      this.backendService.editProduct(product).subscribe(res => {
        this.router.navigate([''])
      })
    } else {
      let product = this.getNewProductFromForm()
      this.backendService.createProduct(product).subscribe(res => {
        this.router.navigate([''])
      })
    }
  }

  getNewProductFromForm(): ProductCreate{
    return {
      name: this.form.value.name,
      description: this.form.value.description,
      price: this.form.value.price,
      quantity: this.form.value.quantity,
      category: this.form.value.category
    }
  }

  getExistingProductFromForm(): Product{
    return {
      id: this.productId!,
      name: this.form.value.name,
      description: this.form.value.description,
      price: this.form.value.price,
      quantity: this.form.value.quantity,
      category: this.form.value.category
    }
  }

  loadItemToForm(product: Product){
    this.form.setValue({
      name: product.name,
      description: product.description ?? '',
      price: product.price,
      quantity: product.quantity,
      category: product.category,
    });
  }

  createForm(){
    this.form = new UntypedFormGroup({
      name: new UntypedFormControl(null, {
        validators: [Validators.required, Validators.minLength(3)],
      }),
      description: new UntypedFormControl(null, { validators: [Validators.required] }),
      quantity: new UntypedFormControl(null, {validators: [Validators.required],}),
      price: new UntypedFormControl(null, { validators: [Validators.required] }),
      category: new UntypedFormControl(null, { validators: [Validators.required] }),
    });
  }

  onCancelClicked(){
    this.router.navigate([''])
  }
}

