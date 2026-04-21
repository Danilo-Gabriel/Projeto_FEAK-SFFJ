import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProdutoDTO } from '../../models/dto/produto-dto';
import { PdvItemDTO } from '../../models/dto/pdv-item-dto';
import { RegistrarVendaRequest } from '../../models/request/registrar-venda-request';
import { AuthSessionService } from '../../core/services/auth-session.service';
import { AppMessageService } from '../../shared/services/app-message.service';
import { PdvService } from './services/pdv.service';

@Component({
  selector: 'app-pdv',
  templateUrl: './pdv.component.html',
  styleUrl: './pdv.component.scss'
})
export class PdvComponent implements OnInit {
  public pdvForm!: FormGroup;
  public produtos: ProdutoDTO[] = [];
  public itensVenda: PdvItemDTO[] = [];
  public produtoSelecionado: ProdutoDTO | null = null;
  public salvandoVenda: boolean = false;
  private origemDesconto: 'valor' | 'percentual' = 'valor';
  private readonly authSessionService = inject(AuthSessionService);

  constructor(
    private formBuilder: FormBuilder,
    private pdvService: PdvService,
    private messageService: AppMessageService
  ) {}

  ngOnInit(): void {
    this.criarFormulario();
    this.configurarSincronizacaoDescontos();
    this.carregarProdutos();
  }

  get quantidadeTotal(): number {
    return this.itensVenda.reduce((total, item) => total + item.quantidade, 0);
  }

  get subtotal(): number {
    return this.itensVenda.reduce((total, item) => total + this.obterSubtotalItem(item), 0);
  }

  get acrescimo(): number {
    return 0;
  }

  get descontoTotal(): number {
    return this.itensVenda.reduce((total, item) => {
      const subtotalBruto = item.precoUnitario * item.quantidade;
      return total + (subtotalBruto - this.obterSubtotalItem(item));
    }, 0);
  }

  get totalVenda(): number {
    return this.subtotal + this.acrescimo;
  }

  get totalItemDigitado(): number {
    const quantidade = Number(this.pdvForm.get('quantidade')?.value ?? 0);
    const valorUnitario = Number(this.pdvForm.get('valorUnitario')?.value ?? 0);

    const subtotal = quantidade * valorUnitario;
    const descontoAplicado = this.obterDescontoDigitadoValor();
    const total = subtotal - descontoAplicado;
    return total > 0 ? total : 0;
  }

  onProdutoSelecionado(produtoId: string): void {
    const produto = this.produtos.find((item) => item.id === produtoId) ?? null;
    this.produtoSelecionado = produto;

    if (!produto) {
      this.pdvForm.patchValue({
        valorUnitario: 0,
        quantidade: 1,
        descontoValor: 0,
        descontoPercentual: 0,
        codigoBarras: ''
      }, { emitEvent: false });
      return;
    }

    this.pdvForm.patchValue({
      codigoBarras: produto.codigoBarras,
      valorUnitario: produto.precoVenda,
      quantidade: 1,
      descontoValor: 0,
      descontoPercentual: 0
    }, { emitEvent: false });
  }

  adicionarItem(): void {
    const produtoId = this.pdvForm.get('produtoId')?.value as string;
    const produto = this.produtos.find((item) => item.id === produtoId);

    if (!produto) {
      this.messageService.showWarn('Selecione um produto para adicionar.');
      return;
    }

    const quantidade = Number(this.pdvForm.get('quantidade')?.value ?? 0);
    const valorUnitario = Number(this.pdvForm.get('valorUnitario')?.value ?? 0);
    const descontoValor = this.obterDescontoDigitadoValor();
    const descontoPercentual = this.obterDescontoPercentualDigitado();

    if (quantidade <= 0) {
      this.messageService.showWarn('Informe uma quantidade válida.');
      return;
    }

    if (quantidade > produto.estoqueAtual) {
      this.messageService.showWarn('Quantidade maior do que o estoque disponível.');
      return;
    }

    const itemExistente = this.itensVenda.find((item) => item.id === produto.id);
    const totalQuantidade = quantidade + (itemExistente?.quantidade ?? 0);

    if (totalQuantidade > produto.estoqueAtual) {
      this.messageService.showWarn('A soma das quantidades ultrapassa o estoque disponível.');
      return;
    }

    const subtotalBruto = quantidade * valorUnitario;
    const descontoTotal = descontoValor;

    if (descontoTotal > subtotalBruto) {
      this.messageService.showWarn('O desconto não pode ser maior que o subtotal do item.');
      return;
    }

    if (itemExistente) {
      const novaQuantidade = itemExistente.quantidade + quantidade;
      const novoDescontoValor = Number(((itemExistente.descontoValor ?? 0) + descontoValor).toFixed(2));
      const novoSubtotalBruto = novaQuantidade * valorUnitario;

      itemExistente.quantidade += quantidade;
      itemExistente.precoUnitario = valorUnitario;
      itemExistente.descontoValor = novoDescontoValor;
      itemExistente.descontoPercentual = novoSubtotalBruto > 0
        ? Number(((novoDescontoValor / novoSubtotalBruto) * 100).toFixed(2))
        : 0;
      itemExistente.codigo = produto.codigoBarras;
      this.cancelarDigitacao(false);
      return;
    }

    this.itensVenda = [
      ...this.itensVenda,
      {
        id: produto.id,
        codigo: produto.codigoBarras,
        descricao: produto.descricao,
        precoUnitario: valorUnitario,
        quantidade,
        estoqueDisponivel: produto.estoqueAtual,
        descontoValor,
        descontoPercentual
      }
    ];

    this.cancelarDigitacao(false);
  }

  alterarQuantidade(item: PdvItemDTO, variacao: number): void {
    const novaQuantidade = item.quantidade + variacao;

    if (novaQuantidade <= 0) {
      this.removerItem(item.id);
      return;
    }

    if (novaQuantidade > item.estoqueDisponivel) {
      this.messageService.showWarn('A quantidade solicitada ultrapassa o estoque disponível.');
      return;
    }

    this.itensVenda = this.itensVenda.map((produto) => {
      if (produto.id !== item.id) {
        return produto;
      }

      return {
        ...produto,
        quantidade: novaQuantidade
      };
    });
  }

  removerItem(itemId: string): void {
    this.itensVenda = this.itensVenda.filter((item) => item.id !== itemId);
  }

  pesquisarPorCodigoBarras(): void {
    const codigoBarras = this.pdvForm.get('codigoBarras')?.value?.toString().trim();
    const buscaReferencia = !!this.pdvForm.get('buscaReferencia')?.value;
    const inclusaoAutomatica = !!this.pdvForm.get('inclusaoAutomatica')?.value;
    const exclusaoAutomatica = !!this.pdvForm.get('exclusaoAutomatica')?.value;

    if (!codigoBarras) {
      return;
    }

    const localizarProduto = (produtos: ProdutoDTO[]): ProdutoDTO | undefined => {
      const termo = codigoBarras.toLowerCase();

      return produtos.find((produto) => produto.codigoBarras === codigoBarras)
        ?? (buscaReferencia
          ? produtos.find((produto) => produto.descricao.toLowerCase().includes(termo)
            || produto.codigoBarras.toLowerCase().includes(termo)
            || produto.id.toLowerCase().includes(termo))
          : undefined);
    };

    const aplicarSelecao = (produto: ProdutoDTO): void => {
      const itemExistente = this.itensVenda.find((item) => item.id === produto.id);

      if (exclusaoAutomatica && itemExistente) {
        this.removerItem(produto.id);
        this.cancelarDigitacao();
        return;
      }

      this.pdvForm.patchValue({ produtoId: produto.id }, { emitEvent: false });
      this.onProdutoSelecionado(produto.id);

      if (inclusaoAutomatica) {
        this.adicionarItem();
      }

    //  if (inclusaoAutomatica && itemExistente) {
    //     this.messageService.showInfo('Produto carregado no editor. Confirme a quantidade e clique em Adicionar para lançar na listagem.');
    //   }
    };

    const produtoLocal = localizarProduto(this.produtos);
    if (produtoLocal) {
      aplicarSelecao(produtoLocal);
      return;
    }

    this.pdvService.listarProdutos().subscribe({
      next: (produtos) => {
        this.produtos = produtos;
        const produto = localizarProduto(produtos);
        if (!produto) {
          this.messageService.showWarn('Produto não encontrado para o código informado.');
          return;
        }

        aplicarSelecao(produto);
      },
      error: () => {
        this.messageService.showError('Não foi possível localizar o produto pelo código de barras.');
      }
    });
  }

  finalizarVenda(): void {
    if (this.itensVenda.length === 0) {
      this.messageService.showWarn('Adicione pelo menos um item antes de finalizar a venda.');
      return;
    }

    const operador = this.authSessionService.obterOperador();
    if (!operador) {
      this.messageService.showWarn('Não foi possível identificar o operador logado. Faça login novamente.');
      return;
    }

    const payload: RegistrarVendaRequest = {
      operador,
      consumidor: this.pdvForm.get('consumidor')?.value,
      formaPagamento: 'DINHEIRO',
      acrescimo: this.acrescimo,
      itens: this.itensVenda.map((item) => ({
        produtoId: item.id,
        codigoProduto: item.codigo ?? '',
        descricaoProduto: item.descricao,
        quantidade: item.quantidade,
        precoUnitario: item.precoUnitario,
        descontoValor: item.descontoValor ?? 0,
        descontoPercentual: item.descontoPercentual ?? 0
      }))
    };

    this.salvandoVenda = true;
    this.pdvService.registrarVenda(payload).subscribe({
      next: (response) => {
        this.salvandoVenda = false;
        if (!response.success || !response.dados) {
          this.messageService.showError(response.mensagem || 'Não foi possível registrar a venda.');
          return;
        }

        this.messageService.showSuccess(`Venda ${response.dados.numeroVenda} registrada com sucesso.`);
        this.itensVenda = [];
        this.cancelarDigitacao();
        this.carregarProdutos();
      },
      error: () => {
        this.salvandoVenda = false;
        this.messageService.showError('Erro ao registrar venda.');
      }
    });
  }

  cancelarDigitacao(limparBusca: boolean = true): void {
    this.produtoSelecionado = null;
    this.pdvForm.patchValue({
      codigoBarras: '',
      produtoId: '',
      quantidade: 1,
      valorUnitario: 0,
      descontoValor: 0,
      descontoPercentual: 0,
    }, { emitEvent: false });
  }

  obterSubtotalItem(item: PdvItemDTO): number {
    const subtotal = item.quantidade * item.precoUnitario;
    const descontoValor = Number(item.descontoValor ?? 0);
    const total = subtotal - descontoValor;
    return total > 0 ? total : 0;
  }

  private criarFormulario(): void {
    this.pdvForm = this.formBuilder.group({
      consumidor: ['CONSUMIDOR FINAL'],
      codigoBarras: [''],
      produtoId: ['', Validators.required],
      quantidade: [1, [Validators.required, Validators.min(1)]],
      valorUnitario: [0, [Validators.required, Validators.min(0)]],
      descontoValor: [0, [Validators.min(0)]],
      descontoPercentual: [0, [Validators.min(0)]],
      inclusaoAutomatica: [true],
      exclusaoAutomatica: [false],
      buscaReferencia: [false]
    });
  }

  private configurarSincronizacaoDescontos(): void {
    this.pdvForm.get('descontoPercentual')?.valueChanges.subscribe((value) => {
      this.origemDesconto = 'percentual';
      this.atualizarDescontoPorPercentual(Number(value ?? 0));
    });

    this.pdvForm.get('descontoValor')?.valueChanges.subscribe((value) => {
      this.origemDesconto = 'valor';
      this.atualizarDescontoPorValor(Number(value ?? 0));
    });

    this.pdvForm.get('quantidade')?.valueChanges.subscribe(() => {
      this.atualizarDescontoAtivo();
    });

    this.pdvForm.get('valorUnitario')?.valueChanges.subscribe(() => {
      this.atualizarDescontoAtivo();
    });
  }

  private obterSubtotalBrutoDigitado(): number {
    const quantidade = Number(this.pdvForm.get('quantidade')?.value ?? 0);
    const valorUnitario = Number(this.pdvForm.get('valorUnitario')?.value ?? 0);
    return quantidade * valorUnitario;
  }

  private obterDescontoDigitadoValor(): number {
    const subtotal = this.obterSubtotalBrutoDigitado();
    const descontoValor = Number(this.pdvForm.get('descontoValor')?.value ?? 0);
    return this.normalizarDesconto(descontoValor, subtotal);
  }

  private obterDescontoPercentualDigitado(): number {
    const subtotal = this.obterSubtotalBrutoDigitado();
    const descontoValor = this.obterDescontoDigitadoValor();
    return subtotal > 0 ? Number(((descontoValor / subtotal) * 100).toFixed(2)) : 0;
  }

  private atualizarDescontoAtivo(): void {
    if (this.origemDesconto === 'percentual') {
      const percentual = Number(this.pdvForm.get('descontoPercentual')?.value ?? 0);
      this.atualizarDescontoPorPercentual(percentual);
      return;
    }

    const descontoValor = Number(this.pdvForm.get('descontoValor')?.value ?? 0);
    this.atualizarDescontoPorValor(descontoValor);
  }

  private atualizarDescontoPorPercentual(percentualInformado: number): void {
    const subtotal = this.obterSubtotalBrutoDigitado();
    const percentualNormalizado = percentualInformado < 0 ? 0 : percentualInformado;
    const descontoValor = subtotal > 0
      ? this.normalizarDesconto((subtotal * percentualNormalizado) / 100, subtotal)
      : 0;

    this.pdvForm.patchValue({
      descontoPercentual: subtotal > 0 ? Number(((descontoValor / subtotal) * 100).toFixed(2)) : 0,
      descontoValor
    }, { emitEvent: false });
  }

  private atualizarDescontoPorValor(descontoInformado: number): void {
    const subtotal = this.obterSubtotalBrutoDigitado();
    const descontoValor = this.normalizarDesconto(descontoInformado, subtotal);
    const descontoPercentual = subtotal > 0 ? Number(((descontoValor / subtotal) * 100).toFixed(2)) : 0;

    this.pdvForm.patchValue({
      descontoValor,
      descontoPercentual
    }, { emitEvent: false });
  }

  private normalizarDesconto(descontoInformado: number, subtotal: number): number {
    if (subtotal <= 0) {
      return 0;
    }

    if (descontoInformado <= 0) {
      return 0;
    }

    return Number(Math.min(descontoInformado, subtotal).toFixed(2));
  }

  private carregarProdutos(): void {
    this.pdvService.listarProdutos().subscribe({
      next: (produtos) => {
        this.produtos = produtos;
      },
      error: () => {
        this.produtos = [];
        this.messageService.showError('Não foi possível carregar o catálogo do PDV.');
      }
    });
  }
}