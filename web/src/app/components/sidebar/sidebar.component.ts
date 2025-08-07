import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppMessageService } from '../../services/app-message.service';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent implements OnInit {


  constructor(
    private router : Router,
    private message : AppMessageService
  ){

  }

    sidebarClosed = false;


    ngOnInit(): void {
      
    }

    toggleSidebar(): void {
       this.sidebarClosed = !this.sidebarClosed;
    }


    logout(){
     this.router.navigate(['/login']);
    }

    
}
