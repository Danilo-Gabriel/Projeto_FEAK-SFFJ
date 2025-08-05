import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { InputTextModule } from 'primeng/inputtext';



@NgModule({
exports: [
  InputTextModule,
  HttpClientModule,
]
})
export class LibsModule { }
