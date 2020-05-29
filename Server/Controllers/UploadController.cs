using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using AmnasKitchen.Server.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AmnasKitchen.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    public class UploadController : Controller
    {
        private readonly IPathProvider _pathProvider;

        public UploadController(IPathProvider pathProvider)
        {
            _pathProvider = pathProvider;
        }

        [HttpPost]
        public async Task Save(IEnumerable<IFormFile> files, string path)
        {
            // the default field name. See SaveField
            if (files != null && !string.IsNullOrEmpty(path))
            {
                foreach (var file in files)
                {
                    var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                    var fileName = Path.GetFileName(fileContent.FileName.ToString().Trim('"'));
                    var physicalPath = Path.Combine(_pathProvider.MapPath(path), fileName);
                    await using var fileStream = new FileStream(physicalPath, FileMode.Create);
                    await file.CopyToAsync(fileStream);
                }
            }
        }

        [HttpPost]
        public ActionResult Remove(string[] files, string path)
        {
            // the default field name. See RemoveField
            if (files != null && !string.IsNullOrEmpty(path))
            {
                try
                {
                    foreach (var fullName in files)
                    {
                        var fileName = Path.GetFileName(fullName);
                        var physicalPath = Path.Combine(_pathProvider.MapPath(path), fileName);

                        if (System.IO.File.Exists(physicalPath))
                        {
                            System.IO.File.Delete(physicalPath);
                        }
                    }
                }
                catch (Exception ee)
                {
                    // implement error handling here, this merely indicates a failure to the upload
                    Response.StatusCode = 500;
                    Response.WriteAsync(ee.Message); // custom error message
                }
            }

            // Return an empty string message in this case
            return new EmptyResult();
        }
    }
}
