using System.ComponentModel.DataAnnotations;

namespace mvc4.Models
{
    public class InvestorModel
    {
        [Key]
        public int InvestorId { get; set; }
        public string? InvestorName { get; set;}
        public string? InvestorAddress { get; set;}
        public string? InvestorEmail { get; set; }
        public string? InvestorPassword { get; set; }
        public string? InvestorContact { get; set; }
        public string? InvestorContactA { get; set; }
        public string? InvestorLogo { get; set; }
        public DateOnly InvestorDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public int InvestorActive { get; set; }
        [Required]
        public int FundId { get; set; }
        [Required]
        public int ClientId { get; set; }
    }
}
