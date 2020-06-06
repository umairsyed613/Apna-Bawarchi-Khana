using System;
using System.IO;
using System.Threading.Tasks;

namespace AmnasKitchen.Server.Services
{
    public interface IAkImageFileService
    {
        Task<byte[]> GetFileAsBytes(string path, bool deleteFile = true);
    }

    public class AkImageFileService : IAkImageFileService
    {
        private readonly IPathProvider _pathProvider;

        public AkImageFileService(IPathProvider pathProvider)
        {
            _pathProvider = pathProvider ?? throw new ArgumentNullException(nameof(pathProvider));
        }

        public async Task<byte[]> GetFileAsBytes(string path, bool deleteFile = true)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            var physicalPath = _pathProvider.MapPath(path);

            if (!File.Exists(physicalPath))
            {
                throw new FileNotFoundException(physicalPath + " doesnt exists on provided path");
            }

            if (deleteFile)
            {
                var bytes = await File.ReadAllBytesAsync(physicalPath);
                File.Delete(physicalPath);

                return bytes;
            }


            return await File.ReadAllBytesAsync(physicalPath);
        }
    }
}
