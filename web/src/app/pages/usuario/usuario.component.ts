import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Customer } from '../../models/dto/customer';
import { Table } from 'primeng/table';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { UsuarioDTO } from '../../models/dto/user-dto';
import { catchError, map, throwError } from 'rxjs';
import { environment } from '../../../environment/environment';
import { AppMessageService } from '../../services/app-message.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-usuario',
  templateUrl: './usuario.component.html',
  styleUrl: './usuario.component.scss'
})
export class UsuarioComponent implements OnInit {


    constructor(
          private router : Router,
          private http : HttpClient,
          private message: AppMessageService,
          private formBuilder : FormBuilder
    ){}


    @ViewChild('dt1') dt1! : Table
    public listaUsuarios!: UsuarioDTO[];
    public visivel: boolean = false;
    labelModel : string = ''
    selectedCustomers!: Customer;

    public formUsuario! : FormGroup

  ngOnInit(): void {
      this.obterUsuarios();
      this.formulario();
  }

  getSeverity(status: string): 'success' | 'secondary' | 'info' | 'warning' | 'danger' | 'contrast' | undefined {
  switch (status) {
    case 'unqualified':
      return 'danger';
    case 'qualified':
      return 'success';
    case 'new':
      return 'info';
    case 'negotiation':
      return 'warning';
    case 'renewal':
      return 'secondary'; // Substitua 'null' por algo válido ou 'undefined'
    default:
      return undefined;
  }
}

onFilterGlobal(event: Event) {
  const input = event.target as HTMLInputElement;
  const value = input?.value ?? '';
  this.dt1.filterGlobal(value, 'contains');
}


obterUsuarios(){
this.http.get<UsuarioDTO[]>(`${environment.endPoint}/usuario`)
  .pipe(
    // map(users => users.filter(user => user.ativo)),
    catchError(error => {
      console.error('Erro na API', error);
      return throwError(() => new Error('Erro ao buscar usuários.'));
    })
  )
  .subscribe(data => {
    this.listaUsuarios = data;
  });
}

    showInfo() {
        this.message.showInfo("");
    }


    registrarUsuario(){
        this.labelModel = "Cadastrar";
        this.formUsuario.reset();
        this.visivel = !this.visivel;
      
    }

    editarUsuario(user : UsuarioDTO) {
        this.labelModel = "Editar";
        this.visivel = !this.visivel;
        if(user != null){
          this.formUsuario.patchValue(user);
        }
        console.log(user, "DADOS USUÃRIO");
    }

    formulario(){
        this.formUsuario = this.formBuilder.group({
          nomeCompleto: ['', Validators.required],
          nomeLogin: ['', Validators.required],
          senha: ['', Validators.required]
        })
    }


    onSubmit(){
      if(this.formUsuario.valid){
        console.log("VALIDO")
      }
      else{
      console.log("NAO VALIDO")
      }
    }

    fechar(){
      this.visivel = !this.visivel;
    }
}

