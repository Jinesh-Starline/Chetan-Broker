using Chetan_Broker.Services;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Chetan_Broker.Controls
{
    public partial class HomeView : UserControl
    {
        private TransactionService _service = new TransactionService();

        public PlotModel AmountModel { get; set; }
        public PlotModel BrokerageModel { get; set; }

        public HomeView()
        {
            try
            {
                InitializeComponent();
                LoadChart();
                DataContext = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error initializing dashboard.\n\n{ex.Message}",
                    "Dashboard Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void LoadChart()
        {
            try
            {
                var data = _service.GetAllWithNames();

                // 🔹 Amount Chart
                var amountModel = new PlotModel { Title = "Amount" };

                var amountSeries = new BarSeries();

                var categoryAxis1 = new CategoryAxis
                {
                    Position = AxisPosition.Left
                };

                var valueAxis1 = new LinearAxis
                {
                    Position = AxisPosition.Bottom
                };

                foreach (var item in data)
                {
                    amountSeries.Items.Add(new BarItem { Value = (double)item.Amount });
                    categoryAxis1.Labels.Add(item.Name ?? "--");
                }

                amountModel.Axes.Add(categoryAxis1);
                amountModel.Axes.Add(valueAxis1);
                amountModel.Series.Add(amountSeries);

                AmountModel = amountModel;


                // 🔹 Brokerage Chart
                var brokerageModel = new PlotModel { Title = "Brokerage" };

                var brokerageSeries = new LineSeries();

                var categoryAxis2 = new CategoryAxis
                {
                    Position = AxisPosition.Bottom
                };

                var valueAxis2 = new LinearAxis
                {
                    Position = AxisPosition.Left
                };

                int i = 1;
                foreach (var item in data)
                {
                    brokerageSeries.Points.Add(new DataPoint(i, (double)item.Brokerage));
                    categoryAxis2.Labels.Add(item.Name ?? $"T{i++}");
                }

                brokerageModel.Series.Add(brokerageSeries);
                brokerageModel.Axes.Add(categoryAxis2);
                brokerageModel.Axes.Add(valueAxis2);

                BrokerageModel = brokerageModel;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading chart data.\n\n{ex.Message}",
                    "Chart Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}