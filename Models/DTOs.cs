using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using iText.Pdfua.Checkers.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chetan_Broker.Models
{
    public class BrokerAccount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PersonName { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string IFSCCode { get; set; }
        public string PANNo { get; set; }
    }

    public class Party
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int BrokerAccountId { get; set; }

        public string BrokerName { get; set; }   // 🔥 important

        public string City { get; set; }
    }

    public class TransactionModel
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string? TransactionDate { get; set; }
        public string? Amount { get; set; }
        public string? Brokerage { get; set; }
        public string? BagQuantity { get; set; }
        public string? Remarks { get; set; }
    }
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }

    public class ReportDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? City { get; set; }
        public string? TransactionDate { get; set; }
        public string? Amount { get; set; }
        public string? Brokerage { get; set; }
        public string? BagQuantity { get; set; }
        public string? Remarks { get; set; }
    }
}
