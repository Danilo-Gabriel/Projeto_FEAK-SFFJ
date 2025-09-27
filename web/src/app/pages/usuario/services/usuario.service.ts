import { Injectable } from '@angular/core';
import { HttpServiceService } from '../../../services/http-service.service';
import { UsuarioDTO } from '../../../models/dto/user-dto';

@Injectable({
  providedIn: 'root'
})
export class UsuarioService {

  constructor(
    private apiService: HttpServiceService
  ) {}

}
