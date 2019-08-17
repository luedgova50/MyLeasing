namespace MyLeasing.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = "Manager")]
    public class LesseesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}