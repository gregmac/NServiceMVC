using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace NServiceMVC
{
    abstract public class NServiceMVC
    {
        /// <summary>
        /// Bootstrap method for NServiceMVC
        /// </summary>
        public static void Initalize()
        {
            ModelBinders.Binders.DefaultBinder = (new MultipleRepresentationsBinder());
        }
    }
}
