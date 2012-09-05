using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace NServiceMVC.WebStack
{
    /// <summary>
    /// Controller that services NServiceMVC static content at ~/__NServiceMvcContent/
    /// </summary>
    public class NServiceMvcContentController : Controller
    {

        public ActionResult File(string filename)
        {
            var data = WebStack.TemplateEngine.LoadEmbeddedResource("NServiceMVC.Content." + filename);
            if (data == null)
                return new HttpNotFoundResult();
            
            return new ContentResult() {
                Content = data,
                ContentType = InferContentType(filename)
            };
        }

        /// <summary>
        /// Very naieve method to get a mime content-type
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private string InferContentType(string filename)
        {
            switch (System.IO.Path.GetExtension(filename).ToLower())
            {
                case ".css": return "text/css";
                case ".js": return "text/javascript";
                case ".gif": return "image/gif";
                case ".jpg": case ".jpeg": return "image/jpeg";
                case ".png": return "image/png";
                case ".txt": return "text/plain";
                default: return "text/html";
            }
        }
    }
}
