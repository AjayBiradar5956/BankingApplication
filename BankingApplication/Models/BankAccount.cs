using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Text.Json.Serialization;

namespace BankingApplication.Models
{
    public class BankAccount
    {
        [Key]
        public int BankAccountNumber { get; set; }
        public int CurrentBalance { get; set; }

        [ForeignKey("Users")]
        public int UserId { get; set; }
        [JsonIgnore]
        public Users? Users { get; set; }


    }
}
