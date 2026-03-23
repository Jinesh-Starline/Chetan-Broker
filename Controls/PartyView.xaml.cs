using Chetan_Broker.Models;
using Chetan_Broker.Services;
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
    /// Interaction logic for PartyView.xaml
    /// </summary>
    public partial class PartyView : UserControl
    {
        private PartyService _partyService = new PartyService();
        private Party _selectedParty;
        public PartyView()
        {
            InitializeComponent();
            LoadParties();
        }

        private void LoadParties()
        {
            lstParties.ItemsSource = _partyService.GetAll();
        }

        private void AddParty_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPartyName.Text))
                return;

            _partyService.Add(txtPartyName.Text);
            txtPartyName.Clear();

            LoadParties();
        }

        private void lstParties_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedParty = lstParties.SelectedItem as Party;

            if (_selectedParty != null)
            {
                txtPartyName.Text = _selectedParty.Name;
            }
        }

        private void UpdateParty_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedParty == null)
            {
                MessageBox.Show("Select party first");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPartyName.Text))
            {
                MessageBox.Show("Enter party name");
                return;
            }

            _partyService.Update(_selectedParty.Id, txtPartyName.Text);

            txtPartyName.Clear();
            _selectedParty = null;

            LoadParties();
        }

        private void DeleteParty_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedParty == null)
            {
                MessageBox.Show("Select party first");
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
                MessageBox.Show("Cannot delete. Party is used in transactions.");
                return;
            }

            txtPartyName.Clear();
            _selectedParty = null;

            LoadParties();
        }
    }
}
