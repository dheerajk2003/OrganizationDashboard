using System.ComponentModel.DataAnnotations;

namespace mvc4.Models
{
    public class ClientModel
    {
        [Key]
        public int ClientId { get; set; }
        public string? ClientName { get; set; }
        public string? ClientAddress { get; set; }
        public string? ClientEmail { get; set; }
        public string? ClientPassword { get; set; }
        public string? ClientContact { get; set; }
        public string? ClientLogo { get; set; }
        public DateOnly ClientDate { get; set; }
        public int ClientActive { get; set; }
    }
}
