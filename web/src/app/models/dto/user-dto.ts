export interface UsuarioDTO{
    id: string,
    nomeCompleto: string,
    nomeLogin: string,
    ativo: boolean,
    dhInclusao: Date,
    dhExclusao: Date
}