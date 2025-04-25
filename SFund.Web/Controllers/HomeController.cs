using Microsoft.AspNetCore.Mvc;
using SFund.Data;
using SFund.Web.Models;
using System.Diagnostics;

namespace SFund.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimchaFund;Integrated Security=true;TrustServerCertificate=yes;";


        public IActionResult Index()
        {
            
            var sfm = new SimchaFundManager(_connectionString);
            var svm = new SimchasViewModel();
            svm.simchas = sfm.GetSimchas();
            svm.TotalContributors = sfm.GetTotalAmountOfContributors();
            if (TempData["Message"] != null)
            {
                svm.Message = (string)TempData["Message"];
            }
            return View(svm);
        }

        [HttpPost]
        public IActionResult NewSimcha(Simcha simcha)
        {
            var sfm = new SimchaFundManager(_connectionString);
            sfm.AddNewSimcha(simcha);
            TempData["Message"] = "New Simcha created!";
            return Redirect("/");


        }

        public IActionResult Contributions(int simchaid)
        {
            var sfm = new SimchaFundManager(_connectionString);
            var cvm = new ContributionsForASimcha();
            cvm.Contributors = sfm.GetContributionsForASimcha(simchaid);
            cvm.SimchaName = sfm.GetSimchaNameById(simchaid);
            cvm.SimchaId = simchaid;
            return View(cvm);
        }

        [HttpPost]
        public IActionResult UpdateContributions(List<ContributionInclusion> contributors,int simchaId)
        {
     
            var sfm = new SimchaFundManager(_connectionString);
            sfm.Delete(simchaId);
            var update = contributors.Where(c => c.Include).ToList();       
            sfm.UpdateSimcha(simchaId, update);
            TempData["message"] = "Simcha updated successfully";
            return Redirect("/");

        }



    }
}
