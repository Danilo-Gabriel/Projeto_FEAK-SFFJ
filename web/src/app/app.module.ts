import { APP_INITIALIZER, DEFAULT_CURRENCY_CODE, LOCALE_ID, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './routing/app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './components/login/login.component';
import { LibsModule } from '../assets/libs/libs.module';
import { ReactiveFormsModule } from '@angular/forms';
import { UsuarioComponent } from './pages/usuario/usuario.component';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { ConfirmationService, MessageService } from 'primeng/api';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ProdutoComponent } from './pages/produto/produto.component';
import { DevelopComponent } from './components/develop/develop.component';
import { KeycloakInitService } from '../keycloak.config';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { CustomKeycloakInterceptor } from './core/interceptors/CustomKeycloakInterceptor';
import { KeycloakService } from 'keycloak-angular';

export function initializeKeycloak(keycloakInit: KeycloakInitService) {
  return () => keycloakInit.init();
}

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    SidebarComponent,
    DevelopComponent
  ],
  imports: [
    BrowserModule,
    ReactiveFormsModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    LibsModule
  ],
  providers: [
    MessageService,
    KeycloakService, 
    ConfirmationService,
    {
      provide: LOCALE_ID,
      useValue: 'pt-BR'
    },
    {
      provide: DEFAULT_CURRENCY_CODE,
      useValue: 'BRL'
    },
    // KeycloakInitService,
    // {
    //   provide: APP_INITIALIZER,
    //   useFactory: initializeKeycloak,
    //   deps: [KeycloakInitService],
    //   multi: true
    // },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: CustomKeycloakInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
