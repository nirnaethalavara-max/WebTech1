using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Encodings.Web;

namespace Kakhanouskaya.UI.TagHelpers
{
    [HtmlTargetElement("pager")]  // Гэта значыць, што тэг будзе называцца <pager>
    public class PagerTagHelper : TagHelper
    {
        // Уласцівасці, якія можна перадаць у тэг
        public int CurrentPage { get; set; }     // бягучы нумар старонкі
        public int TotalPages { get; set; }      // агульная колькасць старонак
        public string? Category { get; set; }    // імя катэгорыі
        public bool IsAdmin { get; set; } = false; // для будучых патрэб

        // Сэрвісы, якія нам патрэбны (іх ASP.NET укажа аўтаматычна)
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = null!;

        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly LinkGenerator _linkGenerator;

        public PagerTagHelper(IUrlHelperFactory urlHelperFactory, LinkGenerator linkGenerator)
        {
            _urlHelperFactory = urlHelperFactory;
            _linkGenerator = linkGenerator;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Калі ўсяго адна старонка — нічога не выводзім
            if (TotalPages <= 1)
            {
                output.SuppressOutput();
                return;
            }

            // Усталёўваем тып тэга: <nav>
            output.TagName = "nav";
            output.Attributes.SetAttribute("aria-label", "Page navigation");
            output.AddClass("mt-4", HtmlEncoder.Default);

            // Ствараем HTML для пагінацыі
            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination");
            ul.AddCssClass("justify-content-center");

            // Дадаем кнопку "Назад"
            ul.InnerHtml.AppendHtml(CreatePageLink(GetPreviousPage(), "« Назад", false));

            // Дадаем кнопкі з нумарамі старонак
            for (int i = 1; i <= TotalPages; i++)
            {
                ul.InnerHtml.AppendHtml(CreatePageLink(i, i.ToString(), i == CurrentPage));
            }

            // Дадаем кнопку "Далей"
            ul.InnerHtml.AppendHtml(CreatePageLink(GetNextPage(), "Далей »", false));

            // Укладваем ul у nav
            output.Content.AppendHtml(ul);
        }

        private TagBuilder CreatePageLink(int pageNumber, string displayText, bool isActive)
        {
            var li = new TagBuilder("li");
            li.AddCssClass("page-item");

            if (isActive)
            {
                li.AddCssClass("active");
            }

            var a = new TagBuilder("a");
            a.AddCssClass("page-link");
            a.Attributes["href"] = GenerateUrl(pageNumber);
            a.InnerHtml.Append(displayText);

            li.InnerHtml.AppendHtml(a);
            return li;
        }

        private string GenerateUrl(int pageNumber)
        {
            // Ствараем URL для зададзенай старонкі
            var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
            return urlHelper.Action("Index", "Product", new { pageNo = pageNumber, category = Category });
        }

        private int GetPreviousPage() => CurrentPage == 1 ? 1 : CurrentPage - 1;
        private int GetNextPage() => CurrentPage == TotalPages ? TotalPages : CurrentPage + 1;
        private bool IsPreviousPageDisabled() => CurrentPage == 1;
        private bool IsNextPageDisabled() => CurrentPage == TotalPages;
    }
}
