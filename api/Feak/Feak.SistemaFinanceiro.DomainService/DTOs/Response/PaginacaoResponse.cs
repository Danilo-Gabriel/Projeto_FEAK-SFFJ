namespace DomainService.DTOs;

public class PaginacaoResponse<T>
{
    public int PaginaAtual { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalItens { get; set; }
    public int TotalPaginas => (int)Math.Ceiling((double)TotalItens / TamanhoPagina);
    public IEnumerable<T> Itens { get; set; }

    public PaginacaoResponse(IEnumerable<T> itens, int paginaAtual, int tamanhoPagina, int totalItens)
    {
        Itens = itens;
        PaginaAtual = paginaAtual;
        TamanhoPagina = tamanhoPagina;
        TotalItens = totalItens;
    }

    public PaginacaoResponse() {}
}