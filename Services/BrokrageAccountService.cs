using Chetan_Broker.Data;
using Chetan_Broker.Models;
using Dapper;
using System.Windows;

namespace Chetan_Broker.Services
{
    public class BrokrageAccountService
    {
        public List<BrokerAccount> GetAll()
        {
            try
            {
                using var conn = DbHelper.GetConnection();
                conn.Open();

                return conn.Query<BrokerAccount>("SELECT * FROM BrokerAccount").ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading broker accounts.\n\n{ex.Message}",
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return new List<BrokerAccount>(); // safe fallback
            }
        }

        public BrokerAccount? GetById(int brokerId)
        {
            try
            {
                using var conn = DbHelper.GetConnection();
                conn.Open();

                return conn.QueryFirstOrDefault<BrokerAccount>(
                    @"SELECT * FROM BrokerAccount WHERE Id = @Id",
                    new { Id = brokerId });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading broker account.\n\n{ex.Message}",
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return null;
            }
        }

        //public void Add(string name)
        //{
        //    using var conn = DbHelper.GetConnection();
        //    conn.Open();

        //    conn.Execute(
        //        "INSERT INTO Party (Name) VALUES (@Name)",
        //        new { Name = name }
        //    );
        //}

        //public void Update(int id, string name)
        //{
        //    using var conn = DbHelper.GetConnection();
        //    conn.Open();

        //    conn.Execute(
        //        "UPDATE Party SET Name = @Name WHERE Id = @Id",
        //        new { Name = name, Id = id }
        //    );
        //}

        //public bool Delete(int id)
        //{
        //    using var conn = DbHelper.GetConnection();
        //    conn.Open();

        //    // Check usage
        //    var used = conn.ExecuteScalar<int>(
        //        @"SELECT COUNT(1) 
        //  FROM [Transaction] 
        //  WHERE SenderId = @Id OR ReceiverId = @Id",
        //        new { Id = id });

        //    if (used > 0)
        //        return false;

        //    conn.Execute(
        //        "DELETE FROM Party WHERE Id = @Id",
        //        new { Id = id });

        //    return true;
        //}
    }
}
