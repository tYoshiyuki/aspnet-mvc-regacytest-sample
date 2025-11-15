using System.Web.Mvc;
using System.Web.Routing;

namespace AspNetMvcRegacyTestSample.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        public static void RegisterAttributeRoutes(RouteCollection routes)
        {
            // 属性ルーティングを有効化
            routes.MapMvcAttributeRoutes();
        }
    }
}
