import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProdutoDTO } from '../../models/dto/produto-dto';
import { PdvItemDTO } from '../../models/dto/pdv-item-dto';
import { VendaDTO } from '../../models/dto/venda-dto';
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
  public itemEmEdicaoId: string | null = null;
  public exibirModalConsumidor: boolean = false;
  public consumidorEditando: string = '';
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
    const quantidade = this.converterNumero(this.pdvForm.get('quantidade')?.value);
    const valorUnitario = this.converterNumero(this.pdvForm.get('valorUnitario')?.value);

    const subtotal = quantidade * valorUnitario;
    const descontoAplicado = this.obterDescontoDigitadoValor();
    const total = subtotal - descontoAplicado;
    return total > 0 ? total : 0;
  }

  get emEdicao(): boolean {
    return !!this.itemEmEdicaoId;
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
      produtoId: produto.id,
      codigoBarras: produto.codigoBarras,
      valorUnitario: produto.precoVenda,
      quantidade: 1,
      descontoValor: 0,
      descontoPercentual: 0
    }, { emitEvent: false });

    if (this.pdvForm.get('inclusaoAutomatica')?.value) {
      this.adicionarItem();
    }
  }

  adicionarItem(): void {
    const produtoId = this.pdvForm.get('produtoId')?.value as string;
    const produto = this.produtos.find((item) => item.id === produtoId);

    if (!produto) {
      this.messageService.showWarn('Selecione um produto para adicionar.');
      return;
    }

    const quantidade = this.converterNumero(this.pdvForm.get('quantidade')?.value);
    const valorUnitario = this.converterNumero(this.pdvForm.get('valorUnitario')?.value);
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

    if (this.itemEmEdicaoId) {
      if (quantidade > produto.estoqueAtual) {
        this.messageService.showWarn('A quantidade editada ultrapassa o estoque disponível.');
        return;
      }

      this.itensVenda = this.itensVenda.map((item) => {
        if (item.id !== this.itemEmEdicaoId) {
          return item;
        }

        return {
          ...item,
          codigo: produto.codigoBarras,
          descricao: produto.descricao,
          quantidade,
          precoUnitario: valorUnitario,
          estoqueDisponivel: produto.estoqueAtual,
          descontoValor,
          descontoPercentual
        };
      });

      this.messageService.showSuccess('Item atualizado na lista.');
      this.cancelarDigitacao(false);
      return;
    }

    if (itemExistente) {
      const novaQuantidade = itemExistente.quantidade + quantidade;
      const novoDescontoValor = Number(((itemExistente.descontoValor ?? 0) + descontoValor).toFixed(2));
      const novoSubtotalBruto = novaQuantidade * valorUnitario;

      this.itensVenda = this.itensVenda.map((item) => {
        if (item.id !== itemExistente.id) {
          return item;
        }

        return {
          ...item,
          quantidade: novaQuantidade,
          precoUnitario: valorUnitario,
          descontoValor: novoDescontoValor,
          descontoPercentual: novoSubtotalBruto > 0
            ? Number(((novoDescontoValor / novoSubtotalBruto) * 100).toFixed(2))
            : 0,
          codigo: produto.codigoBarras
        };
      });

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

    if (this.itemEmEdicaoId === itemId) {
      this.cancelarDigitacao();
    }
  }

  editarItem(item: PdvItemDTO): void {
    const produto = this.produtos.find((produtoAtual) => produtoAtual.id === item.id);
    this.itemEmEdicaoId = item.id;
    this.produtoSelecionado = produto ?? {
      id: item.id,
      codigoBarras: item.codigo ?? '',
      descricao: item.descricao,
      precoCusto: 0,
      precoVenda: item.precoUnitario,
      estoqueAtual: item.estoqueDisponivel
    };

    this.origemDesconto = 'valor';
    this.pdvForm.patchValue({
      produtoId: item.id,
      codigoBarras: item.codigo ?? '',
      quantidade: item.quantidade,
      valorUnitario: item.precoUnitario,
      descontoValor: item.descontoValor ?? 0,
      descontoPercentual: item.descontoPercentual ?? 0
    }, { emitEvent: false });
  }

  pesquisarPorCodigoBarras(adicionarAposPesquisa: boolean = false): void {
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

      if (adicionarAposPesquisa && !inclusaoAutomatica) {
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

        this.imprimirReciboVenda(response.dados);
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
    this.itemEmEdicaoId = null;
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
      this.atualizarDescontoPorPercentual(this.converterNumero(value));
    });

    this.pdvForm.get('descontoValor')?.valueChanges.subscribe((value) => {
      this.origemDesconto = 'valor';
      this.atualizarDescontoPorValor(this.converterNumero(value));
    });

    this.pdvForm.get('quantidade')?.valueChanges.subscribe(() => {
      this.atualizarDescontoAtivo();
    });

    this.pdvForm.get('valorUnitario')?.valueChanges.subscribe(() => {
      this.atualizarDescontoAtivo();
    });
  }

  onEditorSubmit(event: Event): void {
    event.preventDefault();

    const produtoId = this.pdvForm.get('produtoId')?.value;
    if (!produtoId) {
      return;
    }

    this.adicionarItem();
  }

  onEditorEnter(event: Event): void {
    const campo = event.target as HTMLElement | null;
    if (campo?.closest('button')) {
      return;
    }

    event.preventDefault();

    if (campo?.id === 'codigoBarras') {
      this.pesquisarPorCodigoBarras(true);
      return;
    }

    if (!this.pdvForm.get('produtoId')?.value) {
      return;
    }

    this.adicionarItem();
  }

  private obterSubtotalBrutoDigitado(): number {
    const quantidade = this.converterNumero(this.pdvForm.get('quantidade')?.value);
    const valorUnitario = this.converterNumero(this.pdvForm.get('valorUnitario')?.value);
    return quantidade * valorUnitario;
  }

  private obterDescontoDigitadoValor(): number {
    const subtotal = this.obterSubtotalBrutoDigitado();
    const descontoValor = this.converterNumero(this.pdvForm.get('descontoValor')?.value);
    return this.normalizarDesconto(descontoValor, subtotal);
  }

  private obterDescontoPercentualDigitado(): number {
    const subtotal = this.obterSubtotalBrutoDigitado();
    const descontoValor = this.obterDescontoDigitadoValor();
    return subtotal > 0 ? Number(((descontoValor / subtotal) * 100).toFixed(2)) : 0;
  }

  private atualizarDescontoAtivo(): void {
    if (this.origemDesconto === 'percentual') {
      const percentual = this.converterNumero(this.pdvForm.get('descontoPercentual')?.value);
      this.atualizarDescontoPorPercentual(percentual);
      return;
    }

    const descontoValor = this.converterNumero(this.pdvForm.get('descontoValor')?.value);
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

  private converterNumero(valor: unknown): number {
    if (typeof valor === 'number') {
      return Number.isFinite(valor) ? valor : 0;
    }

    if (typeof valor !== 'string') {
      return 0;
    }

    const texto = valor.trim();
    if (!texto) {
      return 0;
    }

    const normalizado = texto.includes(',')
      ? texto.replace(/\./g, '').replace(',', '.')
      : texto;
    const numero = Number(normalizado);
    return Number.isFinite(numero) ? numero : 0;
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

  abrirModalConsumidor(): void {
    this.consumidorEditando = this.pdvForm.get('consumidor')?.value || '';
    this.exibirModalConsumidor = true;
  }

  fecharModalConsumidor(): void {
    this.exibirModalConsumidor = false;
    this.consumidorEditando = '';
  }

  salvarConsumidor(): void {
    const nomeConsumidor = this.consumidorEditando?.trim() || 'CONSUMIDOR FINAL';
    this.pdvForm.patchValue({
      consumidor: nomeConsumidor
    }, { emitEvent: false });
    this.messageService.showSuccess(`Consumidor alterado para: ${nomeConsumidor}`);
    this.fecharModalConsumidor();
  }

  private imprimirReciboVenda(venda: VendaDTO): void {
    const janelaRecibo = window.open('', '_blank', 'width=320,height=600');

    if (!janelaRecibo) {
      this.messageService.showWarn('Venda registrada, mas não foi possível abrir a impressão automática.');
      return;
    }

    const itensHtml = venda.itens.length
      ? venda.itens.flatMap((item) => {
          const linhas: string[] = [];
          for (let i = 0; i < item.quantidade; i++) {
            linhas.push(`
              <tr>
                <td>1x</td>
                <td>[ ] ${item.descricaoProduto}</td>
                <td>${this.formatarMoedaRecibo(item.precoUnitario)}</td>
                <td>${this.formatarMoedaRecibo(item.precoUnitario)}</td>
              </tr>
            `);
          }
          return linhas;
        }).join('')
      : `
          <tr>
            <td colspan="4">Nenhum item encontrado.</td>
          </tr>
        `;

    const reciboHtml = `
      <!DOCTYPE html>
      <html lang="pt-BR">
      <head>
        <meta charset="UTF-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1.0" />
        <title>Recibo ${venda.numeroVenda}</title>
       <style>

  *{
    margin:0;
    padding:0;
    box-sizing:border-box;
  }

  html,
  body{
    width:72mm;
    font-family: monospace;
    font-size:10px;
    line-height:1.1;
    color:#000;
  }

  body{
    padding:2mm;
  }

  .topo{
    text-align:center;
    margin-bottom:4px;
  }

  .topo h1{
    font-size:13px;
    margin:0;
  }

  .topo p{
    font-size:10px;
    margin:1px 0 0;
  }

  .bloco{
    margin-bottom:4px;
  }

  .linha{
    display:flex;
    justify-content:space-between;
    gap:4px;
    margin:1px 0;
  }

  .linha strong{
    font-size:10px;
  }

  table{
    width:100%;
    border-collapse:collapse;
    table-layout:fixed;
    font-size:10px;
  }

  th{
    text-align:left;
    border-bottom:1px dashed #000;
    padding-bottom:2px;
    font-size:10px;
  }

  td{
    padding:1px 0;
    vertical-align:top;
    word-break:break-word;
  }

  .itens td{
    padding:3px 0;
    font-size:11px;
    line-height:1.35;
  }

  .itens tr + tr td{
    padding-top:4px;
  }

  .itens th:nth-child(1),
  .itens td:nth-child(1){
    width:10%;
  }

  .itens th:nth-child(2),
  .itens td:nth-child(2){
    width:52%;
  }

  .itens th:nth-child(3),
  .itens td:nth-child(3){
    width:18%;
    text-align:right;
  }

  .itens th:nth-child(4),
  .itens td:nth-child(4){
    width:20%;
    text-align:right;
  }

  .totais{
    margin-top:3px;
  }

  @media print{

    html,
    body{
      width:72mm;
    }

    @page{
      size:80mm auto;
      margin:0;
    }

  }

</style>
      </head>
      <body>
        <div class="topo">
          <h1>Recibo de Venda</h1>
          <p>FEAK - Sistema Financeiro</p>
        </div>

        <div class="bloco">
          <div class="linha"><span>Venda</span><strong>${venda.numeroVenda}</strong></div>
          <div class="linha"><span>Data</span><strong>${this.formatarDataRecibo(venda.dhInclusao)}</strong></div>
          <div class="linha"><span>Operador</span><strong>${venda.operador || '-'}</strong></div>
          <div class="linha"><span>Consumidor</span><strong>${venda.consumidor || '-'}</strong></div>
          <div class="linha"><span>Pagamento</span><strong>${venda.formaPagamento || '-'}</strong></div>
          <div class="linha"><span>Status</span><span>${venda.cancelada ? 'Cancelada' : 'Fechada'}</span></div>
        </div>

        <div class="bloco">
          <table class="itens">
            <thead>
              <tr>
                <th>Qtd</th>
                <th>Produto</th>
                <th>Unit.</th>
                <th>Total</th>
              </tr>
            </thead>
            <tbody>
              ${itensHtml}
            </tbody>
          </table>
        </div>

        <div class="totais">
          <div class="linha"><span>Subtotal</span><strong>${this.formatarMoedaRecibo(venda.subtotal)}</strong></div>
          <div class="linha"><span>Desconto</span><strong>${this.formatarMoedaRecibo(venda.descontoTotal)}</strong></div>
          <div class="linha"><span>Acréscimo</span><strong>${this.formatarMoedaRecibo(venda.acrescimo)}</strong></div>
          <div class="linha"><span>Total</span><strong>${this.formatarMoedaRecibo(venda.total)}</strong></div>
        </div>

       <script>
  window.onload = () => {
    setTimeout(() => {
      window.print();
    }, 200);
  };
</script>
      </body>
      </html>
    `;

    janelaRecibo.document.open();
    janelaRecibo.document.write(reciboHtml);
    janelaRecibo.document.close();
  }

  private formatarMoedaRecibo(valor: number): string {
    return Number(valor ?? 0).toLocaleString('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    });
  }

  private formatarDataRecibo(data: Date | string): string {
    return new Date(data).toLocaleString('pt-BR');
  }
}
