using Chetan_Broker.Data;
using Chetan_Broker.Models;
using Dapper;

namespace Chetan_Broker.Services
{
    public class TransactionService
    {
        public void Add(TransactionModel model)
        {
            using var conn = DbHelper.GetConnection();
            conn.Open();

            conn.Execute(@"
            INSERT INTO [Transaction]
            (SenderId, ReceiverId, TransactionDate, Amount, Brokerage)
            VALUES
            (@SenderId, @ReceiverId, @TransactionDate, @Amount, @Brokerage)
        ", model);
        }

        public List<TransactionModel> GetAll()
        {
            using var conn = DbHelper.GetConnection();
            conn.Open();

            return conn.Query<TransactionModel>("SELECT * FROM [Transaction]")
                       .ToList();
        }

        public void Update(int id, TransactionModel model)
        {
            using var conn = DbHelper.GetConnection();
            conn.Open();

            conn.Execute(@"
                UPDATE [Transaction]
                SET SenderId = @SenderId,
                    ReceiverId = @ReceiverId,
                    TransactionDate = @TransactionDate,
                    Amount = @Amount,
                    Brokerage = @Brokerage
                WHERE Id = @Id
            ",
            new
            {
                model.SenderId,
                model.ReceiverId,
                model.TransactionDate,
                model.Amount,
                model.Brokerage,
                Id = id
            });
        }

        public List<dynamic> GetAllWithNames()
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
                    t.Brokerage
                FROM [Transaction] t
                JOIN Party s ON t.SenderId = s.Id
                JOIN Party r ON t.ReceiverId = r.Id
                ORDER BY t.Id DESC
            ").ToList();
        }

        public void Delete(int id)
        {
            using var conn = DbHelper.GetConnection();
            conn.Open();

            conn.Execute(
                "DELETE FROM [Transaction] WHERE Id = @Id",
                new { Id = id });
        }
    }
}
