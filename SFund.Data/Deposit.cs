using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFund.Data
{
    public class Deposit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ContributorId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}
