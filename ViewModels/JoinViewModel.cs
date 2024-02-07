using mvc4.Models;

namespace mvc4.ViewModels
{
    public class JoinViewModel
    {
        public InvestorModel Investor { get; set; }
        public FundModel Fund { get; set; }
        public ClientModel Client { get; set; }
    }
}
