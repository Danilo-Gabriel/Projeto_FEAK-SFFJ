import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Table } from 'primeng/table';
import { ProdutoDTO } from '../../models/dto/produto-dto';
import { ConfirmationService } from 'primeng/api';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProdutoRequest } from '../../models/request/produto-request';
import { AppMessageService } from '../../shared/services/app-message.service';
import { ProdutoService } from './services/produto.service';

@Component({
  selector: 'app-produto',
  templateUrl: './produto.component.html',
  styleUrl: './produto.component.scss'
})
export class ProdutoComponent implements OnInit {
  constructor(
    private produtoService: ProdutoService,
    private appMessageService: AppMessageService,
    private confirmationService: ConfirmationService,
    private formBuilder: FormBuilder
  ) { }

  @ViewChild('dt1') dt1!: Table;
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;
  public formProduto!: FormGroup;
  public listaProdutos: ProdutoDTO[] = [];
  public visivel: boolean = false;
  public labelModel: string = '';
  public importando: boolean = false;

  ngOnInit(): void {
    this.formulario();
    this.obterProdutos();
  }

  onFilterGlobal(event: Event): void {
    const input = event.target as HTMLInputElement;
    const value = input?.value ?? '';
    this.dt1.filterGlobal(value, 'contains');
  }

  formulario(): void {
    this.formProduto = this.formBuilder.group({
      id: [''],
      codigoBarras: ['', Validators.required],
      descricao: ['', Validators.required],
      precoCusto: [0, [Validators.required, Validators.min(0)]],
      precoVenda: [0, [Validators.required, Validators.min(0)]],
      estoqueAtual: [0, [Validators.required, Validators.min(0)]]
    });
  }

  onSubmit(): void {
    if (this.formProduto.valid) {
      const id = this.formProduto.get('id')?.value as string;
      if (!id) {
        this.criarProduto(this.formProduto.value as ProdutoRequest);
      }
      else {
        this.atualizarProduto(this.formProduto.value as ProdutoDTO);
      }
    }
  }

  acaoProduto(produto?: ProdutoDTO): void {
    if (produto?.id) {
      this.labelModel = 'Editar produto';
      this.formProduto.patchValue(produto);
    }
    else {
      this.labelModel = 'Novo produto';
      this.formProduto.reset();
      this.formProduto.patchValue({
        id: '',
        codigoBarras: '',
        precoCusto: 0,
        precoVenda: 0,
        estoqueAtual: 0
      });
    }

    this.visivel = true;
  }

  infoProduto(produto: ProdutoDTO): void {
    this.confirmationService.confirm({
      message: `Deseja seguir com exclusão do produto ${produto.descricao}?`,
      header: 'Confirmação',
      icon: 'pi pi-info-circle',
      acceptButtonStyleClass: 'p-button-danger p-button-text',
      rejectButtonStyleClass: 'p-button-text p-button-text',
      acceptIcon: 'none',
      rejectIcon: 'none',
      acceptLabel: 'Sim',
      rejectLabel: 'Não',
      accept: () => {
        this.inativarProduto(produto.id);
      }
    });
  }

  abrirImportacao(): void {
    this.fileInput.nativeElement.click();
  }

  onArquivoSelecionado(event: Event): void {
    const input = event.target as HTMLInputElement;
    const arquivo = input.files?.[0];

    if (!arquivo) {
      return;
    }

    this.importando = true;
    this.produtoService.importarProdutos(arquivo)
      .subscribe({
        next: (dados) => {
          this.importando = false;
          if (!dados.success) {
            this.appMessageService.showError(dados.mensagem || 'Falha ao importar produtos.');
          }
          else {
            this.appMessageService.showSuccess(dados.mensagem || 'Importação concluída.');
            this.obterProdutos();
          }
          input.value = '';
        },
        error: () => {
          this.importando = false;
          input.value = '';
          this.appMessageService.showError('Erro ao importar produtos.');
        }
      });
  }

  baixarTemplateImportacao(): void {
    this.produtoService.baixarTemplateImportacao().subscribe({
      next: (arquivo) => {
        const blobUrl = window.URL.createObjectURL(arquivo);
        const link = document.createElement('a');
        link.href = blobUrl;
        link.download = 'template_importacao_produtos.xlsx';
        link.click();
        window.URL.revokeObjectURL(blobUrl);
      },
      error: () => {
        this.appMessageService.showError('Erro ao baixar template de importação.');
      }
    });
  }

  fechar(): void {
    this.visivel = false;
  }

  private criarProduto(novoProduto: ProdutoRequest): void {
    this.produtoService.cadastrarProduto(novoProduto)
      .subscribe({
        next: (dados) => {
          if (!dados.success) {
            this.appMessageService.showError(dados.mensagem || 'Falha ao cadastrar produto.');
            return;
          }

          this.appMessageService.showSuccess(dados.mensagem || 'Produto cadastrado com sucesso.');
          this.fechar();
          this.obterProdutos();
        },
        error: () => {
          this.appMessageService.showError('Erro ao cadastrar produto.');
        }
      });
  }

  private obterProdutos(): void {
    this.produtoService.listarProdutos()
      .subscribe({
        next: (dados) => {
          this.listaProdutos = (dados.dados || []).map((produto) => ({
            ...produto,
            precoCusto: Number(produto.precoCusto ?? 0),
            precoVenda: Number(produto.precoVenda ?? 0),
            estoqueAtual: Number(produto.estoqueAtual ?? 0)
          }));
        },
        error: () => {
          this.appMessageService.showError('Erro ao buscar produtos.');
        }
      });
  }

  private atualizarProduto(produto: ProdutoDTO): void {
    this.produtoService.atualizarProduto(produto)
      .subscribe({
        next: (dados) => {
          if (!dados.success) {
            this.appMessageService.showError(dados.mensagem || 'Falha ao atualizar produto.');
            return;
          }

          this.appMessageService.showSuccess(dados.mensagem || 'Produto atualizado com sucesso.');
          this.fechar();
          this.obterProdutos();
        },
        error: () => {
          this.appMessageService.showError('Erro ao atualizar produto.');
        }
      });
  }

  private inativarProduto(id: string): void {
    this.produtoService.excluirProduto(id)
      .subscribe({
        next: (dados) => {
          if (!dados.success) {
            this.appMessageService.showError(dados.mensagem || 'Falha ao excluir produto.');
            return;
          }

          this.appMessageService.showSuccess(dados.mensagem || 'Produto excluído com sucesso.');
          this.obterProdutos();
        },
        error: () => {
          this.appMessageService.showError('Erro ao excluir produto.');
        }
      });
  }
}

