using Chetan_Broker.Data;
using Dapper;
using System;
using System.Windows;

namespace Chetan_Broker.Services
{
    public class UserService
    {
        public bool Validate(string username, string password)
        {
            try
            {
                using var conn = DbHelper.GetConnection();
                conn.Open();

                var user = conn.QueryFirstOrDefault<string>(
                    "SELECT PasswordHash FROM User WHERE Username = @Username",
                    new { Username = username });

                if (user == null)
                    return false;

                return SecurityHelper.VerifyPassword(password, user);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error validating user.\n\n{ex.Message}",
                    "Authentication Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }
        }

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            try
            {
                using var conn = DbHelper.GetConnection();
                conn.Open();

                var current = conn.QueryFirstOrDefault<string>(
                    "SELECT PasswordHash FROM User WHERE Username = @Username",
                    new { Username = username });

                if (current == null || !SecurityHelper.VerifyPassword(oldPassword, current))
                    return false;

                var passwordHash = SecurityHelper.HashPassword(newPassword);

                if (string.IsNullOrEmpty(passwordHash))
                {
                    MessageBox.Show(
                        "Failed to generate password hash.",
                        "Security Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return false;
                }

                conn.Execute(
                    "UPDATE User SET PasswordHash = @Password WHERE Username = @Username",
                    new { Password = passwordHash, Username = username });

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error changing password.\n\n{ex.Message}",
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }
        }

        public void SetAutoLogin(string username, bool isAuto)
        {
            try
            {
                using var conn = DbHelper.GetConnection();
                conn.Open();

                conn.Execute(
                    "UPDATE User SET AutoLogin = @AutoLogin WHERE Username = @Username",
                    new
                    {
                        AutoLogin = isAuto ? 1 : 0,
                        Username = username
                    });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error updating auto-login setting.\n\n{ex.Message}",
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public string GetAutoLoginUser()
        {
            try
            {
                using var conn = DbHelper.GetConnection();
                conn.Open();

                return conn.QueryFirstOrDefault<string>(
                    "SELECT Username FROM User WHERE AutoLogin = 1 LIMIT 1");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error fetching auto-login user.\n\n{ex.Message}",
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return null;
            }
        }
    }
}