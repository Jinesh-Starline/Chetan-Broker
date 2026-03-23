using Microsoft.Data.Sqlite;
using System.IO;

namespace Chetan_Broker.Data
{
    public static class DbHelper
    {
        private static string _dbPath;

        static DbHelper()
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

        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection($"Data Source={_dbPath}");
        }

        private static void InitializeDatabase()
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

            string sql = File.ReadAllText(scriptPath);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
    }
}
