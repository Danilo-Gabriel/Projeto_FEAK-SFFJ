import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ProdutoDTO } from '../../../models/dto/produto-dto';
import { ProdutoRequest } from '../../../models/request/produto-request';
import { ServiceResponse } from '../../../models/response/service-response';
import { HttpServiceService } from '../../../shared/services/http-service.service';

@Injectable({
  providedIn: 'root'
})
export class ProdutoService {
  constructor(private httpService: HttpServiceService) {}

  listarProdutos(): Observable<ServiceResponse<ProdutoDTO[]>> {
    return this.httpService.get<ProdutoDTO[]>('Produtos');
  }

  obterPorCodigoBarras(codigoBarras: string): Observable<ServiceResponse<ProdutoDTO>> {
    return this.httpService.get<ProdutoDTO>(`Produtos/codigo-barras/${codigoBarras}`);
  }

  cadastrarProduto(payload: ProdutoRequest): Observable<ServiceResponse<ProdutoDTO>> {
    return this.httpService.post<ServiceResponse<ProdutoDTO>>('Produtos', payload);
  }

  atualizarProduto(payload: ProdutoDTO): Observable<ServiceResponse<ProdutoDTO>> {
    return this.httpService.put<ServiceResponse<ProdutoDTO>>('Produtos', payload);
  }

  excluirProduto(id: string): Observable<ServiceResponse<ProdutoDTO>> {
    return this.httpService.delete<ProdutoDTO>(`Produtos/${id}`);
  }

  importarProdutos(arquivo: File): Observable<ServiceResponse<ProdutoDTO[]>> {
    const formData = new FormData();
    formData.append('arquivo', arquivo);
    return this.httpService.post<ServiceResponse<ProdutoDTO[]>>('Produtos/importar', formData);
  }

  baixarTemplateImportacao(): Observable<Blob> {
    return this.httpService.getBlob('Produtos/template-importacao');
  }
}