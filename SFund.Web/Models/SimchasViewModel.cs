using SFund.Data;

namespace SFund.Web.Models
{
    public class SimchasViewModel
    {
        public List<Simcha> simchas { get; set; }
        public int TotalContributors { get; set; }
        public string Message { get; set; }
    }
}
