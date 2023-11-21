using Microsoft.Extensions.Configuration.UserSecrets;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace BankingApplication.Models
{
    public class Transactions
    {
        [Key]
        public int TransactionId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int Amount { get; set; }
        public int CurrentBalance { get; set; }

        [ForeignKey("Users")]
        public int UserId { get; set; }
        public Users? Users { get; set; }
    }
}
