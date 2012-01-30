using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace NServiceMVC.WebStack
{
    /// <remarks>from http://stackoverflow.com/questions/7746253/using-custom-virtualpathprovider-to-load-embedded-resource-partial-views
    /// </remarks>
    public class NsVirtualPathProvider : VirtualPathProvider
    {
        public NsVirtualPathProvider() : base() { }

        private bool IsEmbeddedResourcePath(string virtualPath)
        {
            
            var checkPath = VirtualPathUtility.ToAppRelative(virtualPath);
            return checkPath.StartsWith(NServiceMVC.VirtualPathPrefix, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool FileExists(string virtualPath)
        {
            return IsEmbeddedResourcePath(virtualPath) || base.FileExists(virtualPath);
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            if (IsEmbeddedResourcePath(virtualPath))
            {
                return new NsVirtualFile(virtualPath);
            }
            else
            {
                return base.GetFile(virtualPath);
            }
        }

        public override System.Web.Caching.CacheDependency GetCacheDependency(string virtualPath, System.Collections.IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (IsEmbeddedResourcePath(virtualPath))
            {
                return null;
            }
            else
            {
                return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
            }
        }


        class NsVirtualFile : VirtualFile
        {
            private string relativeVirtualPath;

            public NsVirtualFile(string virtualPath)
                : base(virtualPath)
            {
                relativeVirtualPath = VirtualPathUtility.ToAppRelative(virtualPath);
                if (!relativeVirtualPath.StartsWith(NServiceMVC.VirtualPathPrefix))
                {
                    // guard clause
                    throw new ArgumentException(String.Format("virtualPath '{0}' is not in the required prefix {1} for this path provider.", virtualPath, NServiceMVC.VirtualPathPrefix));
                }
            }

            public override System.IO.Stream Open()
            {
                var parts = relativeVirtualPath.Split('/');
                //var assemblyName = parts[1];
                var resourceName = parts[2];

                //assemblyName = System.IO.Path.Combine(HttpRuntime.BinDirectory, assemblyName);
                //var assembly = System.Reflection.Assembly.LoadFile(assemblyName + ".dll");
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();

                if (assembly != null)
                {
                    return assembly.GetManifestResourceStream(resourceName);
                }
                return null;
            }
        }

    }
}
