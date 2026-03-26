using Chetan_Broker.Services;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Chetan_Broker.Controls
{
    public partial class HomeView : UserControl, INotifyPropertyChanged
    {
        private readonly TransactionService _service = new TransactionService();
        private List<dynamic> _allData = new();
        private PlotModel _brokeragePieModel;
        private PlotModel _receiverPieModel;

        public event PropertyChangedEventHandler PropertyChanged;

        // View State Property
        private bool _isSenderView = true;
        public bool IsSenderView
        {
            get => _isSenderView;
            set
            {
                _isSenderView = value;
                OnPropertyChanged();
                UpdatePartyListSource(); // Refresh list items when switching views
                ApplyFilters();           // Re-apply filters for the new view
            }
        }

        public PlotModel ReceiverPieModel
        {
            get => _receiverPieModel;
            set { _receiverPieModel = value; OnPropertyChanged(); }
        }

        public PlotModel BrokeragePieModel
        {
            get => _brokeragePieModel;
            set { _brokeragePieModel = value; OnPropertyChanged(); }
        }

        public HomeView()
        {
            InitializeComponent();
            DataContext = this;
            LoadInitialData();
        }

        private void LoadInitialData()
        {
            try
            {
                var raw = _service.GetAllWithNames();
                if (raw == null) return;
                _allData = raw.Cast<dynamic>().ToList();

                // Populate Months (Common for both)
                lstMonths.ItemsSource = _allData
                    .Select(x => SafeDate(x.TransactionDate).ToString("MMMM yyyy"))
                    .Distinct()
                    .OrderByDescending(x => DateTime.ParseExact(x, "MMMM yyyy", null))
                    .ToList();

                UpdatePartyListSource();
                ApplyFilters(isInitialLoad: true);
            }
            catch (Exception ex) { ShowError("Load Error", ex.Message); }
        }

        // Updates the ListBox items based on whether we are looking at Senders or Receivers
        private void UpdatePartyListSource()
        {
            if (_allData == null) return;

            var parties = IsSenderView
                ? _allData.Select(x => (string)(x.SenderName ?? "Unknown"))
                : _allData.Select(x => (string)(x.ReceiverName ?? "Unknown"));

            lstParties.ItemsSource = parties
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }

        private void ApplyFilters(bool isInitialLoad = false)
        {
            var query = _allData.AsEnumerable();

            // 1. Dynamic Party Filter based on Current View
            var selectedParties = lstParties.SelectedItems.Cast<string>().ToList();
            if (selectedParties.Any())
            {
                query = IsSenderView
                    ? query.Where(x => selectedParties.Contains((string)x.SenderName))
                    : query.Where(x => selectedParties.Contains((string)x.ReceiverName));
            }

            // 2. Multi-Month Filter
            var selectedMonths = lstMonths.SelectedItems.Cast<string>().ToList();
            if (selectedMonths.Any())
            {
                query = query.Where(x => selectedMonths.Contains(SafeDate(x.TransactionDate).ToString("MMMM yyyy")));
            }

            var result = query.ToList();

            // 3. Top 10 Logic (Initial Load only)
            if (isInitialLoad && !selectedParties.Any() && !selectedMonths.Any())
            {
                Func<dynamic, string> selector = IsSenderView ? x => x.SenderName : x => x.ReceiverName;

                var top10Names = result
                    .GroupBy(selector)
                    .Select(g => new {
                        Name = g.Key,
                        Total = g.Sum(v => (double)(v.Brokerage ?? 0.0))
                    })
                    .OrderByDescending(x => x.Total)
                    .Take(10)
                    .Select(x => x.Name)
                    .ToList();

                result = result.Where(x => top10Names.Contains(selector(x))).ToList();
            }

            UpdateUI(result);
        }

        private void UpdateUI(List<dynamic> data)
        {
            double totalBrokerage = data.Sum(x => (double)(x.Brokerage ?? 0.0));
            txtTotalBrokerage.Text = $"₹ {totalBrokerage:N2}";

            var row = (IDictionary<string, object>)data[0];
            decimal total = Convert.ToDecimal(row["TotalBrokerage"]);

            txtDatabaseTotal.Text = $"₹ {total:N2}";

            // Update only the model currently in view to save resources, or both
            BrokeragePieModel = CreatePieChart("Brokerage by Sender", data, x => (string)(x.SenderName ?? "Unknown"));
            ReceiverPieModel = CreatePieChart("Brokerage by Receiver", data, x => (string)(x.ReceiverName ?? "Unknown"));
        }

        private PlotModel CreatePieChart(string title, List<dynamic> data, Func<dynamic, string> nameSelector)
        {
            var model = new PlotModel { Title = title, PlotAreaBorderThickness = new OxyThickness(0) };
            var pieSeries = new PieSeries
            {
                StrokeThickness = 1.0,
                InsideLabelPosition = 0.5,
                AngleSpan = 360,
                StartAngle = 0,
                FontSize = 12,
                TrackerFormatString = "{1}: {2:N2} ({3:P1})"
            };

            var chartData = data
                .GroupBy(nameSelector)
                .Select(g => new {
                    Name = g.Key,
                    Total = g.Sum(x => (double)(x.Brokerage ?? 0.0))
                })
                .Where(x => x.Total > 0)
                .OrderByDescending(x => x.Total);

            foreach (var item in chartData)
            {
                pieSeries.Slices.Add(new PieSlice(item.Name, item.Total));
            }

            model.Series.Add(pieSeries);
            return model;
        }

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e) => ApplyFilters();

        private void ResetFilter_Click(object sender, RoutedEventArgs e)
        {
            lstParties.UnselectAll();
            lstMonths.UnselectAll();
            ApplyFilters(isInitialLoad: true);
        }

        // UI Event handlers for the buttons
        private void ViewToggle_Checked(object sender, RoutedEventArgs e)
        {
            // This method ensures that when you click 'Receiver', the IsSenderView property updates
            if (sender is RadioButton rb && rb.Tag != null)
            {
                IsSenderView = rb.Tag.ToString() == "Sender";
            }
        }

        private DateTime SafeDate(object value) => DateTime.TryParse(value?.ToString(), out var dt) ? dt : DateTime.MinValue;
        private void ShowError(string title, string msg) => MessageBox.Show($"{title}\n\n{msg}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}