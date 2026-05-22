using Application.Dtos;
using Application.Interfaces;
using ClosedXML.Excel;
using System.Globalization;

namespace Infrastructure.FileStorage;

public class ExcelParserService : IExcelParserService
{
    public IEnumerable<DemandPlanDto> ParseDemandExcel(Stream fileStream)
    {
        var demandPlans = new List<DemandPlanDto>();
        using var workbook = new XLWorkbook(fileStream);

        foreach (var worksheet in workbook.Worksheets)
        {
            var lastRowUsed = worksheet.LastRowUsed();
            if (lastRowUsed == null) continue;

            int rowCount = lastRowUsed.RowNumber();
            int colCount = worksheet.LastColumnUsed()?.ColumnNumber() ?? 50;

            int headerRow = -1;
            var columnMap = new Dictionary<string, int>();

            for (int r = 1; r <= Math.Min(rowCount, 30); r++)
            {
                for (int c = 1; c <= colCount; c++)
                {
                    var cellText = worksheet.Cell(r, c).GetString().Trim().ToUpper();
                    if (cellText.Contains("MODELO") || cellText.Contains("NO PARTE"))
                    {
                        headerRow = r;
                        break;
                    }
                }

                if (headerRow != -1)
                {
                    for (int c = 1; c <= colCount; c++)
                    {
                        var headerText = worksheet.Cell(headerRow, c).GetString().Trim().ToUpper();
                        if (!string.IsNullOrEmpty(headerText) && !columnMap.ContainsKey(headerText))
                        {
                            columnMap.Add(headerText, c);
                        }
                    }
                    break;
                }
            }

            if (headerRow == -1) continue;

            string currentLine = worksheet.Cell("C1").GetString();
            string currentDateText = worksheet.Cell("F2").GetString();
            DateTime.TryParse(currentDateText, out DateTime productionDate);
            if (productionDate == default) productionDate = DateTime.Now.Date;

            string lastModel = string.Empty;

            for (int r = headerRow + 1; r <= rowCount; r++)
            {
                int noParteCol = GetColumnIndex(columnMap, "NO PARTE", "PARTE", "N/P");
                int modeloCol = GetColumnIndex(columnMap, "MODELO");
                int descCol = GetColumnIndex(columnMap, "DESCRIPCION", "DESC");
                int cantXUnidCol = GetColumnIndex(columnMap, "UNID", "CANT X UNID");
                int totalCol = GetColumnIndex(columnMap, "TOTAL"); // Volvemos a mapear la columna TOTAL

                var partNumber = noParteCol > 0 ? worksheet.Cell(r, noParteCol).GetString().Trim() :
                                 modeloCol > 0 ? worksheet.Cell(r, modeloCol).GetString().Trim() : string.Empty;

                if (string.IsNullOrEmpty(partNumber)) continue;

                var cantXUnidText = cantXUnidCol > 0 ? worksheet.Cell(r, cantXUnidCol).GetString().Trim() : string.Empty;
                var totalText = totalCol > 0 ? worksheet.Cell(r, totalCol).GetString().Trim() : string.Empty;

                bool isParent = false;
                int quantity = 0;

                // Evaluamos si es la cabecera del ensamble
                if (cantXUnidText.ToUpper().Contains("KIT") || cantXUnidText.ToUpper().Contains("SINGLE") || cantXUnidText.ToUpper().Contains("DUAL"))
                {
                    lastModel = partNumber;
                    isParent = true;
                    // El padre tiene su cantidad en la columna TOTAL
                    quantity = ParseQuantity(totalText);
                }
                else
                {
                    quantity = ParseQuantity(cantXUnidText);
                    if (quantity == 0) quantity = ParseQuantity(totalText);
                }

                demandPlans.Add(new DemandPlanDto
                {
                    PartNumber = partNumber,
                    Quantity = quantity,
                    ShopOrder = string.Empty,
                    Model = isParent ? partNumber : lastModel, // Si es el padre, su modelo es él mismo
                    Description = descCol > 0 ? worksheet.Cell(r, descCol).GetString().Trim() : string.Empty,
                    LineName = currentLine,
                    ProductionDate = productionDate
                });
            }
        }

        return demandPlans;
    }

    private int GetColumnIndex(Dictionary<string, int> map, params string[] keywords)
    {
        foreach (var kvp in map)
        {
            foreach (var keyword in keywords)
            {
                if (kvp.Key.Contains(keyword)) return kvp.Value;
            }
        }
        return 0;
    }

    private int ParseQuantity(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return 0;

        text = text.Replace(",", ".");
        string cleanNumber = new string(text.Where(c => char.IsDigit(c) || c == '.').ToArray());

        if (double.TryParse(cleanNumber, NumberStyles.Any, CultureInfo.InvariantCulture, out double qtyDouble))
        {
            return (int)Math.Round(qtyDouble);
        }

        return 0;
    }
}