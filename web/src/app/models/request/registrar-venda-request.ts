export interface RegistrarVendaRequest {
  operador: string;
  consumidor: string;
  formaPagamento: string;
  acrescimo: number;
  itens: RegistrarVendaItemRequest[];
}

export interface RegistrarVendaItemRequest {
  produtoId: string;
  codigoProduto: string;
  descricaoProduto: string;
  quantidade: number;
  precoUnitario: number;
  descontoValor: number;
  descontoPercentual: number;
}