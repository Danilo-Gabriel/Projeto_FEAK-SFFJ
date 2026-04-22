import { Component, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { KeycloakService } from 'keycloak-angular';
import { AuthSessionService } from '../../core/services/auth-session.service';
import { AppMessageService } from '../../shared/services/app-message.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent implements OnInit {


  constructor(
    private router : Router,
    private message : AppMessageService,
    private authSessionService: AuthSessionService,
    private keycloak: KeycloakService
  ){

  }

    sidebarClosed = false;
    mobileOpen = false;

    ngOnInit(): void {
      this.router.events.subscribe(event => {
        if (event instanceof NavigationEnd) {
          this.mobileOpen = false;
        }
      });
    }

    toggleSidebar(): void {
       this.sidebarClosed = !this.sidebarClosed;
    }

    toggleMobile(): void {
      this.mobileOpen = !this.mobileOpen;
    }

    closeMobile(): void {
      this.mobileOpen = false;
    }


    async logout(){
      this.authSessionService.limparSessao();
      const autenticado = this.keycloak.isLoggedIn();
      if (autenticado) {
        await this.keycloak.logout(window.location.origin);
        return;
      }

      this.router.navigate(['/login']);
    }

    
}
