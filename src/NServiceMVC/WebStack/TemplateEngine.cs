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
        /// Loads an embedded resource. Returns null if there is a problem (eg, not found).
        /// </summary>
        /// <param name="embeddedResourcePath">The full resource path (including namespace)</param>
        /// <returns></returns>
        public static string LoadEmbeddedResource(string embeddedResourcePath)
        {
            try
            {
                var templateStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResourcePath);
                return new System.IO.StreamReader(templateStream).ReadToEnd();
            }
            catch (Exception)
            {
                return null; // could do this better but it's not all that important
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="embeddedResourcePath"></param>
        /// <param name="model"></param>
        /// <exception cref="FileNotFoundException">The resource could not be found</exception>
        /// <returns></returns>
        public static ContentResult RenderView(string viewName, object model)
        {
            var templateContent = LoadEmbeddedResource("NServiceMVC.Views." + viewName);
            if (templateContent == null) templateContent = string.Format("View {0} not found", viewName);


            var template = DotLiquid.Template.Parse(templateContent);
            // TODO: cache 


            template.Registers.Add("file_system", new EmbeddedResourceFileProvider());

            
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
        }

        /// <summary>
        /// Provider that allows DotLiquid includes from embedded resources
        /// </summary>
        public class EmbeddedResourceFileProvider : DotLiquid.FileSystems.IFileSystem
        {
            public string ReadTemplateFile(DotLiquid.Context context, string templateName)
            {
                return LoadEmbeddedResource("NServiceMVC.Views." + templateName);
            }
        }
    }
}
