import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { LibsModule } from '../../assets/libs/libs.module';
import { UsuarioComponent } from './usuario/usuario.component';
import { ProdutoComponent } from './produto/produto.component';
import { PdvComponent } from './pdv/pdv.component';
import { RelatorioComponent } from './relatorio/relatorio.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { PagesRoutingModule } from '../routing/pages-routing.module';

@NgModule({
  providers: [MessageService],
  declarations: [
    UsuarioComponent,
    ProdutoComponent,
    PdvComponent,
    RelatorioComponent,
    DashboardComponent,
  ],
  imports: [
    CommonModule,
    PagesRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    LibsModule
  ]
})

export class PagesModule { }
