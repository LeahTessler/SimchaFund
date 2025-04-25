using SFund.Data;

namespace SFund.Web.Models
{
    public class HistoryViewModel
    {
        public List<TransactionHistory> Transactions { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
    }
}
