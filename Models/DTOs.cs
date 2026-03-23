using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chetan_Broker.Models
{
    public class Party
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TransactionModel
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public decimal Brokerage { get; set; }
    }
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }

}
