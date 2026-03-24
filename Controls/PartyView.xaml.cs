using Chetan_Broker.Models;
using Chetan_Broker.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Chetan_Broker.Controls
{
    public partial class PartyView : UserControl
    {
        private PartyService _partyService = new PartyService();
        private BrokrageAccountService _brokerService = new BrokrageAccountService();
        private Party _selectedParty;

        public PartyView()
        {
            InitializeComponent();
            LoadParties();
        }

        private void LoadParties()
        {
            try
            {
                lstParties.ItemsSource = _partyService.GetAll();
                cmbBrokerAccount.ItemsSource = _brokerService.GetAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data:\n{ex.Message}","Problem",MessageBoxButton.OK,MessageBoxImage.Information);
            }
        }

        private void AddParty_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtPartyName.Text))
                {
                    MessageBox.Show("Enter party name", "Problem", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtCity.Text))
                {
                    MessageBox.Show("Enter city", "Problem", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (cmbBrokerAccount.SelectedValue == null)
                {
                    MessageBox.Show("Select broker account", "Problem", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                _partyService.Add(new Party
                {
                    Name = txtPartyName.Text,
                    BrokerAccountId = Convert.ToInt32(cmbBrokerAccount.SelectedValue),
                    City = txtCity.Text
                });

                txtPartyName.Clear();
                txtCity.Clear();
                cmbBrokerAccount.SelectedIndex = -1;

                LoadParties();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding party:\n{ex.Message}", "Problem", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void lstParties_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _selectedParty = lstParties.SelectedItem as Party;

                if (_selectedParty != null)
                {
                    txtPartyName.Text = _selectedParty.Name;
                    cmbBrokerAccount.SelectedValue = _selectedParty.BrokerAccountId;
                    txtCity.Text = _selectedParty.City;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting party:\n{ex.Message}", "Problem", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void UpdateParty_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedParty == null)
                {
                    MessageBox.Show("Select party first", "Problem", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPartyName.Text))
                {
                    MessageBox.Show("Enter party name", "Problem", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtCity.Text))
                {
                    MessageBox.Show("Enter city", "Problem", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (cmbBrokerAccount.SelectedValue == null)
                {
                    MessageBox.Show("Select broker account", "Problem", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                _partyService.Update(
                    _selectedParty.Id,
                    txtPartyName.Text,
                    Convert.ToInt32(cmbBrokerAccount.SelectedValue),
                    txtCity.Text
                );

                txtPartyName.Clear();
                txtCity.Clear();
                cmbBrokerAccount.SelectedIndex = -1;
                _selectedParty = null;

                LoadParties();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating party:\n{ex.Message}", "Problem", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteParty_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedParty == null)
                {
                    MessageBox.Show("Select party first", "Problem", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var result = MessageBox.Show(
                    "Are you sure you want to delete this party?",
                    "Confirm",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                    return;

                var success = _partyService.Delete(_selectedParty.Id);

                if (!success)
                {
                    MessageBox.Show("Cannot delete. Party is used in transactions.", "Problem", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                txtPartyName.Clear();
                txtCity.Clear();
                cmbBrokerAccount.SelectedIndex = -1;
                _selectedParty = null;

                LoadParties();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting party:\n{ex.Message}", "Problem", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}