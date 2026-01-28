import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { LibsModule } from '../../assets/libs/libs.module';
import { PagesRoutingModule } from '../routing/pages-routing.module';
import { UsuarioComponent } from './usuario/usuario.component';
import { ProdutoComponent } from './produto/produto.component';

@NgModule({
  providers: [MessageService],
  declarations: [
    UsuarioComponent,
    ProdutoComponent,
  ],
  imports: [
    CommonModule,
    PagesRoutingModule,
    ReactiveFormsModule,
    LibsModule
  ]
})

export class PagesModule { }
