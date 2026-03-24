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
            try
            {
                var parties = _partyService.GetAll();

                cmbSenderParty.ItemsSource = parties;
                cmbReceiverParty.ItemsSource = parties;
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
        private void AddTransaction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbSenderParty.SelectedItem == null)
                {
                    MessageBox.Show("Please select Sender Party",
                        "Validation",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (cmbReceiverParty.SelectedItem == null)
                {
                    MessageBox.Show("Please select Receiver Party",
                        "Validation",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (cmbSenderParty.SelectedItem == cmbReceiverParty.SelectedItem)
                {
                    MessageBox.Show("Sender and Receiver cannot be same",
                        "Validation",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(txtTransactionAmount.Text, out var amount))
                {
                    MessageBox.Show("Please enter valid Amount",
                        "Validation",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (amount <= 0)
                {
                    MessageBox.Show("Amount must be greater than 0",
                        "Validation",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(txtBrokerageAmount.Text, out var brokerage))
                {
                    MessageBox.Show("Please enter valid Brokerage",
                        "Validation",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (brokerage < 0)
                {
                    MessageBox.Show("Brokerage cannot be negative",
                        "Validation",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (dpTransactionDate.SelectedDate == null)
                {
                    MessageBox.Show("Please select a date",
                        "Validation",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (dpTransactionDate.SelectedDate > DateTime.Today)
                {
                    MessageBox.Show("Future date not allowed",
                        "Validation",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                int bagQty = 0;
                if (!string.IsNullOrWhiteSpace(txtBagQuantity.Text))
                    int.TryParse(txtBagQuantity.Text, out bagQty);

                var model = new TransactionModel
                {
                    SenderId = Convert.ToInt32(cmbSenderParty.SelectedValue),
                    ReceiverId = Convert.ToInt32(cmbReceiverParty.SelectedValue),
                    TransactionDate = dpTransactionDate.SelectedDate.Value.ToString("yyyy-MM-dd"),
                    Amount = amount,
                    Brokerage = brokerage,
                    BagQuantity = bagQty,
                    Remarks = txtRemarks.Text
                };

                if (_selectedTransactionId == null)
                {
                    _transactionService.Add(model);
                    MessageBox.Show("Transaction added successfully",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    _transactionService.Update(_selectedTransactionId.Value, model);
                    MessageBox.Show("Transaction updated successfully",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }

                ClearForm();
                LoadTransactions();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error saving transaction.\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void LoadTransactions()
        {
            try
            {
                dgTransactions.ItemsSource = _transactionService.GetAllWithNames();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to load transactions.\n\n{ex.Message}",
                    "Load Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
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
            try
            {
                dynamic item = dgTransactions.SelectedItem;

                if (item == null) return;

                _selectedTransactionId = (int?)item.Id;

                cmbSenderParty.SelectedValue = item.SenderId;
                cmbReceiverParty.SelectedValue = item.ReceiverId;

                dpTransactionDate.SelectedDate = DateTime.Parse(item.TransactionDate);

                txtTransactionAmount.Text = item.Amount.ToString();
                txtBrokerageAmount.Text = item.Brokerage.ToString();
                txtBagQuantity.Text = item.BagQuantity.ToString();
                txtRemarks.Text = item.Remarks;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error selecting transaction.\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ClearForm()
        {
            _selectedTransactionId = null;

            cmbSenderParty.SelectedIndex = -1;
            cmbReceiverParty.SelectedIndex = -1;

            dpTransactionDate.SelectedDate = DateTime.Today;

            txtTransactionAmount.Clear();
            txtBrokerageAmount.Clear();

            txtBagQuantity.Clear();
            txtRemarks.Clear();
        }

        private void DeleteTransaction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedTransactionId == null)
                {
                    MessageBox.Show("Select transaction first",
                        "Validation",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    "Are you sure you want to delete this transaction?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                    return;

                _transactionService.Delete(_selectedTransactionId.Value);

                MessageBox.Show("Transaction deleted successfully",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                ClearForm();
                LoadTransactions();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error deleting transaction.\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }

}
