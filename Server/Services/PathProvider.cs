using Microsoft.AspNetCore.Hosting;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ApnaBawarchiKhana.Server.Services
{
    public interface IPathProvider
    {
        string MapPath(string path);

        string GetRootPath();
    }

    public class PathProvider : IPathProvider
    {
        private IWebHostEnvironment _hostingEnvironment;

        public PathProvider(IWebHostEnvironment environment)
        {
            _hostingEnvironment = environment;
        }

        public string MapPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (path.StartsWith("\\") || path.StartsWith("/"))
            {
                path = path.Substring(1, path.Length - 1);
            }

            var rootPath = GetRootPath();
            var filePath = Path.Combine(rootPath, path);

            return filePath;
        }

        public string GetRootPath()
        {
#if DEBUG
            return GetApplicationRoot();
#else
            return _hostingEnvironment.WebRootPath;
#endif
        }

        public string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(System.Reflection
                                                      .Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return appRoot;
        }
    }
}
