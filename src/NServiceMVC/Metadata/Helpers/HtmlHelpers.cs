using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace NServiceMVC.Metadata.Helpers
{
    public static class HtmlHelpers
    {
        /// <summary>
        /// Links to the metadata type page, if this is a model that has a metadata type page. Otherwise, plain text.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static MvcHtmlString MetadataTypeLink(this HtmlHelper helper, string typeName)
        {
            if (typeName == null)
            {
                return MvcHtmlString.Create(string.Empty);
            }
            else if (MetadataReflector.GetModelTypes().Contains(typeName))
            {
                return helper.ActionLink(typeName, "type", "Metadata", new { id = typeName }, null);
            }
            else
            {
                return MvcHtmlString.Create(typeName);
            }
        }
    }
}