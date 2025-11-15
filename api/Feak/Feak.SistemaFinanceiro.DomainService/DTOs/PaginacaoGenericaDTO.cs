namespace DomainService.DTOs;

public class PaginacaoGenericaDTO
{
    public int PaginaAtual { get; set; }
    
    public int TamanhoPagina { get; set; }

    public PaginacaoGenericaDTO(int paginaAtual, int tamanhoPagina)
    {
        PaginaAtual = paginaAtual <= 0 ? 1 : paginaAtual;
        TamanhoPagina = tamanhoPagina <= 0 ? 10 : tamanhoPagina;
    }
}