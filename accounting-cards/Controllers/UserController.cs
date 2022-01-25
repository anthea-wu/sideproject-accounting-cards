using System.Web.Mvc;

namespace accounting_cards.Controllers
{
    public class UserController : Controller
    {
        // GET
        public ActionResult Index()
        {
            return View();
        }
    }
}