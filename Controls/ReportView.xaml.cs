using Chetan_Broker.Models;
using Chetan_Broker.Services;
using Microsoft.Win32;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Chetan_Broker.Controls
{
    public partial class ReportView : UserControl
    {
        private PartyService _partyService = new PartyService();
        private TransactionService _transactionService = new TransactionService();
        private List<ReportDto> _reportData = new();
        private decimal _totalBrokerage = 0;
        private BrokrageAccountService _brokerService = new BrokrageAccountService();

        public ReportView()
        {
            InitializeComponent();
            LoadParties();
        }

        private void LoadParties()
        {
            try
            {
                cmbParty.ItemsSource = _partyService.GetAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to load parties.\n\n{ex.Message}",
                    "Load Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void LoadReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var culture = new CultureInfo("en-IN");

                if (cmbParty.SelectedValue == null)
                {
                    MessageBox.Show(
                        "Please select a party.",
                        "Validation",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                int partyId = Convert.ToInt32(cmbParty.SelectedValue);

                var data = _transactionService.GetByParty(partyId);

                _reportData = data;
                dgReport.ItemsSource = _reportData;


                decimal totalBrokerage = data
                .Select(x => (decimal)x.Brokerage)
                .Sum();

                txtTotalBrokerage.Text = totalBrokerage.ToString("N0", culture);

                _totalBrokerage = totalBrokerage;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading report.\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ExportPdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_reportData == null || !_reportData.Any())
                {
                    MessageBox.Show(
                        "No data to export.",
                        "Validation",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                var selectedParty = cmbParty.SelectedItem as Party;

                if (selectedParty == null)
                {
                    MessageBox.Show("Invalid party selected");
                    return;
                }

                // 🔥 Get Broker using BrokerAccountId
                var broker = _brokerService.GetById(selectedParty.BrokerAccountId);

                if (broker == null)
                {
                    MessageBox.Show("Broker not found");
                    return;
                }


                var partyName = (cmbParty.SelectedItem as dynamic)?.Name ?? "Report";

                var dialog = new SaveFileDialog
                {
                    FileName = $"{partyName}_Report.pdf",
                    Filter = "PDF files (*.pdf)|*.pdf",
                    DefaultExt = ".pdf"
                };

                if (dialog.ShowDialog() == true)
                {
                    ExportHelper.ExportToPdf(_reportData, partyName, dialog.FileName, _totalBrokerage, broker);

                    MessageBox.Show(
                        $"PDF exported successfully.\n\nLocation:\n{dialog.FileName}",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
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

        public static decimal ParseBrokerage(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0;

            // Remove "/-" and "." and spaces
            var cleaned = input
                .Replace("/-", "")
                .Replace(".", "")
                .Replace(" ", "")
                .Trim();

            // Remove commas for decimal conversion
            cleaned = cleaned.Replace(",", "");

            return decimal.TryParse(cleaned, out var value) ? value : 0;
        }
    }
}