using Microsoft.VisualStudio.TestTools.UnitTesting;
using AspNetMvcRegacyTestSample.Tests.Helpers;

namespace AspNetMvcRegacyTestSample.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            // 各テストの前にルートを初期化
            RoutingTestHelper.InitializeRoutes();
        }

        [TestMethod]
        public void URL_ルート_Home_Index_に対応するアクションが存在する()
        {
            // Arrange & Act & Assert
            RoutingTestHelper.AssertRouteAndActionExists(
                url: "/Home/Index",
                httpMethod: "GET"
            );
        }

        [TestMethod]
        public void URL_ルート_Home_About_に対応するアクションが存在する()
        {
            // Arrange & Act & Assert
            RoutingTestHelper.AssertRouteAndActionExists(
                url: "/Home/About",
                httpMethod: "GET"
            );
        }

        [TestMethod]
        public void URL_ルート_Home_Contact_に対応するアクションが存在する()
        {
            // Arrange & Act & Assert
            RoutingTestHelper.AssertRouteAndActionExists(
                url: "/Home/Contact",
                httpMethod: "GET"
            );
        }

        [TestMethod]
        public void URL_ルート_デフォルト_に対応するHome_Indexアクションが存在する()
        {
            // Arrange & Act & Assert
            RoutingTestHelper.AssertRouteAndActionExists(
                url: "/",
                httpMethod: "GET"
            );
        }

        [TestMethod]
        public void URL_存在しないアクション_はルートが見つからない()
        {
            // Arrange & Act & Assert
            RoutingTestHelper.AssertRouteExistsButActionNotExists(
                url: "/Home/NonExistentAction",
                httpMethod: "GET"
            );
        }
    }
}
