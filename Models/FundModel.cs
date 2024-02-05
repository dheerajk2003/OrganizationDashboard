using System.ComponentModel.DataAnnotations;

namespace mvc4.Models
{
    public class FundModel
    {
        [Key]
        public int FundId { get; set; }
        public string? FundName { get; set; }
        public string? FundAddress { get; set; }
        public string? FundEmail { get; set; }
        public string? FundPassword { get; set; }
        public string? FundContact { get; set; }
        public string? FundLogo { get; set; }
        public DateOnly FundDate { get; set; }
        public int FundActive { get; set; }
        public string? FundBankAccount { get; set; }
        public string? FundBank { get; set; }
        public string? FundBankIFSC { get; set; }
        [Required]
        public int ClientId { get; set; }
    }
}
