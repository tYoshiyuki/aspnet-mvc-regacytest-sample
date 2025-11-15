using System.Web.Mvc;

namespace AspNetMvcRegacyTestSample.Web.Controllers
{
    /// <summary>
    /// 属性ルーティングを使用するコントローラーの例
    /// </summary>
    [RoutePrefix("Product"), Route("{action}")]
    public class ProductController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Details(int id)
        {
            ViewBag.ProductId = id;
            return View();
        }

        [HttpGet]
        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ByCategory(string category, int id)
        {
            ViewBag.Category = category;
            ViewBag.ProductId = id;
            return View();
        }

        [HttpGet]
        public ActionResult Search(string keyword = null)
        {
            ViewBag.Keyword = keyword;
            return View();
        }
    }
}


