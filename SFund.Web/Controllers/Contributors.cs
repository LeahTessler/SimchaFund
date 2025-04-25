using Microsoft.AspNetCore.Mvc;
using SFund.Data;
using SFund.Web.Models;

namespace SFund.Web.Controllers
{
    public class Contributors : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimchaFund;Integrated Security=true;TrustServerCertificate=yes;";

        public IActionResult Index()
        {
            
            var sfm = new SimchaFundManager(_connectionString);
            var cvm = new ContributorsViewModel();
            cvm.Contributors = sfm.GetContributors();
            cvm.Total = sfm.GetTotal();
            if(TempData["Message"] != null)
            {
                cvm.Message = (string)TempData["Message"];
            }
            return View(cvm);
        }

        [HttpPost]
        public IActionResult New(Contributor contributor, Deposit deposit)
        {
            
            var sfm = new SimchaFundManager(_connectionString);
            deposit.ContributorId=sfm.AddContributor(contributor);     
            sfm.InsertDeposit(deposit);
            return Redirect("/contributors");

        }


        [HttpPost]
        public IActionResult Deposit(Deposit deposit)
        {
            var sfm = new SimchaFundManager(_connectionString);
            sfm.InsertDeposit(deposit);
            TempData["message"] = "Deposit successfully recorded";
            return Redirect("/contributors");

        }

        public IActionResult History(int id)
        {
            var sfm = new SimchaFundManager(_connectionString);
            
            var transactions = new List<TransactionHistory>();
            transactions.AddRange(sfm.GetContributionsHistoryById(id));
            transactions.AddRange(sfm.GetDepositHistoryById(id));
            var hvm = new HistoryViewModel
            {
                Transactions = transactions.OrderByDescending(t => t.Date).ToList(),
                Balance = sfm.GetBalanceById(id),
                Name = sfm.GetContributorNameById(id)
            };

            return View(hvm);

        }

        [HttpPost]
        public IActionResult Update(Contributor contributor)
        {
            var sfm = new SimchaFundManager(_connectionString);
            sfm.UpdateContributor(contributor);
            TempData["success-message"] = "contributor updated successfully!";
            return Redirect("/Contributors");
        }

    }
}
