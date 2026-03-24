using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Windows;

namespace Chetan_Broker.Data
{
    public static class DbHelper
    {
        private static string _dbPath = string.Empty;

        static DbHelper()
        {
            try
            {
                string folder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "ChetanBroker"
                );

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                _dbPath = Path.Combine(folder, "chetanbroker.db");

                InitializeDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to initialize application storage.\n\n{ex.Message}",
                    "Initialization Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public static SqliteConnection GetConnection()
        {
            try
            {
                return new SqliteConnection($"Data Source={_dbPath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to create database connection.\n\n{ex.Message}",
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                throw; // still throw so calling code knows it's broken
            }
        }

        private static void InitializeDatabase()
        {
            try
            {
                if (File.Exists(_dbPath))
                    return;

                using var conn = new SqliteConnection($"Data Source={_dbPath}");
                conn.Open();

                string scriptPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Data",
                    "Scripts",
                    "init.sql"
                );

                if (!File.Exists(scriptPath))
                {
                    MessageBox.Show(
                        $"Database script not found.\n\nPath:\n{scriptPath}",
                        "Missing File",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                string sql = File.ReadAllText(scriptPath);

                using var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Database initialization failed.\n\n{ex.Message}",
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}