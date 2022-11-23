import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http'
import {FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { BackendService } from './services/backend.service';
import { ProductListComponent } from './components/ProductList/product-list.component';
import { AuthorizationInterceptor } from './services/providers/token.interceptor';
import { HeaderComponent } from './components/common/header/header.component';
import { BasketComponent } from './components/basket/basket.component';
import { OrderComponent } from './components/order/order.component';
import { CreateProductComponent } from './components/create-product/create-product.component';
import { AuthGuard } from './services/providers/auth.guard';
import { CreateCategoryComponent } from './components/create-category/create-category.component';
import { MsalBroadcastService, MsalGuard, MsalGuardConfiguration, MsalInterceptor, MsalInterceptorConfiguration, MsalModule, MsalService, MSAL_GUARD_CONFIG, MSAL_INSTANCE, MSAL_INTERCEPTOR_CONFIG } from '@azure/msal-angular';
import { IPublicClientApplication,PublicClientApplication, InteractionType, BrowserCacheLocation, LogLevel } from '@azure/msal-browser';
import { environment } from 'src/environments/environment';
import { ErrorInterceptor } from './services/providers/error.interceptor';
import { AngularMaterialModule } from './angular-material.module';
import { PaymentComponent } from './components/payment/payment.component';
import { NgxPayPalModule } from 'ngx-paypal';


@NgModule({
  declarations: [
    AppComponent,
    ProductListComponent,
    HeaderComponent,
    BasketComponent,
    OrderComponent,
    CreateProductComponent,
    CreateCategoryComponent,
    PaymentComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    MsalModule,
    AngularMaterialModule,
    FormsModule,
    NgxPayPalModule
  ],
  providers: [
    BackendService,
    {provide: HTTP_INTERCEPTORS, useClass: AuthorizationInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true},
    AuthGuard,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: MsalInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorInterceptor,
      multi: true
    },
    {
      provide: MSAL_INSTANCE,
      useFactory: MSALInstanceFactory
    },
    {
      provide: MSAL_GUARD_CONFIG,
      useFactory: MSALGuardConfigFactory
    },
    {
      provide: MSAL_INTERCEPTOR_CONFIG,
      useFactory: MSALInterceptorConfigFactory
    },
    MsalService,
    MsalGuard,
    MsalBroadcastService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }


export function loggerCallback(logLevel: LogLevel, message: string) {
  console.log(message);
}

export function MSALInstanceFactory(): IPublicClientApplication {
  return new PublicClientApplication({
    auth: {
      clientId: environment.clientId, 
      authority: environment.authority, 
      redirectUri: environment.redirectUrl,
      knownAuthorities: ['microwebshop.b2clogin.com'],
      postLogoutRedirectUri: '/'
    },
    cache: {
      cacheLocation: BrowserCacheLocation.LocalStorage,
    },
    system: {
      loggerOptions: {
        loggerCallback,
        logLevel: LogLevel.Info,
        piiLoggingEnabled: false
      }
    }
  });
}

export function MSALInterceptorConfigFactory(): MsalInterceptorConfiguration {
  const protectedResourceMap = new Map<string, Array<string>>();
  // protectedResourceMap.set('https://graph.microsoft.com/v1.0/me', ['user.read']); // Prod environment. Uncomment to use.
  protectedResourceMap.set('https://20.238.248.85/api/products/public', []);
  protectedResourceMap.set('https://20.238.248.85/api/categories/get', []);
  protectedResourceMap.set('https://20.238.248.85/api/products/protected', [environment.scopeUrl]);
  protectedResourceMap.set('https://20.238.248.85/api/categories/create', [environment.scopeUrl]);
  protectedResourceMap.set('https://20.238.248.85/api/basket/', [environment.scopeUrl]);
  protectedResourceMap.set('https://20.238.248.85/api/orders/', [environment.scopeUrl]);
  protectedResourceMap.set('https://20.238.248.85/api/payment/', [environment.scopeUrl]);

  return {
    interactionType: InteractionType.Popup,
    protectedResourceMap
  };
}

export function MSALGuardConfigFactory(): MsalGuardConfiguration {
  return { 
    interactionType: InteractionType.Popup,
    authRequest: {
      scopes: ['user.read']
    },
    loginFailedRoute: '/login-failed'
  };
}

