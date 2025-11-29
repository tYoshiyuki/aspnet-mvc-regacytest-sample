using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AspNetMvcRegacyTestSample.Tests.Helpers;
using AspNetMvcRegacyTestSample.Web.Controllers;
using AspNetMvcRegacyTestSample.Web.Models;

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

        [TestMethod]
        public void Index_Ok()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(string.Empty, result.ViewName);
        }

        [TestMethod]
        public void About_Ok()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.About() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("About", result.ViewName);

            var vm = result.Model as SampleViewModel;
            Assert.IsNotNull(vm);
            Assert.AreEqual(1, vm.Id);
            Assert.AreEqual("Sample Name", vm.Name);
        }

        [TestMethod]
        public void Contact_Ok()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.Contact() as ViewResult;
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Contact", result.ViewName);
        }

        [TestMethod]
        public void Redirect_Ok()
        {
            // Arrange
            var controller = new HomeController();
            
            // Act
            var result = controller.Redirect() as RedirectToRouteResult;
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
    }
}
