export interface ServiceResponse<T> {
  dados: T,
  mensagem: string;
  sucess: string;
}