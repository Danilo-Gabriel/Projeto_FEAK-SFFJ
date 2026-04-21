export interface ProdutoDTO {
    id: string;
    codigoBarras: string;
    descricao: string;
    precoCusto: number;
    precoVenda: number;
    estoqueAtual: number;
    dhInclusao?: Date;
    dhExclusao?: Date | null;
}