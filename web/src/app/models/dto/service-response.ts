export interface ServiceResponse<T> {
  dados: T,
  mensagem: string;
  success: boolean;
}