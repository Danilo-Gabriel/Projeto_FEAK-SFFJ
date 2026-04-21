import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UsuarioComponent } from '../pages/usuario/usuario.component';
import { ProdutoComponent } from '../pages/produto/produto.component';
import { PdvComponent } from '../pages/pdv/pdv.component';
import { SidebarComponent } from '../components/sidebar/sidebar.component';

const routes: Routes = [
    {path: '', component: SidebarComponent,

    children: [
    { path: '', pathMatch: 'full', redirectTo: 'pdv' },
    { path: 'pdv', component: PdvComponent},
    { path: 'usuario', component: UsuarioComponent},
    { path: 'produto', component: ProdutoComponent},
    { path: 'estoque', component: ProdutoComponent},

    ]
  }
];


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PagesRoutingModule { }
