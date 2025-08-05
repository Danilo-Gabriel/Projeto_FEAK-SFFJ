import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent implements OnInit, AfterViewInit {

     @ViewChild('headerRef') sidebar!: ElementRef<HTMLElement>;


       sidebarClosed = false;

    ngAfterViewInit(): void {
     
      const body = this.sidebar.nativeElement.querySelector(".body-sidebar"),
            sidebar = body?.querySelector(".sidebar"),
            toggle = body?.querySelector(".toggle"),
            searchBtn = body?.querySelector(".search-box");
      }

    ngOnInit(): void {
      
    }

      toggleSidebar(): void {
    this.sidebarClosed = !this.sidebarClosed;
  }

 
}
