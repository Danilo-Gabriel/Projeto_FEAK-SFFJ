import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { TagModule } from 'primeng/tag';



@NgModule({
exports: [
  InputTextModule,
  HttpClientModule,
  TableModule,
  TagModule,
  IconFieldModule,
  InputIconModule
  
]
})
export class LibsModule { }
