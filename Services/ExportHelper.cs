using Chetan_Broker.Models;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Windows;
using TextAlignment = iText.Layout.Properties.TextAlignment;

namespace Chetan_Broker.Services
{
    public static class ExportHelper
    {
        public static void ExportToPdf(List<ReportDto> data, string partyName, string filePath, decimal totalBrokerage,BrokerAccount broker)
        {
            try
            {
                using var writer = new PdfWriter(filePath);
                using var pdf = new PdfDocument(writer);

                // Page Size (Portrait 21.5 x 34.5 cm)
                float width = 21.5f * 72f / 2.54f;
                float height = 34.5f * 72f / 2.54f;

                var pageSize = new PageSize(width, height);
                pdf.SetDefaultPageSize(pageSize);

                var document = new Document(pdf, pageSize);
                document.SetMargins(25, 20, 20, 20);

                // Title
                document.Add(new Paragraph($"{partyName} Brokrage Bill {DateTime.Today.AddDays(-365).ToString("yy")}-{DateTime.Today.ToString("yy")}")
                    .SetFontSize(18)
                    .SimulateBold()
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(5));


                // 🔥 Broker Name (center)
                document.Add(new Paragraph(broker.Name)
                    .SetFontSize(18)
                    .SimulateBold()
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(5));


                // 🔥 Info Table (2 columns)
                var infoTable = new Table(new float[] { 1, 1 });
                infoTable.UseAllAvailableWidth();
                infoTable.SetFontSize(16);

                // Row 1
                infoTable.AddCell(new Paragraph($"{broker.PersonName ?? "--"}"));
                infoTable.AddCell(new Paragraph($"Mobile: {broker.MobileNo ?? "--"}"));

                // Row 2
                infoTable.AddCell(new Paragraph($"Bank: {broker.BankName ?? "--"}"));
                infoTable.AddCell(new Paragraph($"A/C: {broker.AccountNumber ?? "--"}"));

                // Row 3
                infoTable.AddCell(new Paragraph($"IFSC: {broker.IFSCCode ?? "--"}"));
                infoTable.AddCell(new Paragraph($"PAN: {broker.PANNo ?? "--"}"));

                // 🔥 Address (full width row)
                infoTable.AddCell(new Cell(1, 2)
                    .Add(new Paragraph($"Address: {broker.Address ?? "--"}")));

                document.Add(infoTable);

                // spacing
                document.Add(new Paragraph("\n"));


                // Table
                var table = new Table(new float[] { 70, 260, 80, 70, 80, 90 });
                table.UseAllAvailableWidth();
                table.SetFontSize(13f);
                table.SimulateBold();

                var headerBg = new DeviceRgb(230, 230, 230);

                var headers = new List<Cell>
                {
                    new Cell().Add(new Paragraph("Date")),
                    new Cell().Add(new Paragraph("Name")),
                    new Cell().Add(new Paragraph("Rate")),
    
                    // 🔥 Multi-line header
                    new Cell().Add(new Paragraph("Bag\nQuantity")),

                    new Cell().Add(new Paragraph("Remarks")),
                    new Cell().Add(new Paragraph("Rs."))
                };

                foreach (var cell in headers)
                {
                    cell.SetBackgroundColor(headerBg)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SimulateBold();

                    table.AddHeaderCell(cell);
                }

                // Rows
                foreach (var item in data)
                {
                    DateTime dt;
                    string formattedDate = DateTime.TryParse(item.TransactionDate, out dt)
                        ? dt.ToString("dd-MM-yy")
                        : item.TransactionDate;

                    table.AddCell(new Paragraph(formattedDate)
                        .SetTextAlignment(TextAlignment.CENTER));

                    table.AddCell(new Paragraph(item.Name ?? "--")
                        .SetTextAlignment(TextAlignment.CENTER));


                    table.AddCell(new Paragraph(FormatIndianCurrency(item.Amount))
                        .SetTextAlignment(TextAlignment.CENTER));

                    table.AddCell(new Paragraph(item.BagQuantity.ToString())
                        .SetTextAlignment(TextAlignment.CENTER));

                    table.AddCell(new Paragraph(item.Remarks ?? "--").SetTextAlignment(TextAlignment.CENTER));

                    table.AddCell(new Paragraph($"{FormatIndianCurrency(item.Brokerage)}."))
                        .SetTextAlignment(TextAlignment.CENTER);
                }

                document.Add(table);

                // Total
                document.Add(new Paragraph($"Total Brokerage: ₹ {totalBrokerage:N2}")
                    .SimulateBold()
                    .SetFontSize(18)
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetMarginTop(10));

                document.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error exporting PDF.\n\n{ex.Message}",
                    "Export Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private static string FormatIndianCurrency(decimal? value)
        {
            if (value == null) return "0/-";

            return string.Format(
                new System.Globalization.CultureInfo("en-IN"),
                "{0:N0}/-",
                value
            );
        }
    }
}