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
    const janelaRecibo = window.open('', '_blank', 'width=420,height=760');

    if (!janelaRecibo) {
      this.messageService.showError('Não foi possível abrir o recibo. Verifique se o bloqueador de pop-up está desativado.');
      return;
    }

    const itensHtml = venda.itens.length
      ? venda.itens.map((item) => `
          <tr>
            <td>${item.quantidade}x</td>
            <td>${item.descricaoProduto}</td>
            <td>${this.formatarMoeda(item.precoUnitario)}</td>
            <td>${this.formatarMoeda(item.totalItem)}</td>
          </tr>
        `).join('')
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
          body { font-family: Arial, sans-serif; color: #0f172a; margin: 16px; }
          .topo { text-align: center; margin-bottom: 16px; }
          .topo h1 { font-size: 18px; margin: 0; }
          .topo p { margin: 4px 0 0; font-size: 12px; color: #475569; }
          .bloco { margin-bottom: 14px; }
          .linha { display: flex; justify-content: space-between; font-size: 12px; margin: 4px 0; gap: 12px; }
          .linha strong { font-size: 13px; }
          table { width: 100%; border-collapse: collapse; font-size: 12px; }
          th, td { border-bottom: 1px solid #cbd5e1; padding: 6px 4px; text-align: left; }
          th { background: #f8fafc; }
          .totais { margin-top: 14px; border-top: 1px dashed #64748b; padding-top: 10px; }
          .status { display: inline-block; padding: 3px 8px; border-radius: 999px; font-size: 11px; font-weight: bold; }
          .status.fechada { background: #dcfce7; color: #166534; }
          .status.cancelada { background: #fee2e2; color: #991b1b; }
          @media print { .print-action { display: none; } body { margin: 0; } }
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

        <button class="print-action" onclick="window.print()" style="width:100%;margin-top:16px;padding:10px;border:0;border-radius:8px;background:#1d4ed8;color:#fff;font-weight:700;cursor:pointer;">Imprimir recibo</button>
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