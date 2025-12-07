using System.Web.Mvc;

namespace AspNetMvcRegacyTestSample.Web.Controllers
{
    /// <summary>
    /// 属性ルーティングを使用するコントローラーの例
    /// </summary>
    [RoutePrefix("Product")]
    public class ProductController : Controller
    {
        [Route("Index")]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [Route("Details")]
        [HttpPost]
        public ActionResult Details(int id)
        {
            ViewBag.ProductId = id;
            return View();
        }

        [Route("List")]
        [HttpGet]
        public ActionResult List()
        {
            return View();
        }

        [Route("ByCategory")]
        [HttpGet]
        public ActionResult ByCategory(string category, int id)
        {
            ViewBag.Category = category;
            ViewBag.ProductId = id;
            return View();
        }

        [Route("Search")]
        [HttpGet]
        public ActionResult Search(string keyword = null)
        {
            ViewBag.Keyword = keyword;
            return View();
        }
    }
}


