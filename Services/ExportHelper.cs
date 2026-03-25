using Chetan_Broker.Models;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Font;
using iText.IO.Font;
using System.Windows;
using TextAlignment = iText.Layout.Properties.TextAlignment;

namespace Chetan_Broker.Services
{
    public static class ExportHelper
    {

        public static void ExportToPdf(
     List<ReportDto> data,
     string partyName,
     string filePath,
     decimal totalBrokerage,
     BrokerAccount broker)
        {
            try
            {
                using var writer = new PdfWriter(filePath);
                using var pdf = new PdfDocument(writer);


                float width = 21.5f * 72f / 2.54f;
                float height = 34.5f * 72f / 2.54f;

                var pageSize = new PageSize(width, height);
                pdf.SetDefaultPageSize(pageSize);

                var document = new Document(pdf, pageSize);
                document.SetMargins(25, 20, 20, 20);

                PdfFont font = PdfFontFactory.CreateFont(
                    @"C:\Windows\Fonts\calibri.TTF",
                    PdfEncodings.WINANSI
                );

                document.SetFont(font);

                // Title
                document.Add(new Paragraph($"{partyName} Brokerage Bill {DateTime.Today.AddDays(-365):yy}-{DateTime.Today:yy}")
                    .SetFontSize(18)
                    .SimulateBold()
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMargin(0));

                // Broker Name
                document.Add(new Paragraph(broker.Name)
                    .SetFontSize(18)
                    .SimulateBold()
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMargin(0));

                // Info Table
                var infoTable = new Table(new float[] { 1, 1 }).UseAllAvailableWidth().SimulateBold();

                Cell InfoCell(string text) =>
                    new Cell().Add(new Paragraph(text).SetMargin(0)).SetPadding(5);

                infoTable.AddCell(InfoCell(broker.PersonName ?? "--"));
                infoTable.AddCell(InfoCell($"Mobile: {broker.MobileNo ?? "--"}"));

                infoTable.AddCell(InfoCell($"Bank: {broker.BankName ?? "--"}"));
                infoTable.AddCell(InfoCell($"A/C: {broker.AccountNumber ?? "--"}"));

                infoTable.AddCell(InfoCell($"IFSC: {broker.IFSCCode ?? "--"}"));
                infoTable.AddCell(InfoCell($"PAN: {broker.PANNo ?? "--"}"));

                infoTable.AddCell(new Cell(1, 2)
                    .Add(new Paragraph($"Address: {broker.Address ?? "--"}").SetMargin(0))
                    .SetPadding(5));

                document.Add(infoTable);

                document.Add(new Paragraph("\n").SetMargin(0));

                // 🔥 Main Table
                var table = new Table(new float[] { 60, 330, 100, 45, 100, 65 })
                    .UseAllAvailableWidth()
                    .SetCharacterSpacing(-0.1f)
                    .SimulateBold()
                    .SetFontSize(11); // ✅ Global font size

                var headerBg = new DeviceRgb(230, 230, 230);

                Cell Header(string text) =>
                    new Cell()
                        .Add(new Paragraph(text)
                            .SetMargin(0)
                            .SetFontSize(13))
                            .SetCharacterSpacing(-0.1f)
                            .SetBackgroundColor(headerBg)
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SimulateBold()
                            .SetPadding(0);

                table.AddHeaderCell(Header("Date"));
                table.AddHeaderCell(Header("Name"));
                table.AddHeaderCell(Header("City"));
                table.AddHeaderCell(Header("Bag\nQty"));
                table.AddHeaderCell(Header("Amount"));
                table.AddHeaderCell(Header("Rs."));

                Cell DataCell(string text) =>
                    new Cell()
                        .Add(new Paragraph(text)
                            .SetMargin(0)
                            .SetTextAlignment(TextAlignment.CENTER))
                        .SetPadding(0);

                foreach (var item in data)
                {
                    string formattedDate = FormatDateManual(item.TransactionDate);

                    string name = item.Name ?? "--";
                    string city = string.IsNullOrWhiteSpace(item.City) ? "--" : item.City;

                    // 🔥 Multi-line Amount
                    string amount = string.Join("\n",
                        (item.Amount ?? ""));

                    // 🔥 Bag (Remarks + Quantity)
                    Paragraph bagText;

                    if (string.IsNullOrWhiteSpace(item.Remarks) || item.Remarks == "--")
                    {
                        bagText = new Paragraph(item.BagQuantity.ToString())
                            .SetMargin(0).SetPadding(0)
                            .SetTextAlignment(TextAlignment.CENTER);
                    }
                    else
                    {
                        bagText = new Paragraph()
                            .Add(new Text(item.Remarks))
                            .Add("\n")
                            .Add(new Text(item.BagQuantity.ToString()))
                            .SetMargin(0).SetPadding(0)
                            .SetTextAlignment(TextAlignment.CENTER);
                    }

                    table.AddCell(DataCell(formattedDate));
                    table.AddCell(DataCell(name));
                    table.AddCell(DataCell(city));
                    table.AddCell(new Cell()
                        .Add(bagText)
                        .SetPadding(0));

                    table.AddCell(DataCell(amount));
                    table.AddCell(DataCell(item.Brokerage ?? "--"));
                }

                document.Add(table);

                // Total
                document.Add(new Paragraph($"Total Brokerage: ₹ {totalBrokerage:N2}")
                    .SimulateBold()
                    .SetFontSize(18)
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetMarginTop(10)
                    .SetMarginBottom(0));

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

        private static string FormatDateManual(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Length < 10)
                return input;

            // Expected: yyyy-MM-dd
            string year = input.Substring(2, 2);   // "26"
            string month = input.Substring(5, 2);  // "03"
            string day = input.Substring(8, 2);    // "26"

            return $"{day}/{month}/{year}";
        }
    }
}