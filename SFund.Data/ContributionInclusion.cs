using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFund.Data
{
    public class ContributionInclusion
    {
        public int ContributorId { get; set; }
        public bool Include { get; set; }
        public decimal Amount { get; set; }

    }
}
