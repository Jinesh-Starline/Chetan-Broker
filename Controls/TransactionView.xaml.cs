using Chetan_Broker.Data;
using Chetan_Broker.Models;
using Chetan_Broker.Services;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chetan_Broker.Controls
{
    /// <summary>
    /// Interaction logic for TransactionView.xaml
    /// </summary>
    public partial class TransactionView : UserControl
    {
        private PartyService _partyService = new PartyService();
        private TransactionService _transactionService = new TransactionService();
        private int? _selectedTransactionId = null;
        public TransactionView()
        {
            InitializeComponent();
            dpTransactionDate.SelectedDate = DateTime.Today;
            dpTransactionDate.DisplayDateEnd = DateTime.Today;
            LoadParties();
            LoadTransactions();
        }

        private void LoadParties()
        {
            var parties = _partyService.GetAll();

            cmbSenderParty.ItemsSource = parties;
            cmbReceiverParty.ItemsSource = parties;
        }

        private void AddTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (cmbSenderParty.SelectedItem == null)
            {   
                MessageBox.Show("Please select Sender Party");
                return;
            }

            if (cmbReceiverParty.SelectedItem == null)
            {
                MessageBox.Show("Please select Receiver Party");
                return;
            }

            if (cmbSenderParty.SelectedItem == cmbReceiverParty.SelectedItem)
            {
                MessageBox.Show("Sender and Receiver cannot be same");
                return;
            }

            if (!decimal.TryParse(txtTransactionAmount.Text, out var amount))
            {
                MessageBox.Show("Please enter valid Amount");
                return;
            }

            if (amount <= 0)
            {
                MessageBox.Show("Amount must be greater than 0");
                return;
            }

            if (!decimal.TryParse(txtBrokerageAmount.Text, out var brokerage))
            {
                MessageBox.Show("Please enter valid Brokerage");
                return;
            }

            if (brokerage < 0)
            {
                MessageBox.Show("Brokerage cannot be negative");
                return;
            }

            if (dpTransactionDate.SelectedDate == null)
            {
                MessageBox.Show("Please select a date");
                return;
            }

            if (dpTransactionDate.SelectedDate > DateTime.Today)
            {
                MessageBox.Show("Future date not allowed");
                return;
            }

            var model = new TransactionModel
            {
                SenderId = Convert.ToInt32(cmbSenderParty.SelectedValue),
                ReceiverId = Convert.ToInt32(cmbReceiverParty.SelectedValue),
                TransactionDate = dpTransactionDate.SelectedDate.Value.ToString("yyyy-MM-dd"),
                Amount = amount,
                Brokerage = brokerage
            };

            if (_selectedTransactionId == null)
            {
                _transactionService.Add(model);
                MessageBox.Show("Transaction Added");
            }
            else
            {
                _transactionService.Update(_selectedTransactionId.Value, model);
                MessageBox.Show("Transaction Updated");
            }

            ClearForm();
            LoadTransactions();
        }

        private void LoadTransactions()
        {
            dgTransactions.ItemsSource = _transactionService.GetAllWithNames();
        }

        private void NumberOnly(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;

            string fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            e.Handled = !decimal.TryParse(fullText, out _);
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var text = (string)e.DataObject.GetData(typeof(string));

                if (!decimal.TryParse(text, out _))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void dgTransactions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic item = dgTransactions.SelectedItem;

            if (item == null) return;

            _selectedTransactionId = (int?)item.Id;

            cmbSenderParty.SelectedValue = item.SenderId;
            cmbReceiverParty.SelectedValue = item.ReceiverId;

            dpTransactionDate.SelectedDate = DateTime.Parse(item.TransactionDate);

            txtTransactionAmount.Text = item.Amount.ToString();
            txtBrokerageAmount.Text = item.Brokerage.ToString();
        }

        private void ClearForm()
        {
            _selectedTransactionId = null;

            cmbSenderParty.SelectedIndex = -1;
            cmbReceiverParty.SelectedIndex = -1;

            dpTransactionDate.SelectedDate = DateTime.Today;

            txtTransactionAmount.Clear();
            txtBrokerageAmount.Clear();
        }

        private void DeleteTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTransactionId == null)
            {
                MessageBox.Show("Select transaction first");
                return;
            }

            var result = MessageBox.Show(
                "Are you sure you want to delete this transaction?",
                "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            _transactionService.Delete(_selectedTransactionId.Value);

            MessageBox.Show("Transaction deleted");

            ClearForm();
            LoadTransactions();
        }
    }

}
