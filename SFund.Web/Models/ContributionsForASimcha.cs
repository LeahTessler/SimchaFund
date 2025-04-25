using SFund.Data;

namespace SFund.Web.Models
{
    public class ContributionsForASimcha
    {
        public List<Contributor> Contributors { get; set; }
        public int SimchaId { get; set; }
        public string SimchaName { get; set; }
    }
}
