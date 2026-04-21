export interface UsuarioLogadoDTO {
  id: string | null;
  nomeCompleto: string | null;
  nomeLogin: string | null;
  email: string | null;
  token: string | null;
  perfis: string[];
  autenticado: boolean;
  origem: 'keycloak' | 'sistema';
}