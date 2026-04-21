import { Component, OnInit } from '@angular/core';
import { PdvService } from '../pdv/services/pdv.service';
import { VendaDTO } from '../../models/dto/venda-dto';
import { UsuarioLogadoDTO } from '../../models/dto/usuario-logado-dto';
import { AuthSessionService } from '../../core/services/auth-session.service';
import { AppMessageService } from '../../shared/services/app-message.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  public dataInicial: string = '';
  public dataFinal: string = '';
  public carregando: boolean = false;
  public usuarioLogado: UsuarioLogadoDTO | null = null;

  public faturamento: number = 0;

  private vendas: VendaDTO[] = [];

  constructor(
    private readonly pdvService: PdvService,
    private readonly authSessionService: AuthSessionService,
    private readonly messageService: AppMessageService
  ) {}

  ngOnInit(): void {
    const hoje = new Date();
    const primeiroDiaMes = new Date(hoje.getFullYear(), hoje.getMonth(), 1);

    this.dataInicial = this.formatarDataInput(primeiroDiaMes);
    this.dataFinal = this.formatarDataInput(hoje);
    this.usuarioLogado = this.authSessionService.obterUsuarioLogado();

    this.authSessionService.usuarioLogado$.subscribe((usuario) => {
      this.usuarioLogado = usuario;
    });

    this.carregarVendas();
  }

  pesquisar(): void {
    this.aplicarFiltro();
  }

  obterNomeUsuario(): string {
    return this.usuarioLogado?.nomeCompleto
      || this.usuarioLogado?.nomeLogin
      || this.usuarioLogado?.email
      || 'Usuário logado';
  }

  obterIniciaisUsuario(): string {
    const nome = this.obterNomeUsuario();
    return nome
      .split(' ')
      .filter(Boolean)
      .slice(0, 2)
      .map((parte) => parte[0]?.toUpperCase())
      .join('');
  }

  private carregarVendas(): void {
    this.carregando = true;

    this.pdvService.listarVendas().subscribe({
      next: (vendas) => {
        this.carregando = false;
        this.vendas = (vendas ?? []).map((venda) => ({
          ...venda,
          dhInclusao: new Date(venda.dhInclusao),
          dhCancelamento: venda.dhCancelamento ? new Date(venda.dhCancelamento) : null
        }));
        this.aplicarFiltro();
      },
      error: () => {
        this.carregando = false;
        this.messageService.showError('Não foi possível carregar os dados do dashboard.');
      }
    });
  }

  private aplicarFiltro(): void {
    const inicio = this.dataInicial ? new Date(`${this.dataInicial}T00:00:00`) : null;
    const fim = this.dataFinal ? new Date(`${this.dataFinal}T23:59:59`) : null;

    const vendasFiltradas = this.vendas
      .filter((venda) => !venda.cancelada)
      .filter((venda) => {
        const dataVenda = new Date(venda.dhInclusao);

        if (inicio && dataVenda < inicio) {
          return false;
        }

        if (fim && dataVenda > fim) {
          return false;
        }

        return true;
      });

    this.faturamento = vendasFiltradas.reduce((total, venda) => total + Number(venda.total ?? 0), 0);
  }

  private formatarDataInput(data: Date): string {
    const ano = data.getFullYear();
    const mes = `${data.getMonth() + 1}`.padStart(2, '0');
    const dia = `${data.getDate()}`.padStart(2, '0');
    return `${ano}-${mes}-${dia}`;
  }
}