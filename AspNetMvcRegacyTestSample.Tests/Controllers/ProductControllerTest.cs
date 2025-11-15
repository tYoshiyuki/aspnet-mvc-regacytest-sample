using Microsoft.VisualStudio.TestTools.UnitTesting;
using AspNetMvcRegacyTestSample.Tests.Helpers;

namespace AspNetMvcRegacyTestSample.Tests.Controllers
{
    [TestClass]
    public class ProductControllerTest
    {
        [TestMethod]
        public void URL_属性ルーティング_Product_Index_に対応するアクションが存在する()
        {
            // Arrange & Act & Assert
            RoutingTestHelper.AssertAttributeRouteExists(
                url: "/Product/Index",
                httpMethod: "GET"
            );
        }

        [TestMethod]
        public void URL_属性ルーティング_Product_List_に対応するアクションが存在する()
        {
            // Arrange & Act & Assert
            RoutingTestHelper.AssertAttributeRouteExists(
                url: "/Product/List",
                httpMethod: "GET"
            );
        }

        [TestMethod]
        public void URL_属性ルーティング_Product_Details_に対応するアクションが存在する()
        {
            // Arrange & Act & Assert
            RoutingTestHelper.AssertAttributeRouteExists(
                url: "/Product/Details",
                httpMethod: "POST"
            );
        }

        [TestMethod]
        public void URL_属性ルーティング_Product_Search_に対応するアクションが存在する()
        {
            // Arrange & Act & Assert
            RoutingTestHelper.AssertAttributeRouteExists(
                url: "/Product/Search",
                httpMethod: "GET"
            );
        }

        [TestMethod]
        public void URL_属性ルーティング_Product_ByCategory_に対応するアクションが存在する()
        {
            // Arrange & Act & Assert
            RoutingTestHelper.AssertAttributeRouteExists(
                url: "/Product/ByCategory",
                httpMethod: "GET"
            );
        }
    }
}
