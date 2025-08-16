import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UsuarioComponent } from '../usuario/usuario.component';
import { PagesComponent } from '../pages.component';
import { EstoqueComponent } from '../estoque/estoque.component';

const routes: Routes = [
    {path: '', component: PagesComponent,

    children: [
    { path: 'usuario', component: UsuarioComponent},
    { path: 'estoque', component: EstoqueComponent},


    ]
  }
];


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PagesRoutingModule { }
