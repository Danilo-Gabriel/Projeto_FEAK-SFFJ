import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { KeycloakService } from 'keycloak-angular';
import { UsuarioDTO } from '../../models/dto/user-dto';
import { UsuarioLogadoDTO } from '../../models/dto/usuario-logado-dto';

@Injectable({
  providedIn: 'root'
})
export class AuthSessionService {
  private readonly storageKey = 'feak.usuario-logado';
  private readonly sessionStorageKey = 'feak.usuario-logado.keycloak';
  private readonly usuarioLogadoSubject = new BehaviorSubject<UsuarioLogadoDTO | null>(this.lerSessaoKeycloak());

  public readonly usuarioLogado$ = this.usuarioLogadoSubject.asObservable();

  salvarUsuario(usuario: UsuarioDTO): void {
    localStorage.setItem(this.storageKey, JSON.stringify(usuario));

    this.salvarSessaoInterna({
      id: usuario.id,
      nomeCompleto: usuario.nomeCompleto,
      nomeLogin: usuario.nomeLogin,
      email: null,
      token: null,
      perfis: [],
      autenticado: true,
      origem: 'sistema'
    });
  }

  async sincronizarComKeycloak(keycloak: KeycloakService): Promise<UsuarioLogadoDTO | null> {
    const autenticado = keycloak.isLoggedIn();
    if (!autenticado) {
      return this.obterUsuarioLogado();
    }

    const token = await keycloak.getToken().catch(() => null);
    const perfil = await keycloak.loadUserProfile().catch(() => null);
    const tokenParseado = (keycloak.getKeycloakInstance().tokenParsed ?? {}) as Record<string, unknown>;

    const realmAccess = this.obterObjeto(tokenParseado['realm_access']);
    const resourceAccess = this.obterObjeto(tokenParseado['resource_access']);
    const perfis = new Set<string>();

    const perfisRealm = Array.isArray(realmAccess?.['roles']) ? realmAccess['roles'] : [];
    perfisRealm.forEach((perfilAtual) => {
      if (typeof perfilAtual === 'string' && perfilAtual.trim()) {
        perfis.add(perfilAtual);
      }
    });

    Object.values(resourceAccess ?? {}).forEach((recurso) => {
      const recursoObjeto = this.obterObjeto(recurso);
      const roles = Array.isArray(recursoObjeto?.['roles']) ? recursoObjeto['roles'] : [];
      roles.forEach((perfilAtual) => {
        if (typeof perfilAtual === 'string' && perfilAtual.trim()) {
          perfis.add(perfilAtual);
        }
      });
    });

    const sessao: UsuarioLogadoDTO = {
      id: this.obterTexto(tokenParseado['sub']) ?? this.obterTexto(perfil?.['id']) ?? null,
      nomeCompleto:
        this.obterTexto(tokenParseado['name'])
        ?? this.obterTexto(perfil?.['firstName'])
        ?? this.obterTexto(perfil?.['username'])
        ?? null,
      nomeLogin:
        this.obterTexto(tokenParseado['preferred_username'])
        ?? this.obterTexto(perfil?.['username'])
        ?? null,
      email:
        this.obterTexto(tokenParseado['email'])
        ?? this.obterTexto(perfil?.['email'])
        ?? null,
      token,
      perfis: Array.from(perfis),
      autenticado: true,
      origem: 'keycloak'
    };

    this.salvarSessaoInterna(sessao);
    return sessao;
  }

  obterUsuario(): UsuarioDTO | null {
    const usuarioSalvo = localStorage.getItem(this.storageKey);
    if (usuarioSalvo) {
      try {
        return JSON.parse(usuarioSalvo) as UsuarioDTO;
      } catch {
        this.limparSessao();
        return null;
      }
    }

    const sessao = this.obterUsuarioLogado();
    if (!sessao) {
      return null;
    }

    return {
      id: sessao.id ?? '',
      nomeCompleto: sessao.nomeCompleto ?? sessao.nomeLogin ?? '',
      nomeLogin: sessao.nomeLogin ?? sessao.email ?? '',
      ativo: sessao.autenticado,
      dhInclusao: new Date(),
      dhExclusao: null
    };
  }

  obterUsuarioLogado(): UsuarioLogadoDTO | null {
    return this.usuarioLogadoSubject.value ?? this.lerSessaoKeycloak();
  }

  obterOperador(): string | null {
    const usuarioLogado = this.obterUsuarioLogado();
    if (usuarioLogado) {
      return usuarioLogado.nomeLogin || usuarioLogado.nomeCompleto || usuarioLogado.email || null;
    }

    const usuario = this.obterUsuario();
    return usuario?.nomeLogin || usuario?.nomeCompleto || null;
  }

  limparSessao(): void {
    localStorage.removeItem(this.storageKey);
    localStorage.removeItem(this.sessionStorageKey);
    this.usuarioLogadoSubject.next(null);
  }

  private salvarSessaoInterna(usuario: UsuarioLogadoDTO): void {
    localStorage.setItem(this.sessionStorageKey, JSON.stringify(usuario));
    this.usuarioLogadoSubject.next(usuario);
  }

  private lerSessaoKeycloak(): UsuarioLogadoDTO | null {
    const sessaoSalva = localStorage.getItem(this.sessionStorageKey);
    if (!sessaoSalva) {
      return null;
    }

    try {
      return JSON.parse(sessaoSalva) as UsuarioLogadoDTO;
    } catch {
      localStorage.removeItem(this.sessionStorageKey);
      return null;
    }
  }

  private obterTexto(valor: unknown): string | null {
    return typeof valor === 'string' && valor.trim() ? valor : null;
  }

  private obterObjeto(valor: unknown): Record<string, unknown> | null {
    return valor && typeof valor === 'object' ? (valor as Record<string, unknown>) : null;
  }
}