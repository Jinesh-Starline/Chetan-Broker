using Chetan_Broker.Data;
using Chetan_Broker.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Chetan_Broker.Services
{
    public class TransactionService
    {
        public void Add(TransactionModel model)
        {
            try
            {
                using var conn = DbHelper.GetConnection();
                conn.Open();

                conn.Execute(@"
                    INSERT INTO [Transaction]
                    (SenderId, ReceiverId, TransactionDate, Amount, Brokerage, BagQuantity, Remarks)
                    VALUES
                    (@SenderId, @ReceiverId, @TransactionDate, @Amount, @Brokerage, @BagQuantity, @Remarks)
                ", model);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error adding transaction.\n\n{ex.Message}",
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public List<TransactionModel> GetAll()
        {
            try
            {
                using var conn = DbHelper.GetConnection();
                conn.Open();

                return conn.Query<TransactionModel>("SELECT * FROM [Transaction]").ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading transactions.\n\n{ex.Message}",
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return new List<TransactionModel>();
            }
        }

        public void Update(int id, TransactionModel model)
        {
            try
            {
                using var conn = DbHelper.GetConnection();
                conn.Open();

                conn.Execute(@"
                    UPDATE [Transaction]
                    SET SenderId = @SenderId,
                        ReceiverId = @ReceiverId,
                        TransactionDate = @TransactionDate,
                        Amount = @Amount,
                        Brokerage = @Brokerage,
                        BagQuantity = @BagQuantity,
                        Remarks = @Remarks
                    WHERE Id = @Id
                ",
                new
                {
                    model.SenderId,
                    model.ReceiverId,
                    model.TransactionDate,
                    model.Amount,
                    model.Brokerage,
                    model.BagQuantity,
                    model.Remarks,
                    Id = id
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error updating transaction.\n\n{ex.Message}",
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public List<dynamic> GetAllWithNames()
        {
            try
            {
                using var conn = DbHelper.GetConnection();
                conn.Open();

                return conn.Query(@"
                    SELECT 
                        t.Id,
                        t.SenderId,
                        t.ReceiverId,
                        s.Name AS SenderName,
                        r.Name AS ReceiverName,
                        t.TransactionDate,
                        t.Amount,
                        t.Brokerage,
                        t.BagQuantity As BagQuantity,
                        t.Remarks As Remarks
                    FROM [Transaction] t
                    JOIN Party s ON t.SenderId = s.Id
                    JOIN Party r ON t.ReceiverId = r.Id
                    ORDER BY t.Id DESC
                ").ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading transaction list.\n\n{ex.Message}",
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return new List<dynamic>();
            }
        }

        public void Delete(int id)
        {
            try
            {
                using var conn = DbHelper.GetConnection();
                conn.Open();

                conn.Execute(
                    "DELETE FROM [Transaction] WHERE Id = @Id",
                    new { Id = id });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error deleting transaction.\n\n{ex.Message}",
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public List<ReportDto> GetByParty(int partyId)
        {
            try
            {
                using var conn = DbHelper.GetConnection();
                conn.Open();

                return conn.Query<ReportDto>(@"
                    SELECT 
                        t.Id,
                        CASE 
                            WHEN t.SenderId = @Id THEN r.Name
                            ELSE s.Name
                        END AS Name,
                        CASE 
                            WHEN t.SenderId = @Id THEN r.City
                            ELSE s.City
                        END AS City,
                        t.TransactionDate,
                        t.Amount,
                        t.Brokerage,
                        t.BagQuantity,
                        t.Remarks
                    FROM [Transaction] t
                    JOIN Party s ON t.SenderId = s.Id
                    JOIN Party r ON t.ReceiverId = r.Id
                    WHERE t.SenderId = @Id OR t.ReceiverId = @Id
                    ORDER BY t.TransactionDate DESC
                ", new { Id = partyId }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading party transactions.\n\n{ex.Message}",
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return new List<ReportDto>();
            }
        }
    }
}