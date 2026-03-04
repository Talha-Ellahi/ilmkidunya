using Microsoft.AspNetCore.Mvc;

namespace IKDFrontEnd.ViewComponents
{
    public class GoogleAdViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string adSlot)
        {
            return View("Default", adSlot);
        }
    }
}
