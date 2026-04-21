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