import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environment/environment';
import { ProdutoDTO } from '../../../models/dto/produto-dto';
import { ProdutoRequest } from '../../../models/request/produto-request';
import { ServiceResponse } from '../../../models/response/service-response';

@Injectable({
  providedIn: 'root'
})
export class ProdutoService {
  private readonly apiUrl = `${environment.endPoint}/Produtos`;

  constructor(private http: HttpClient) {}

  listarProdutos(): Observable<ServiceResponse<ProdutoDTO[]>> {
    return this.http.get<ServiceResponse<ProdutoDTO[]>>(this.apiUrl);
  }

  obterPorCodigoBarras(codigoBarras: string): Observable<ServiceResponse<ProdutoDTO>> {
    return this.http.get<ServiceResponse<ProdutoDTO>>(`${this.apiUrl}/codigo-barras/${codigoBarras}`);
  }

  cadastrarProduto(payload: ProdutoRequest): Observable<ServiceResponse<ProdutoDTO>> {
    return this.http.post<ServiceResponse<ProdutoDTO>>(this.apiUrl, payload);
  }

  atualizarProduto(payload: ProdutoDTO): Observable<ServiceResponse<ProdutoDTO>> {
    return this.http.put<ServiceResponse<ProdutoDTO>>(this.apiUrl, payload);
  }

  excluirProduto(id: string): Observable<ServiceResponse<ProdutoDTO>> {
    return this.http.delete<ServiceResponse<ProdutoDTO>>(`${this.apiUrl}/${id}`);
  }

  importarProdutos(arquivo: File): Observable<ServiceResponse<ProdutoDTO[]>> {
    const formData = new FormData();
    formData.append('arquivo', arquivo);
    return this.http.post<ServiceResponse<ProdutoDTO[]>>(`${this.apiUrl}/importar`, formData);
  }

  baixarTemplateImportacao(): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/template-importacao`, { responseType: 'blob' });
  }
}