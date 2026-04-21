import { Component, OnInit, inject } from '@angular/core';
import { KeycloakService } from 'keycloak-angular';
import { AuthSessionService } from './core/services/auth-session.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  private readonly authSessionService = inject(AuthSessionService);
  private readonly keycloak = inject(KeycloakService);
  title = 'web';

  async ngOnInit(): Promise<void> {
    await this.authSessionService.sincronizarComKeycloak(this.keycloak);
  }
}
