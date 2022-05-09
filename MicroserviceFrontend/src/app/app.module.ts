import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http'

import {MatTableModule} from '@angular/material/table';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MatIconModule} from '@angular/material/icon';
import {MatButtonModule} from '@angular/material/button';
import {MatCardModule} from '@angular/material/card';
import {MatFormFieldModule} from '@angular/material/form-field';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {MatInputModule} from '@angular/material/input';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {MatSelectModule} from '@angular/material/select';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {MatRadioModule} from '@angular/material/radio';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { BackendService } from './services/backend.service';
import { ProductListComponent } from './ProductList/product-list.component';
import { AuthorizationInterceptor } from './services/token.interceptor';
import { HeaderComponent } from './header/header.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { BasketComponent } from './basket/basket.component';
import { OrderComponent } from './order/order.component';
import { CreateProductComponent } from './create-product/create-product.component';
import { AuthGuard } from './services/auth.guard';
import { CreateCategoryComponent } from './create-category/create-category.component';
import { MsalBroadcastService, MsalGuard, MsalGuardConfiguration, MsalInterceptor, MsalInterceptorConfiguration, MsalModule, MsalService, MSAL_GUARD_CONFIG, MSAL_INSTANCE, MSAL_INTERCEPTOR_CONFIG } from '@azure/msal-angular';
import { IPublicClientApplication,PublicClientApplication, InteractionType, BrowserCacheLocation, LogLevel } from '@azure/msal-browser';
import { environment } from 'src/environments/environment';

@NgModule({
  declarations: [
    AppComponent,
    ProductListComponent,
    HeaderComponent,
    LoginComponent,
    RegisterComponent,
    BasketComponent,
    OrderComponent,
    CreateProductComponent,
    CreateCategoryComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    MatTableModule,
    MatToolbarModule,
    MatIconModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    //FormsModule,
    ReactiveFormsModule,
    MatInputModule,
    BrowserAnimationsModule,
    MatSelectModule,
    MatCheckboxModule,
    MatRadioModule,
    MsalModule
  ],
  providers: [
    BackendService,
    {provide: HTTP_INTERCEPTORS, useClass: AuthorizationInterceptor, multi: true},
    AuthGuard,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: MsalInterceptor,
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
  protectedResourceMap.set('http://localhost/api/products/public', []);
  protectedResourceMap.set('http://localhost/api/categories/get', []);
  protectedResourceMap.set('http://localhost/api/products/protected', [environment.scopeUrl]);
  protectedResourceMap.set('http://localhost/api/categories/create', [environment.scopeUrl]);
  protectedResourceMap.set('http://localhost/api/basket/', [environment.scopeUrl]);
  protectedResourceMap.set('http://localhost/api/order/', [environment.scopeUrl]);

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

