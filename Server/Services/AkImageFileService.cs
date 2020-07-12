using System;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace ApnaBawarchiKhana.Server.Services
{
    public interface IAkImageFileService
    {
        Task<byte[]> GetFileAsBytes(string path, bool deleteFile = true);
        public byte[] ResizeImage(byte[] photoBytes);
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

        public byte[] ResizeImage(byte[] photoBytes)
        {
            using var outputStream = new MemoryStream();

            using var inputStream = new MemoryStream(photoBytes);
            using (var image = SixLabors.ImageSharp.Image.Load(inputStream))
            {
                image.Mutate(x => x.Resize(0, 600, KnownResamplers.Lanczos3).DrawText("ApnaBarwachiKhana", SixLabors.Fonts.SystemFonts.CreateFont("Arial", 14, SixLabors.Fonts.FontStyle.Regular), SixLabors.ImageSharp.Color.White, new SixLabors.ImageSharp.PointF(5, 5)));
                image.Save(outputStream, new JpegEncoder());
            }

            outputStream.Seek(0, SeekOrigin.Begin);

            return outputStream.ToArray();

            //ISupportedImageFormat format = new JpegFormat { Quality = 70 };
            //ResizeLayer resizeLayer = new ResizeLayer(new Size(640, 480))
            //{
            //    ResizeMode = ResizeMode.Min,
            //    MaxSize = new Size(640, 480)
            //};


            //using (MemoryStream inStream = new MemoryStream(photoBytes))
            //{
            //    using (MemoryStream outStream = new MemoryStream())
            //    {
            //        // Initialize the ImageFactory using the overload to preserve EXIF metadata.
            //        using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
            //        {
            //            // Load, resize, set the format and quality and save an image.
            //            imageFactory.Load(inStream)
            //                        .Resize(resizeLayer)
            //                        .Format(format)
            //                        .Watermark(new TextLayer{
            //                            Text = "ApnaBarwachiKhana",
            //                            FontColor = Color.White,
            //                            FontSize = 14,
            //                            Position = new Point(5, 5)
            //                        })
            //                        .Save(outStream);
            //        }
            //        // Do something with the stream.
            //        return outStream.ToArray();
            //    }
            //}
        }

    }
}
