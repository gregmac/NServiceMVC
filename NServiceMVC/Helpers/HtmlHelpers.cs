using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace NServiceMVC.Helpers
{
    public static class HtmlHelpers
    {
        public static HtmlString JsonSerialize(this HtmlHelper helper, object model)
        {
            return new HtmlString(Newtonsoft.Json.JsonConvert.SerializeObject(model));
        }

    }
}