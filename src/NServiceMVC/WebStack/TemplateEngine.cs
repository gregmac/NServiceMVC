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
            var templateContent = new System.IO.StreamReader(templateStream).ReadToEnd();


            var template = DotLiquid.Template.Parse(templateContent);
            // TODO: cache 

            
            var output = template.Render(DotLiquid.Hash.FromAnonymousObject(model));

            return new ContentResult() {
                Content = output.ToString(),
                ContentType = "text/html"
            };
        }

        /// <summary>
        /// Initialize the template engine, called by <see cref="NServiceMVC.Initialize()"/>
        /// </summary>
        public static void Initialize()
        {
            DotLiquid.Template.NamingConvention = new DotLiquid.NamingConventions.CSharpNamingConvention();
            DotLiquid.Template.RegisterSimpleNamespace("NServiceMVC.Metadata.Models", false);
        }
    }
}
