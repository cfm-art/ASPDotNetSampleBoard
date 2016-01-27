using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBoard.Controllers;
using System.Web.Mvc;

namespace TestBoard.Tests.Controllers
{
    /// <summary>
    /// トップ画面用テスト
    /// </summary>
    [TestClass]
    public class HomeControllerTest
    {
        /// <summary>
        /// トップ画面
        /// </summary>
        [TestMethod]
        public void Index()
        {
            // エラーが無いかだけチェック
            var controller = new HomeController();
            var result = controller.Index() as ViewResult;
            Assert.IsNotNull( result );
        }
    }
}
