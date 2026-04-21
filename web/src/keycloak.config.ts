import { Injectable } from '@angular/core';
import { KeycloakService } from 'keycloak-angular';
import { environment } from './environment/environment';

@Injectable({ providedIn: 'root' })
export class KeycloakInitService {
  constructor(private keycloak: KeycloakService) {}

  init() {
    return this.keycloak.init({
      config: {
        url: environment.KEYCLOAK_URL,
        realm: environment.KEYCLOAK_REALM,
        clientId: environment.KEYCLOAK_CLIENT_ID
      },
      initOptions: {
        onLoad: 'login-required', //login-required, check-sso
        silentCheckSsoRedirectUri:
          window.location.origin + '/assets/public/silent-check-sso.html',
        pkceMethod: 'S256'
      }
    });
  }
}
