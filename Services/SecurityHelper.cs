using System;
using System.Security.Cryptography;
using System.Windows;

namespace Chetan_Broker.Services
{
    public static class SecurityHelper
    {
        public static string HashPassword(string password)
        {
            try
            {
                byte[] salt = RandomNumberGenerator.GetBytes(16);

                var pbkdf2 = new Rfc2898DeriveBytes(
                    password,
                    salt,
                    100000,
                    HashAlgorithmName.SHA256
                );

                byte[] hash = pbkdf2.GetBytes(32);

                return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error hashing password.\n\n{ex.Message}",
                    "Security Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return string.Empty; // fail safe
            }
        }

        public static bool VerifyPassword(string password, string stored)
        {
            try
            {
                var parts = stored.Split('.');
                if (parts.Length != 2)
                    return false;

                byte[] salt = Convert.FromBase64String(parts[0]);
                byte[] storedHash = Convert.FromBase64String(parts[1]);

                var pbkdf2 = new Rfc2898DeriveBytes(
                    password,
                    salt,
                    100000,
                    HashAlgorithmName.SHA256
                );

                byte[] newHash = pbkdf2.GetBytes(32);

                return CryptographicOperations.FixedTimeEquals(newHash, storedHash);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error verifying password.\n\n{ex.Message}",
                    "Security Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }
        }
    }
}