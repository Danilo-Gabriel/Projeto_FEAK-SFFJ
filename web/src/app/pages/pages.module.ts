import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { LibsModule } from '../../assets/libs/libs.module';
import { PagesRoutingModule } from '../routing/pages-routing.module';

@NgModule({
  providers: [MessageService],
  declarations: [

  ],
  imports: [
    CommonModule,
    PagesRoutingModule,
    ReactiveFormsModule,
    LibsModule
  ]
})

export class PagesModule { }
