import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BasketComponent } from './basket/basket.component';
import { CreateCategoryComponent } from './create-category/create-category.component';
import { CreateProductComponent } from './create-product/create-product.component';
import { LoginComponent } from './login/login.component';
import { OrderComponent } from './order/order.component';
import { ProductListComponent } from './ProductList/product-list.component';
import { RegisterComponent } from './register/register.component';
import { AuthGuard } from './services/auth.guard';

const routes: Routes = [
  { path: '', component: ProductListComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'basket', component: BasketComponent, canActivate: [AuthGuard] },
  { path: 'orders', component: OrderComponent, canActivate: [AuthGuard] },
  { path: 'createproduct', component: CreateProductComponent, canActivate: [AuthGuard]},
  { path: 'createcategory', component: CreateCategoryComponent, canActivate: [AuthGuard] },
  { path: 'edit/:productId', component: CreateProductComponent, canActivate: [AuthGuard]},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
