using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.RegularExpressions;

public static class HtmlHelperExtensions
{
    public static IHtmlContent RenderWithAds(this IHtmlHelper htmlHelper, string content)
    {
        if (string.IsNullOrEmpty(content))
            return new HtmlString(string.Empty);

        // Render partial view as string
        var adHtml = RenderPartialViewToString(htmlHelper, "_GoogleAd");

        // Replace marker with ad HTML
        var replacedContent = Regex.Replace(content, @"#\{standard\}#", adHtml);

        return new HtmlString(replacedContent);
    }

    private static string RenderPartialViewToString(IHtmlHelper htmlHelper, string partialViewName)
    {
        using (var writer = new StringWriter())
        {
            var viewContext = htmlHelper.ViewContext;
            var viewEngine = (ICompositeViewEngine)viewContext.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine));
            var viewResult = viewEngine.FindView(viewContext, partialViewName, false);

            if (!viewResult.Success)
                return string.Empty;

            var view = viewResult.View;
            var viewData = new ViewDataDictionary(htmlHelper.ViewData)
            {
                Model = null
            };

            var tempData = htmlHelper.ViewContext.TempData;
            var newViewContext = new ViewContext(viewContext, view, viewData, tempData, writer, new HtmlHelperOptions());

            view.RenderAsync(newViewContext).Wait();

            return writer.GetStringBuilder().ToString();
        }
    }
}
