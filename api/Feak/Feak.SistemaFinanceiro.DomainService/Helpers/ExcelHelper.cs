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

            var propriedades = typeof(T)
                .GetProperties()
                .Select((property, index) => new
                {
                    Property = property,
                    Index = index,
                    Display = property.GetCustomAttribute<DisplayAttribute>(),
                    DisplayFormat = property.GetCustomAttribute<DisplayFormatAttribute>()
                })
                .OrderBy(x => x.Display?.GetOrder() ?? int.MaxValue)
                .ThenBy(x => x.Index)
                .ToList();

            int linhaCabecalho = 3;
            for (int i = 0; i < propriedades.Count; i++)
            {
                worksheet.Cell(linhaCabecalho, i + 1).Value = propriedades[i].Display?.Name ?? propriedades[i].Property.Name;
            }
            
            var cabecalho = worksheet.Range(linhaCabecalho, 1, linhaCabecalho, propriedades.Count);
            cabecalho.Style.Font.Bold = true;
            cabecalho.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cabecalho.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            int row = linhaCabecalho + 1;
            foreach (var item in dados)
            {
                for (int col = 0; col < propriedades.Count; col++)
                {
                    var configuracaoColuna = propriedades[col];
                    var valor = configuracaoColuna.Property.GetValue(item);
                    var cell = worksheet.Cell(row, col + 1);

                    DefinirValorCelula(cell, valor);

                    if (!string.IsNullOrWhiteSpace(configuracaoColuna.DisplayFormat?.DataFormatString))
                    {
                        cell.Style.NumberFormat.Format = configuracaoColuna.DisplayFormat.DataFormatString;
                    }
                }
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream.ToArray();
        }

        private static void DefinirValorCelula(IXLCell cell, object? valor)
        {
            if (valor == null)
            {
                cell.Value = string.Empty;
                return;
            }

            switch (valor)
            {
                case DateTime dateTime:
                    cell.Value = dateTime;
                    break;
                case DateTimeOffset dateTimeOffset:
                    cell.Value = dateTimeOffset.DateTime;
                    break;
                case decimal decimalValue:
                    cell.Value = decimalValue;
                    break;
                case double doubleValue:
                    cell.Value = doubleValue;
                    break;
                case float floatValue:
                    cell.Value = floatValue;
                    break;
                case int intValue:
                    cell.Value = intValue;
                    break;
                case long longValue:
                    cell.Value = longValue;
                    break;
                case short shortValue:
                    cell.Value = shortValue;
                    break;
                case bool boolValue:
                    cell.Value = boolValue;
                    break;
                default:
                    cell.Value = valor.ToString();
                    break;
            }
        }
    }
}
