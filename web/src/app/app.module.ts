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
import { EstoqueComponent } from './pages/estoque/estoque.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    UsuarioComponent,
    PagesComponent,
    SidebarComponent,
    EstoqueComponent,
  ],
  imports: [
    BrowserModule,
    ReactiveFormsModule,
    AppRoutingModule,
    LibsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
