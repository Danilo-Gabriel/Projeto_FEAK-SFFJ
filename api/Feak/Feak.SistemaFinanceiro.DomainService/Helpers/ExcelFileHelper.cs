using Microsoft.AspNetCore.Mvc;

namespace Feak.SistemaFinanceiro.DomainService.Helpers;

public static class ExcelFileHelper
{
    public static FileContentResult CriarArquivo<T>(IEnumerable<T> dados, string nomeArquivo, string tituloPlanilha)
    {
        var conteudoExcel = ExcelHelper<T>.ExportarExcel(dados, tituloPlanilha);
        
        return new FileContentResult(conteudoExcel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = FormatarNomeAquivo(nomeArquivo)
        };
    }


    public static string FormatarNomeAquivo(string nomeArquivo)
    {
        var fileName = nomeArquivo + " " + DateTime.UtcNow.AddHours(-3).ToString("ddMMyyyy_HHmm") + ".xlsx";
        return fileName.Replace(" ", "_");
    }
}