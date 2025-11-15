import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from '../components/login/login.component';
import { DevelopComponent } from '../components/develop/develop.component';

const routes: Routes = [
    {path: '', pathMatch: 'full', redirectTo: 'login'},
    {path: 'login', component: LoginComponent},
    {path: 'develop', component: DevelopComponent},

    {path:'pages',
    loadChildren: () => import('../pages/pages.module').then(m => m.PagesModule)}

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
