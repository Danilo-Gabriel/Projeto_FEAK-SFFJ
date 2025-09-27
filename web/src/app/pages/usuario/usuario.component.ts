import { Component, OnInit, ViewChild } from '@angular/core';
import { Table } from 'primeng/table';
import { UsuarioDTO } from '../../models/dto/user-dto';
import { AppMessageService } from '../../services/app-message.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConfirmationService } from 'primeng/api';
import { HttpServiceService } from '../../services/http-service.service';
import { ServiceResponse } from '../../models/dto/service-response';

@Component({
  selector: 'app-usuario',
  templateUrl: './usuario.component.html',
  styleUrl: './usuario.component.scss'
})
export class UsuarioComponent implements OnInit {


  constructor(
    private apiService: HttpServiceService,
    private message: AppMessageService,
    private confirmationService: ConfirmationService,
    private formBuilder: FormBuilder
  ) { }


  /*  ATRIBUTOS */

  @ViewChild('dt1') dt1!: Table;
  public formUsuario!: FormGroup
  public listaUsuarios!: UsuarioDTO[];
  public visivel: boolean = false;
  public labelModel: string = ''


  /*  METÓDOS HERDADOS */

  ngOnInit(): void {
    this.obterUsuarios();
    this.formulario();
  }



  /*  METÓDOS AUXILIAREIS */


  onFilterGlobal(event: Event) {
    const input = event.target as HTMLInputElement;
    const value = input?.value ?? '';
    this.dt1.filterGlobal(value, 'contains');
  }

  
  formulario() {
    this.formUsuario = this.formBuilder.group({
      id: [0],
      nomeCompleto: ['', Validators.required],
      nomeLogin: ['', Validators.required],
      senha: ['', Validators.required]
    })
  }


  onSubmit() {
    console.log("TESTE", this.formUsuario)
    if (this.formUsuario.valid) {
    
      if(this.formUsuario.get('id')?.value == 0 || this.formUsuario.get('id')?.value == null){
           this.criarUsuario(this.formUsuario.value);
      }
      else{
        this.atualizarUsuario(this.formUsuario.value);
      }
    }
   
  }



  acaoUsuario(user?: UsuarioDTO) {
    if(user?.id != null && user != undefined){
      this.labelModel = "Editar";
      this.formUsuario.patchValue(user);
    }
    else{
      this.labelModel = "Cadastrar";
      this.formUsuario.reset();
    }

     this.visivel = true;
  }


  inforUsuario(user: UsuarioDTO) {
    this.confirmationService.confirm({
      message: `Deseja seguir com exclusão do usuário ${user.nomeLogin}?`,
      header: 'Confirmação',
      icon: 'pi pi-info-circle',
      acceptButtonStyleClass: "p-button-danger p-button-text",
      rejectButtonStyleClass: "p-button-text p-button-text",
      acceptIcon: "none",
      rejectIcon: "none",
      acceptLabel: "Sim",
      rejectLabel: "Não",
      accept: () => {
        // NO FUTUTO ALTERAR PARA UM CODE QUE SERIA O ID CODIFICADO..
        this.inativarUsuario(user.id);
      },
      reject: () => {
        console.log('Usuário cancelou');
      }
    });
  }


  fechar() {
    this.visivel = !this.visivel;
  }



  //  CRUD : TRANSFERIR PARA UM SERVICE DEPOIS

  criarUsuario(novoUsuario: UsuarioDTO): void {
    this.apiService.post<ServiceResponse<UsuarioDTO>>('usuario', novoUsuario)
      .subscribe({
        next: (dados) => {
          console.log(dados, "DADOS")
          if(dados.success == false){
              this.message.showError("Falha ao criar o usuário");
              this.message.showError(dados.mensagem);
              this.fechar();
          }
          else{
          this.message.showSuccess("Usuário criado com sucesso");
          this.fechar();
          this.obterUsuarios();
          }
        },
        error: () => {
          this.message.showError("Erro ao criar usuário");
        }
      });
  }

  
  obterUsuarios() {
    this.apiService.get<UsuarioDTO[]>('usuario')
      .subscribe({
        next: (data) => {
          this.listaUsuarios = data.dados;
        },
        error: () => {
          this.message.showError("Erro ao buscar usuários");
        }
      });
  }


  atualizarUsuario(usuario: UsuarioDTO): void {
    this.apiService.put<UsuarioDTO>(`usuario`, usuario)
      .subscribe({
        next: (dad) => {
          this.message.showSuccess("Usuário atualizado");
          this.obterUsuarios();
          this.fechar();
        },
        error: () => {
          this.message.showError("Erro ao atualizar usuário");
        }
      });
  }

  inativarUsuario(id: number): void {
    this.apiService.delete<ServiceResponse<UsuarioDTO>>(`usuario/${id}`)
      .subscribe({
      next: (dados) => {
          if(dados.success == false){
              this.message.showError("Falha ao inativar usuário");
              this.message.showError(dados.mensagem);
          }
          if(dados.success == true){
          this.message.showSuccess("Usuário Excluido com sucesso");
          this.obterUsuarios();
          }
        },
        error: () => {
          this.message.showError("Erro geral");
        }
      });
  }
}

