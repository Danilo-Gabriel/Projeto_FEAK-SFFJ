export interface VendaDTO {
  id: string;
  numeroVenda: string;
  operador: string;
  consumidor: string;
  formaPagamento: string;
  subtotal: number;
  descontoTotal: number;
  acrescimo: number;
  total: number;
  cancelada: boolean;
  dhInclusao: Date;
  dhCancelamento: Date | null;
  itens: VendaItemDTO[];
}

export interface VendaItemDTO {
  id: string;
  vendaId: string;
  produtoId: string;
  codigoProduto: string;
  descricaoProduto: string;
  quantidade: number;
  precoUnitario: number;
  descontoValor: number;
  descontoPercentual: number;
  totalItem: number;
}