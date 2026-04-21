import { Component, OnInit, ViewChild } from '@angular/core';
import { Table } from 'primeng/table';
import { UsuarioDTO } from '../../models/dto/user-dto';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConfirmationService } from 'primeng/api';
import { UsuarioRequest } from '../../models/request/user-request';
import { UsuarioUpdateRequest } from '../../models/request/user-update-request';
import { AppMessageService } from '../../shared/services/app-message.service';
import { UsuarioService } from './services/usuario.service';


@Component({
  selector: 'app-usuario',
  templateUrl: './usuario.component.html',
  styleUrl: './usuario.component.scss'
})
export class UsuarioComponent implements OnInit {


  constructor(
    private usuarioService: UsuarioService,
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
      id: [''],
      nomeCompleto: ['', Validators.required],
      nomeLogin: ['', Validators.required],
      senha: ['', Validators.required]
    });
  }


  onSubmit() {
    if (this.formUsuario.valid) {
    
      if(this.formUsuario.get('id')?.value == 0 || this.formUsuario.get('id')?.value == null){
           this.criarUsuario(this.formUsuario.value as UsuarioRequest);
      }
      else{
        this.atualizarUsuario({
          id: this.formUsuario.get('id')?.value,
          nomeCompleto: this.formUsuario.get('nomeCompleto')?.value,
          nomeLogin: this.formUsuario.get('nomeLogin')?.value
        });
      }
    }
   
  }



  acaoUsuario(user?: UsuarioDTO) {
    if(user?.id != null && user != undefined){
      this.labelModel = "Editar";
      this.formUsuario.patchValue({
        id: user.id,
        nomeCompleto: user.nomeCompleto,
        nomeLogin: user.nomeLogin,
        senha: ''
      });
      this.formUsuario.get('senha')?.clearValidators();
    }
    else{
      this.labelModel = "Cadastrar";
      this.formUsuario.reset();
      this.formUsuario.get('senha')?.setValidators([Validators.required]);
    }

    this.formUsuario.get('senha')?.setValue('');
    this.formUsuario.get('senha')?.updateValueAndValidity();

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

  criarUsuario(novoUsuario: UsuarioRequest): void {
    this.usuarioService.cadastrarUsuario(novoUsuario)
      .subscribe({
        next: (dados) => {
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
    this.usuarioService.listarUsuarios()
      .subscribe({
        next: (data) => {
          this.listaUsuarios = (data.dados ?? []).map(x => ({
            ...x,
            ativo: x.dhExclusao != null ? false : true
          }));
        },
        error: () => {
          this.message.showError("Erro ao buscar usuários");
        }
      });
  }


  atualizarUsuario(usuario: UsuarioUpdateRequest): void {
    this.usuarioService.atualizarUsuario(usuario)
      .subscribe({
        next: (dados) => {
          if(dados.success == false){
            this.message.showError(dados.mensagem || "Falha ao atualizar usuário");
            return;
          }

          this.message.showSuccess("Usuário atualizado");
          this.obterUsuarios();
          this.fechar();
        },
        error: () => {
          this.message.showError("Erro ao atualizar usuário");
        }
      });
  }

  inativarUsuario(id: string): void {
    this.usuarioService.excluirUsuario(id)
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

