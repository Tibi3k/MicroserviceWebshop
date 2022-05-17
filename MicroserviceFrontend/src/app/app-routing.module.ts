import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BasketComponent } from './components/basket/basket.component';
import { CreateCategoryComponent } from './components/create-category/create-category.component';
import { CreateProductComponent } from './components/create-product/create-product.component';
import { OrderComponent } from './components/order/order.component';
import { ProductListComponent } from './components/ProductList/product-list.component';
import { AuthGuard } from './services/providers/auth.guard';

const routes: Routes = [
  { path: '', component: ProductListComponent },
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
