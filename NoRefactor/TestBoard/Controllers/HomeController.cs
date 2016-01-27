using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestBoard.Controllers
{
    /// <summary>
    /// トップページ
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// GET: Home
        /// トップページ
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}