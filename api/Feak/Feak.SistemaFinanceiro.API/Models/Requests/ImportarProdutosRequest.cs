using Microsoft.AspNetCore.Http;

namespace Feak.SistemaFinanceiro.API.Models.Requests;

public class ImportarProdutosRequest
{
    public IFormFile? Arquivo { get; set; }
}