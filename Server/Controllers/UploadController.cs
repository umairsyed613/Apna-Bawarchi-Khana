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
        public async Task Save(string path, IEnumerable<IFormFile> files)
        {
            // the default field name. See SaveField
            if (files == null || string.IsNullOrEmpty(path))
            {
                throw new InvalidOperationException("Missing Parameter values on saving file.");
            }
            else
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
        public Task Remove(string path, string[] files)
        {
            if (files == null || string.IsNullOrEmpty(path))
            {
                throw new InvalidOperationException("Missing Parameter values on removing file.");
            }
            else
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

                return Task.CompletedTask;
            }
        }
        
        [HttpPost]
        public Task RemoveByPath([FromBody] string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new InvalidOperationException("Missing Parameter values on removing file.");
            }
            else
            {
                var fullPath = _pathProvider.MapPath(path);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                else
                {
                    throw new FileNotFoundException("File cannot be deleted.", path);
                }

                return Task.CompletedTask;
            }
        }
    }
}
