using Chetan_Broker.Controls;
using Chetan_Broker.Services;
using iText.Kernel.Utils.Annotationsflattening;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Chetan_Broker
{
    public partial class MainWindow : Window
    {
        private UserService _userService = new UserService();
        private string _currentUser = string.Empty;

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                var user = _userService.GetAutoLoginUser();

                if (user != null)
                {
                    _currentUser = user;

                    LoginSection.Visibility = Visibility.Collapsed;
                    MainSection.Visibility = Visibility.Visible;


                    // 🔥 THIS WAS MISSING
                    MainContent.Content = new HomeView();
                    SetActive(btnHome);


                    this.WindowState = WindowState.Maximized;
                }
                else
                {
                    txtUsername.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error initializing application.\n\n{ex.Message}",
                    "Startup Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var username = txtUsername.Text;
                var password = txtPassword.Password;

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show(
                        "Please enter username and password.",
                        "Validation",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                var isValid = _userService.Validate(username, password);

                if (!isValid)
                {
                    MessageBox.Show(
                        "Invalid credentials.",
                        "Authentication Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                _userService.SetAutoLogin(username, chkAutoLogin.IsChecked == true);

                _currentUser = username;

                LoginSection.Visibility = Visibility.Collapsed;
                MainSection.Visibility = Visibility.Visible;

                this.WindowState = WindowState.Maximized;

                // 🔥 THIS WAS MISSING
                MainContent.Content = new HomeView();
                SetActive(btnHome);

                MessageBox.Show(
                    "Login successful.",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error during login.\n\n{ex.Message}",
                    "Login Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _userService.SetAutoLogin(_currentUser, false);

                _currentUser = string.Empty;

                MainSection.Visibility = Visibility.Collapsed;
                LoginSection.Visibility = Visibility.Visible;

                this.WindowState = WindowState.Normal;

                MessageBox.Show(
                    "Logged out successfully.",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error during logout.\n\n{ex.Message}",
                    "Logout Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new ChangePasswordWindow(_currentUser);
                window.Owner = this;
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error opening change password window.\n\n{ex.Message}",
                    "UI Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ShowParty_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainContent.Content = new PartyView();
                SetActive(btnParty);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading Party view.\n\n{ex.Message}",
                    "UI Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ShowTransaction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainContent.Content = new TransactionView();
                SetActive(btnTransaction);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading Transaction view.\n\n{ex.Message}",
                    "UI Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ShowReports_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainContent.Content = new ReportView();
                SetActive(btnReports);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading Reports view.\n\n{ex.Message}",
                    "UI Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void SetActive(Button activeBtn)
        {
            try
            {
                foreach (var child in SidebarPanel.Children)
                {
                    if (child is Button btn)
                    {
                        btn.Background = Brushes.Transparent;
                    }
                }

                activeBtn.Background = new SolidColorBrush(Color.FromRgb(63, 63, 70));
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error updating UI state.\n\n{ex.Message}",
                    "UI Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ShowHome_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainContent.Content = new HomeView();
                SetActive(btnHome);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading Home view.\n\n{ex.Message}",
                    "UI Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}