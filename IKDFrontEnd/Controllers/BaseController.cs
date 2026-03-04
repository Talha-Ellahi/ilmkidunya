using Microsoft.AspNetCore.Mvc;

namespace IKDFrontEnd.Controllers
{
	public class BaseController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
