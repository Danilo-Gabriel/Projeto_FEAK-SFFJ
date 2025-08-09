import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PagesComponent } from '../pages/pages.component';
import { UsuarioComponent } from '../pages/usuario/usuario.component';

const routes: Routes = [
    {path: '', component: PagesComponent,

    children: [
    { path: 'usuario', component: UsuarioComponent},

    ]
  }
];


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PagesRoutingModule { }
