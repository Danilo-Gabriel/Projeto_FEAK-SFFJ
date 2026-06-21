import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ProdutoDTO } from '../../../models/dto/produto-dto';
import { VendaDTO } from '../../../models/dto/venda-dto';
import { RegistrarVendaRequest } from '../../../models/request/registrar-venda-request';
import { ServiceResponse } from '../../../models/response/service-response';
import { HttpServiceService } from '../../../shared/services/http-service.service';
import { ConsumidorFinalDTO } from '../../../models/dto/consumidor-final-dto';

@Injectable({
  providedIn: 'root'
})
export class PdvService {
  constructor(private httpService: HttpServiceService) {}

  listarProdutos(): Observable<ProdutoDTO[]> {
    return this.httpService.get<ProdutoDTO[]>('Produtos').pipe(
      map((response) => this.normalizarProdutos(response?.dados))
    );
  }

  registrarVenda(payload: RegistrarVendaRequest): Observable<ServiceResponse<VendaDTO>> {
    return this.httpService.post<ServiceResponse<VendaDTO>>('Vendas', payload);
  }

  listarConsumidores(): Observable<ConsumidorFinalDTO[]> {
    return this.httpService.get<ConsumidorFinalDTO[]>('ConsumidoresFinais').pipe(
      map((response) => response?.dados ?? [])
    );
  }

  cadastrarConsumidor(nome: string): Observable<ServiceResponse<ConsumidorFinalDTO>> {
    return this.httpService.post<ServiceResponse<ConsumidorFinalDTO>>('ConsumidoresFinais', { nome });
  }

  listarVendas(): Observable<VendaDTO[]> {
    return this.httpService.get<VendaDTO[]>('Vendas').pipe(
      map((response) => response?.dados ?? [])
    );
  }

  cancelarVenda(vendaId: string): Observable<ServiceResponse<VendaDTO>> {
    return this.httpService.put<ServiceResponse<VendaDTO>>(`Vendas/${vendaId}/cancelar`, {});
  }

  exportarRelatorioExcel(): Observable<Blob> {
    return this.httpService.getBlob('Vendas/exportar-excel');
  }

  private normalizarProdutos(produtos: ProdutoDTO[] | null | undefined): ProdutoDTO[] {
    if (!produtos) {
      throw new Error('A API não retornou o catálogo de produtos.');
    }

    return produtos.map((produto) => ({
      id: produto.id,
      codigoBarras: produto.codigoBarras,
      descricao: produto.descricao,
      precoCusto: Number(produto.precoCusto ?? 0),
      precoVenda: Number(produto.precoVenda ?? 0),
      estoqueAtual: Number(produto.estoqueAtual ?? 0)
    }));
  }
}
