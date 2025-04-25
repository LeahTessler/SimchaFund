using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFund.Data
{
    public class TransactionHistory
    {
        public string TransactionName { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }

    }
}
