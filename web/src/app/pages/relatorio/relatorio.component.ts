import { Component, OnInit } from '@angular/core';
import { ConfirmationService } from 'primeng/api';
import { VendaDTO } from '../../models/dto/venda-dto';
import { AppMessageService } from '../../shared/services/app-message.service';
import { PdvService } from '../pdv/services/pdv.service';

@Component({
  selector: 'app-relatorio',
  templateUrl: './relatorio.component.html',
  styleUrl: './relatorio.component.scss'
})
export class RelatorioComponent implements OnInit {
  public vendas: VendaDTO[] = [];
  public carregando: boolean = false;
  public cancelandoVendaId: string | null = null;
  public exportandoExcel: boolean = false;

  constructor(
    private pdvService: PdvService,
    private confirmationService: ConfirmationService,
    private messageService: AppMessageService
  ) {}

  ngOnInit(): void {
    this.carregarVendas();
  }

  confirmarCancelamento(venda: VendaDTO): void {
    if (venda.cancelada) {
      this.messageService.showInfo('A venda selecionada já está cancelada.');
      return;
    }

    this.confirmationService.confirm({
      header: 'Cancelar venda',
      message: `Deseja cancelar a venda ${venda.numeroVenda}? O estoque será devolvido automaticamente.`,
      acceptLabel: 'Cancelar venda',
      rejectLabel: 'Fechar',
      accept: () => this.cancelarVenda(venda)
    });
  }

  obterQuantidadeItens(venda: VendaDTO): number {
    return venda.itens.reduce((total, item) => total + item.quantidade, 0);
  }

  obterResumoItens(venda: VendaDTO): string {
    if (!venda.itens?.length) {
      return 'Sem itens';
    }

    return venda.itens
      .map((item) => `${item.quantidade}x ${item.descricaoProduto}`)
      .join(' | ');
  }

  get totalVendido(): number {
    return this.vendas
      .filter((venda) => !venda.cancelada)
      .reduce((acumulador, venda) => acumulador + Number(venda.total ?? 0), 0);
  }

  get totalItensVendidos(): number {
    return this.vendas
      .filter((venda) => !venda.cancelada)
      .reduce((acumulador, venda) => acumulador + this.obterQuantidadeItens(venda), 0);
  }

  get totalVendasFechadas(): number {
    return this.vendas.filter((venda) => !venda.cancelada).length;
  }

  gerarRecibo(venda: VendaDTO): void {
    const janelaRecibo = window.open('', '_blank', 'width=320,height=600');

    if (!janelaRecibo) {
      this.messageService.showError('Não foi possível abrir o recibo. Verifique se o bloqueador de pop-up está desativado.');
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
                <td>${this.formatarMoeda(item.precoUnitario)}</td>
                <td>${this.formatarMoeda(item.precoUnitario)}</td>
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

  .separador{
    border-top:1px dashed #000;
    margin:4px 0;
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

  .total-geral{
    font-size:12px;
    font-weight:bold;
  }

  .rodape{
    text-align:center;
    margin-top:5px;
    font-size:10px;
  }

  .print-action{
    display:none;
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
          <div class="linha"><span>Data</span><strong>${this.formatarData(venda.dhInclusao)}</strong></div>
          <div class="linha"><span>Operador</span><strong>${venda.operador || '-'}</strong></div>
          <div class="linha"><span>Consumidor</span><strong>${venda.consumidor || '-'}</strong></div>
          <div class="linha"><span>Pagamento</span><strong>${venda.formaPagamento || '-'}</strong></div>
          <div class="linha"><span>Status</span><span class="status ${venda.cancelada ? 'cancelada' : 'fechada'}">${venda.cancelada ? 'Cancelada' : 'Fechada'}</span></div>
        </div>

        <div class="bloco">
          <table>
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
          <div class="linha"><span>Subtotal</span><strong>${this.formatarMoeda(venda.subtotal)}</strong></div>
          <div class="linha"><span>Desconto</span><strong>${this.formatarMoeda(venda.descontoTotal)}</strong></div>
          <div class="linha"><span>Acréscimo</span><strong>${this.formatarMoeda(venda.acrescimo)}</strong></div>
          <div class="linha"><span>Total</span><strong>${this.formatarMoeda(venda.total)}</strong></div>
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

  private formatarMoeda(valor: number): string {
    return Number(valor ?? 0).toLocaleString('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    });
  }

  private formatarData(data: Date | string): string {
    return new Date(data).toLocaleString('pt-BR');
  }

  exportarExcel(): void {
    this.exportandoExcel = true;

    this.pdvService.exportarRelatorioExcel().subscribe({
      next: (arquivo) => {
        this.exportandoExcel = false;
        const blobUrl = window.URL.createObjectURL(arquivo);
        const link = document.createElement('a');
        link.href = blobUrl;
        link.download = `relatorio-vendas-${new Date().toISOString().slice(0, 10)}.xlsx`;
        link.click();
        window.URL.revokeObjectURL(blobUrl);
      },
      error: () => {
        this.exportandoExcel = false;
        this.messageService.showError('Não foi possível exportar o relatório em Excel.');
      }
    });
  }

  private carregarVendas(): void {
    this.carregando = true;
    this.pdvService.listarVendas().subscribe({
      next: (vendas) => {
        this.vendas = vendas;
        this.carregando = false;
      },
      error: () => {
        this.carregando = false;
        this.vendas = [];
        this.messageService.showError('Não foi possível carregar o relatório de vendas.');
      }
    });
  }

  private cancelarVenda(venda: VendaDTO): void {
    this.cancelandoVendaId = venda.id;

    this.pdvService.cancelarVenda(venda.id).subscribe({
      next: (response) => {
        this.cancelandoVendaId = null;

        if (!response.success || !response.dados) {
          this.messageService.showError(response.mensagem || 'Não foi possível cancelar a venda.');
          return;
        }

        this.vendas = this.vendas.map((item) => item.id === response.dados!.id ? response.dados! : item);
        this.messageService.showSuccess(`Venda ${response.dados.numeroVenda} cancelada com sucesso.`);
      },
      error: () => {
        this.cancelandoVendaId = null;
        this.messageService.showError('Erro ao cancelar a venda.');
      }
    });
  }
}