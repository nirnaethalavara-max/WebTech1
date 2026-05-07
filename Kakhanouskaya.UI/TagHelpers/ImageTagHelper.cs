using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Kakhanouskaya.UI.TagHelpers
{
    [HtmlTargetElement("img", Attributes = "img-action,img-controller")]
    public class ImageTagHelper : TagHelper
    {
        public string ImgAction { get; set; } = "";
        public string ImgController { get; set; } = "";
        public int ImgId { get; set; } = 0;

        private readonly LinkGenerator _linkGenerator;

        public ImageTagHelper(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string url;
            if (ImgId > 0)
            {
                // Выкарыстоўваем новы метад GetDishImage
                url = _linkGenerator.GetPathByAction("GetDishImage", "Image", new { id = ImgId });
            }
            else
            {
                url = _linkGenerator.GetPathByAction(ImgAction, ImgController);
            }

            output.Attributes.SetAttribute("src", url);
            output.Attributes.RemoveAll("img-action");
            output.Attributes.RemoveAll("img-controller");
            output.Attributes.RemoveAll("img-id");
        }
    }
}