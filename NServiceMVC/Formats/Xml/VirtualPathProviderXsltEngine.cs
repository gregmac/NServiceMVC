////////////////////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2008 Piers Lawson
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in the
// Software without restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
// Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN
// AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Xml.Xsl;
using System.Web.Routing;
using System.Collections.Generic;

namespace NServiceMVC.Formats.Xml
{
    /// <summary>
    /// Locates, compiles and caches XSLT transforms held as files 
    /// </summary>
    internal class VirtualPathProviderXsltEngine
    {
        private readonly string _cacheKeyPrefixXslt;
        private static readonly string[] EmptyLocations = new string[0];
        private VirtualPathProvider _vpp;

        internal static ICompiledTransformCache CompiledTransformCache { get; set; }

        internal string[] XsltLocationFormats { get; set; }
        internal string[] AreaXsltLocationFormats { get; set; }

        internal VirtualPathProviderXsltEngine()
        {
            XsltLocationFormats = new [] { "~/Views/{1}/{0}.xslt", "~/Views/Shared/{0}.xslt" };
            AreaXsltLocationFormats = new [] { "~/Areas/{2}/Views/{1}/{0}.xslt", "~/Areas/{2}/Views/Shared/{0}.xslt" };


            if (HttpContext.Current == null || HttpContext.Current.IsDebuggingEnabled)
            {
                CompiledTransformCache = new NullCompiledTransformCache();
            }
            else
            {
                CompiledTransformCache = new AspNetCacheCompiledTransformCache();
            }

            string className = GetType().FullName;
            _cacheKeyPrefixXslt = className + ":Xslt:";
        }

        protected VirtualPathProvider VirtualPathProvider
        {
            get { return _vpp ?? (_vpp = HostingEnvironment.VirtualPathProvider); }
            set { _vpp = value; }
        }

        internal virtual XsltEngineResult FindTransform(ControllerContext controllerContext, string xsltName)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }

            if (String.IsNullOrEmpty(xsltName))
            {
                throw new ArgumentException("A name must be supplied for the Xslt to be used.", "xsltName");
            }

            string[] searched;
            string controllerName = controllerContext.RouteData.GetRequiredString("controller");
            XslCompiledTransform xslCompiledTransform = GetCompiledTransform(controllerContext, XsltLocationFormats, AreaXsltLocationFormats, "XsltLocationFormats", xsltName, controllerName, _cacheKeyPrefixXslt, out searched);

            if (xslCompiledTransform == null)
            {
                return new XsltEngineResult(searched);
            }

            return new XsltEngineResult(xslCompiledTransform);
        }


        public static string GetAreaName(RouteData routeData)
        {
            object areaObject;
            if (routeData.DataTokens.TryGetValue("area", out areaObject))
            {
                return (areaObject as string);
            }

            return GetAreaName(routeData.Route);
        }

        public static string GetAreaName(RouteBase route)
        {
            var area = route as IRouteWithArea;
            if (area != null)
            {
                return area.Area;
            }
            var route2 = route as Route;
            if ((route2 != null) && (route2.DataTokens != null))
            {
                return (route2.DataTokens["area"] as string);
            }
            return null;
        }

        private static List<ViewLocation> GetViewLocations(IEnumerable<string> viewLocationFormats, IEnumerable<string> areaViewLocationFormats)
        {
            var list = new List<ViewLocation>();
            if (areaViewLocationFormats != null)
            {
                list.AddRange(areaViewLocationFormats.Select(str => new AreaAwareViewLocation(str)));
            }
            if (viewLocationFormats != null)
            {
                list.AddRange(viewLocationFormats.Select(str2 => new ViewLocation(str2)));
            }
            return list;
        }

        private XslCompiledTransform GetCompiledTransform(ControllerContext controllerContext, IEnumerable<string> locations, string[] areaLocations, string locationsPropertyName, string name, string controllerName, string cacheKeyPrefix, out string[] searchedLocations)
        {
            searchedLocations = EmptyLocations;

            if (String.IsNullOrEmpty(name))
            {
                return null;
            }

            var areaName = GetAreaName(controllerContext.RouteData);
            var areaNamePresent = !string.IsNullOrEmpty(areaName);

            var viewLocations = GetViewLocations(locations, areaNamePresent ? areaLocations : null);
            if (viewLocations.Count == 0)
            {
                throw new InvalidOperationException(String.Format("The property '{0}' cannot be null or empty.", locationsPropertyName));
            }

            // Fix to cache generation as outlined at http://forums.asp.net/p/1316297/2604641.aspx
            string cacheKey = cacheKeyPrefix + areaName + controllerName + name;
            XslCompiledTransform result = CompiledTransformCache.Get(cacheKey);

            if (result != null)
            {
                // Spot "dummy" transforms that have not been loaded with style sheets
                // and don't return them. This is a work around for the fact we can't
                // store a null in the HttpContext.Current.Cache and tell the difference
                // between that and an entry not being there in the first place!
                if (result.OutputSettings == null)
                {
                    result = null;
                }

                return result;
            }

            var pathToXslt = IsSpecificPath(name) ? GetPathFromSpecificName(name, ref searchedLocations) : GetPathFromGeneralName(viewLocations, name, controllerName, areaName, ref searchedLocations);
            if (!string.IsNullOrEmpty(pathToXslt))
            {
                result = new XslCompiledTransform();
                result.Load(controllerContext.HttpContext.Server.MapPath(pathToXslt));
                CompiledTransformCache.Set(cacheKey, result);
            }
            else
            {
                // Save an unloaded compiled transform that has not been loaded with a style sheet,
                // So we don't go searching for this transform again
                CompiledTransformCache.Set(cacheKey, new XslCompiledTransform());
            }

            return result;
        }

        private string GetPathFromGeneralName(List<ViewLocation> locations, string name, string controllerName, string areaName, ref string[] searchedLocations)
        {
            string result = "";
            searchedLocations = new string[locations.Count];

            for (int i = 0; i < locations.Count; i++)
            {
                string virtualPath = locations[i].Format(name, controllerName, areaName);

                if (VirtualPathProvider.FileExists(virtualPath))
                {
                    searchedLocations = EmptyLocations;
                    result = virtualPath;
                    break;
                }

                searchedLocations[i] = virtualPath;
            }

            return result;
        }

        private string GetPathFromSpecificName(string name, ref string[] searchedLocations)
        {
            string result = name;

            if (!VirtualPathProvider.FileExists(name))
            {
                result = "";
                searchedLocations = new[] { name };
            }

            return result;
        }

        private static bool IsSpecificPath(string name)
        {
            char c = name[0];
            return (c == '~' || c == '/');
        }

        private class AspNetCacheCompiledTransformCache : ICompiledTransformCache
        {
            private readonly TimeSpan _slidingTimeout = new TimeSpan(0, 15, 0);

            public XslCompiledTransform Get(string cacheKey)
            {
                return (XslCompiledTransform)HttpContext.Current.Cache[cacheKey];
            }

            public void Set(string cacheKey, XslCompiledTransform xslCompiledTransform)
            {
                HttpContext.Current.Cache.Insert(cacheKey, xslCompiledTransform, null, Cache.NoAbsoluteExpiration, _slidingTimeout);
            }
        }

        private class NullCompiledTransformCache : ICompiledTransformCache
        {
            public XslCompiledTransform Get(string cacheKey)
            {
                return null;
            }

            public void Set(string cacheKey, XslCompiledTransform xslCompiledTransform)
            {
            }
        }

        internal interface ICompiledTransformCache
        {
            XslCompiledTransform Get(string cacheKey);
            void Set(string cacheKey, XslCompiledTransform xslCompiledTransform);
        }

        private class ViewLocation
        {
            // Fields
            protected readonly string VirtualPathFormatString;

            // Methods
            public ViewLocation(string virtualPathFormatString)
            {
                VirtualPathFormatString = virtualPathFormatString;
            }

            public virtual string Format(string viewName, string controllerName, string areaName)
            {
                return string.Format(CultureInfo.InvariantCulture, VirtualPathFormatString, new object[] { viewName, controllerName });
            }
        }

        private class AreaAwareViewLocation : ViewLocation
        {
            // Methods
            public AreaAwareViewLocation(string virtualPathFormatString)
                : base(virtualPathFormatString)
            {
            }

            public override string Format(string viewName, string controllerName, string areaName)
            {
                return string.Format(CultureInfo.InvariantCulture, VirtualPathFormatString, new object[] { viewName, controllerName, areaName });
            }
        }
    }
}