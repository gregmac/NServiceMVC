using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceMVC.Formats
{
    public interface IFormatHandler
    {
        //IEnumerable<string> SupportedContentTypes();
        //IEnumerable<string> SupportedTypeAliases();

        //System.Web.Mvc.ContentResult Serialize(string contentType, object model);

        string Serialize(object model, bool humanReadable = false);

        object Deserialize(string input, Type modelType);
    }
}
