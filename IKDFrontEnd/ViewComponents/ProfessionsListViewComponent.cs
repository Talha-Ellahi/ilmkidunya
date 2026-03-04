using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace IKDFrontEnd.ViewComponents
{
    public class ProfessionsListViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string professionsRaw)
        {
            // Example input: "Quran-Teacher,Government-Teacher"
            if (string.IsNullOrWhiteSpace(professionsRaw))
                return Content(string.Empty);

            var professions = professionsRaw
                .Split(',', System.StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToList();

            return View(professions);
        }
    }
}
