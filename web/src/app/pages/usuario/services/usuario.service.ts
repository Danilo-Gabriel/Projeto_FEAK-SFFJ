import { Injectable } from '@angular/core';
import { UsuarioDTO } from '../../../models/dto/user-dto';
import { UsuarioRequest } from '../../../models/request/user-request';
import { UsuarioUpdateRequest } from '../../../models/request/user-update-request';
import { ServiceResponse } from '../../../models/response/service-response';
import { HttpServiceService } from '../../../shared/services/http-service.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UsuarioService {

  constructor(
    private apiService: HttpServiceService
  ) {}

  listarUsuarios(): Observable<ServiceResponse<UsuarioDTO[]>> {
    return this.apiService.get<UsuarioDTO[]>('Usuarios');
  }

  cadastrarUsuario(payload: UsuarioRequest): Observable<ServiceResponse<UsuarioDTO>> {
    return this.apiService.post<ServiceResponse<UsuarioDTO>>('Usuarios', payload);
  }

  atualizarUsuario(payload: UsuarioUpdateRequest): Observable<ServiceResponse<UsuarioDTO>> {
    return this.apiService.put<ServiceResponse<UsuarioDTO>>('Usuarios', payload);
  }

  excluirUsuario(id: string): Observable<ServiceResponse<UsuarioDTO>> {
    return this.apiService.delete<UsuarioDTO>(`Usuarios/${id}`);
  }

}
