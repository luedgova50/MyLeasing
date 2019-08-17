namespace MyLeasing.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;


    [Authorize(Roles = "Manager")]
    public class ContractsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}