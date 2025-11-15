import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './routing/app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './components/login/login.component';
import { LibsModule } from '../assets/libs/libs.module';
import { ReactiveFormsModule } from '@angular/forms';
import { UsuarioComponent } from './pages/usuario/usuario.component';
import { PagesComponent } from './pages/pages.component';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { ConfirmationService, MessageService } from 'primeng/api';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ProdutoComponent } from './pages/produto/produto.component';
import { DevelopComponent } from './components/develop/develop.component';


@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    UsuarioComponent,
    PagesComponent,
    SidebarComponent,
    ProdutoComponent,
    DevelopComponent
  ],
  imports: [
    BrowserModule,
    ReactiveFormsModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    LibsModule
  ],
  providers: [ MessageService, ConfirmationService ],
  bootstrap: [AppComponent]
})
export class AppModule { }
