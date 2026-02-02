using ClosedXML.Excel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Feak.SistemaFinanceiro.DomainService.Helpers
{
    public static class ExcelHelper<T>
    {
        public static byte[] ExportarExcel(IEnumerable<T> dados, string titulo = "Planilha")
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(titulo);

            worksheet.Cell(1, 1).Value = "Data de Emissão";
            worksheet.Cell(1, 2).Value = DateTime.Now.AddHours(-3).ToString("dd/MM/yyyy");

            worksheet.Cell(2, 1).Value = "Hora";
            worksheet.Cell(2, 2).Value = DateTime.Now.AddHours(-3).ToString("HH:mm");

            worksheet.Range("A1:A2").Style.Font.Bold = true;
            worksheet.Range("A1:B2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

            var propriedades = typeof(T).GetProperties();

            int linhaCabecalho = 3;
            for (int i = 0; i < propriedades.Length; i++)
            {
                var displayAttr = propriedades[i].GetCustomAttribute<DisplayAttribute>();
                worksheet.Cell(linhaCabecalho, i + 1).Value = displayAttr?.Name ?? propriedades[i].Name;
            }
            
            var cabecalho = worksheet.Range(linhaCabecalho, 1, linhaCabecalho, propriedades.Length);
            cabecalho.Style.Font.Bold = true;
            cabecalho.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cabecalho.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            int row = linhaCabecalho + 1;
            foreach (var item in dados)
            {
                for (int col = 0; col < propriedades.Length; col++)
                {
                    var valor = propriedades[col].GetValue(item);
                    worksheet.Cell(row, col + 1).SetValue(valor?.ToString() ?? string.Empty);
                }
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream.ToArray();
        }
    }
}
