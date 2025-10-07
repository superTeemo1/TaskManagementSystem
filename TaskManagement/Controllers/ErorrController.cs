using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.Web.Controllers
{
    /// <summary>
    /// Serves friendly error/status pages for MVC routes.
    /// </summary>
    public class ErorrController : Controller
    {
        // GET: /Error
        public IActionResult Index()
        {
            // Generic server-side error page
            Response.StatusCode = 500;
            return View("~/Views/Shared/Error.cshtml");
        }

        // GET: /Error/Status/{code}
        [Route("Error/Status/{code:int}")]
        public IActionResult Status(int code)
        {
            // Renders a friendly status page for codes like 404, 403...
            ViewData["StatusCode"] = code;
            return View("~/Views/Shared/Status.cshtml");
        }
    }
}
