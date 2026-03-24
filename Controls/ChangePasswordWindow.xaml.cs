using System.Windows;

namespace Chetan_Broker.Controls
{
    using Chetan_Broker.Services;

    /// <summary>
    /// Interaction logic for ChangePasswordWindow.xaml
    /// </summary>

    public partial class ChangePasswordWindow : Window
    {
        private UserService _userService = new UserService();

        public string Username { get; set; }

        public ChangePasswordWindow(string username)
        {
            InitializeComponent();
            Username = username;
        }
        private void UpdatePassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var oldPass = txtOldPassword.Password;
                var newPass = txtNewPassword.Password;
                var confirmPass = txtConfirmPassword.Password;

                if (string.IsNullOrWhiteSpace(oldPass) ||
                    string.IsNullOrWhiteSpace(newPass) ||
                    string.IsNullOrWhiteSpace(confirmPass))
                {
                    MessageBox.Show(
                        "All fields are required.",
                        "Validation",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (newPass != confirmPass)
                {
                    MessageBox.Show(
                        "Passwords do not match.",
                        "Validation",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                var success = _userService.ChangePassword(Username, oldPass, newPass);

                if (!success)
                {
                    MessageBox.Show(
                        "Old password is incorrect.",
                        "Authentication Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show(
                    "Password updated successfully.",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error updating password.\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

    }
}
