using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
//using Nustache.Core;

namespace NServiceMVC.WebStack
{
    public class TemplateEngine
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="embeddedResourcePath"></param>
        /// <param name="model"></param>
        /// <exception cref="FileNotFoundException">The resource could not be found</exception>
        /// <returns></returns>
        public static ContentResult RenderView(string embeddedResourcePath, object model)
        {
            var templateStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResourcePath);
            var template = new System.IO.StreamReader(templateStream);

            var output = new System.IO.StringWriter();

            Nustache.Core.Render.Template(template, model, output, null);

            return new ContentResult() {
                Content = output.ToString(),
                ContentType = "text/html"
            };
        }
    }
}
