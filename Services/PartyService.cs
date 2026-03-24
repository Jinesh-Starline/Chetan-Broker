using Chetan_Broker.Data;
using Chetan_Broker.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Chetan_Broker.Services
{
    public class PartyService
    {
        public List<Party> GetAll()
        {
            try
            {
                using var conn = DbHelper.GetConnection();
                conn.Open();

                return conn.Query<Party>(@"
                    SELECT 
                        pa.Id,
                        pa.Name,
                        pa.BrokerAccountId,
                        ba.Name AS BrokerName,
                        pa.City
                    FROM Party pa
                    LEFT JOIN BrokerAccount ba 
                        ON ba.Id = pa.BrokerAccountId
                ").ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading parties: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Party>(); // safe fallback
            }
        }

        public void Add(Party model)
        {
            try
            {
                using var conn = DbHelper.GetConnection();
                conn.Open();

                conn.Execute(
                    @"INSERT INTO Party (Name, BrokerAccountId, City) 
                      VALUES (@Name, @BrokerAccountId, @City)",
                    new
                    {
                        model.Name,
                        model.BrokerAccountId,
                        model.City
                    });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding party: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Update(int id, string name, int brokerid, string City)
        {
            try
            {
                using var conn = DbHelper.GetConnection();
                conn.Open();

                conn.Execute(
                    @"UPDATE Party 
                      SET Name = @Name, 
                          BrokerAccountId = @BrokerAccountId,  
                          City = @City
                      WHERE Id = @Id",
                    new
                    {
                        Name = name,
                        Id = id,
                        BrokerAccountId = brokerid,
                        City
                    });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating party: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool Delete(int id)
        {
            try
            {
                using var conn = DbHelper.GetConnection();
                conn.Open();

                // Check usage
                var used = conn.ExecuteScalar<int>(@"
                    SELECT COUNT(1) 
                    FROM [Transaction] 
                    WHERE SenderId = @Id OR ReceiverId = @Id",
                    new { Id = id });

                if (used > 0)
                {
                    MessageBox.Show("Cannot delete. Party is used in transactions.",
                        "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                conn.Execute(
                    "DELETE FROM Party WHERE Id = @Id",
                    new { Id = id });

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting party: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}