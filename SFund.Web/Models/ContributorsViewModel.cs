using SFund.Data;

namespace SFund.Web.Models
{
    public class ContributorsViewModel
    {
        public List<Contributor> Contributors { get; set; }
        public decimal Total { get; set; }
        public string SimchaName { get; set; }
        public int SimchaId { get; set; }
        public string Message { get; set; }
    }
}
