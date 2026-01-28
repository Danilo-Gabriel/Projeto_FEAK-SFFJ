import { Component, OnInit, ViewChild } from '@angular/core';
import { HttpServiceService } from '../../services/http-service.service';
import { AppMessageService } from '../../services/app-message.service';
import { ConfirmationService } from 'primeng/api';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Table } from 'primeng/table';
import { ProdutoDTO } from '../../models/dto/produto-dto';
import { ServiceResponse } from '../../models/response/service-response';

@Component({
  selector: 'app-produto',
  templateUrl: './produto.component.html',
  styleUrl: './produto.component.scss'
})
export class ProdutoComponent implements OnInit {


  constructor(
    private apiService: HttpServiceService,
    private message: AppMessageService,
    private confirmationService: ConfirmationService,
    private formBuilder: FormBuilder
  ) { }


  /*  ATRIBUTOS */

  @ViewChild('dt1') dt1!: Table;
  public formProduto!: FormGroup
  public listaProdutos!: ProdutoDTO[];
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
    this.formProduto = this.formBuilder.group({
      id: [0],
      descricao: ['', Validators.required],
      precoCusto: [0, Validators.required],
      precoVenda: [0, Validators.required],
      estoqueAtual: ['', Validators.required]
    })
  }


  onSubmit() {
    console.log("TESTE", this.formProduto)
    if (this.formProduto.valid) {

      if(this.formProduto.get('id')?.value == 0 || this.formProduto.get('id')?.value == null){
           this.criarUsuario(this.formProduto.value);
      }
      else{
        this.atualizarUsuario(this.formProduto.value);
      }
    }

  }



  acaoProduto(product?: ProdutoDTO) {
    if(product?.id != null && product != undefined){
      this.labelModel = "Editar";
      this.formProduto.patchValue(product);
    }
    else{
      this.labelModel = "Cadastrar";
      this.formProduto.reset();
    }

     this.visivel = true;
  }


  infoProduto(produtc: ProdutoDTO) {
    this.confirmationService.confirm({
      message: `Deseja seguir com exclusão do produto ${produtc.descricao}?`,
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
        this.inativarUsuario(produtc.id);
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

  criarUsuario(novoUsuario: ProdutoDTO): void {
    this.apiService.post<ServiceResponse<ProdutoDTO>>('usuario', novoUsuario)
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
    this.apiService.get<ProdutoDTO[]>('usuario')
      .subscribe({
        next: (data) => {
          this.listaProdutos = data.dados;
        },
        error: () => {
          this.message.showError("Erro ao buscar usuários");
        }
      });
  }


  atualizarUsuario(usuario: ProdutoDTO): void {
    this.apiService.put<ProdutoDTO>(`usuario`, usuario)
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
    this.apiService.delete<ServiceResponse<ProdutoDTO>>(`usuario/${id}`)
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

