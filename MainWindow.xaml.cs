using Chetan_Broker.Controls;
using Chetan_Broker.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Chetan_Broker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserService _userService = new UserService();
        private string _currentUser;

        public MainWindow()
        {
            InitializeComponent();


            var user = _userService.GetAutoLoginUser();

            if (user != null)
            {
                _currentUser = user;

                LoginSection.Visibility = Visibility.Collapsed;
                MainSection.Visibility = Visibility.Visible;

                this.WindowState = WindowState.Maximized;
            }
            else
                txtUsername.Focus();


        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var username = txtUsername.Text;
            var password = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Enter username and password");
                return;
            }

            var isValid = _userService.Validate(username, password);

            if (!isValid)
            {
                MessageBox.Show("Invalid credentials");
                return;
            }

            // 🔥 SAVE AUTO LOGIN
            _userService.SetAutoLogin(username, chkAutoLogin.IsChecked == true);

            _currentUser = username;

            LoginSection.Visibility = Visibility.Collapsed;
            MainSection.Visibility = Visibility.Visible;

            this.WindowState = WindowState.Maximized;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            _userService.SetAutoLogin(_currentUser, false);

            _currentUser = null;

            MainSection.Visibility = Visibility.Collapsed;
            LoginSection.Visibility = Visibility.Visible;

            this.WindowState = WindowState.Normal;
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            var window = new ChangePasswordWindow(_currentUser);
            window.Owner = this;
            window.ShowDialog();
        }

        private void ShowParty_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new PartyView();
            SetActive(btnParty);

        }

        private void ShowTransaction_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new TransactionView();
            SetActive(btnTransaction);

        }

        private void SetActive(Button activeBtn)
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
    }
}