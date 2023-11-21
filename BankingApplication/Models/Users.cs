using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace BankingApplication.Models
{
    public class Users
    {
        [Key]
        public int Id { get; set; }
        [StringLength(20)]
        public string FirstName { get; set; } = string.Empty;
        [StringLength(20)]
        public string LastName { get; set; } = string.Empty;
        public int InitialDeposit { get; set; }
        [StringLength(300)]
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public BankAccount? BankAccount { get; set; }

    }
}
