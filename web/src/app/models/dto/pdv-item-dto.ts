export interface PdvItemDTO {
  id: string;
  codigo?: string;
  descricao: string;
  precoUnitario: number;
  quantidade: number;
  estoqueDisponivel: number;
  descontoValor?: number;
  descontoPercentual?: number;
}