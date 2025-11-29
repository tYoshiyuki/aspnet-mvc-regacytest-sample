using System.Web.Mvc;
using AspNetMvcRegacyTestSample.Web.Models;

namespace AspNetMvcRegacyTestSample.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View("About", new SampleViewModel
            {
                Id = 1,
                Name = "Sample Name"
            });
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View("Contact");
        }

        public ActionResult Redirect()
        {
            return RedirectToAction("Index");
        }
    }
}