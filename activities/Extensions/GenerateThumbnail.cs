
using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats;

namespace activities.Extensions
{
    public class GenerateThumbnail
    {
        //public static string GetReducedImageBase64(int width, int height, Stream resourceImage)
        //{
        //    try
        //    {
        //        using (var image = Image.Load(resourceImage))
        //        {
        //            image.Mutate(i => i.Resize(new ResizeOptions
        //            {
        //                Size = new Size(300, 300),
        //                Mode = ResizeMode.Crop
        //            }));
        //            string base64String;
        //            using (MemoryStream memoryStream = new MemoryStream())
        //            {
        //                image.Save(memoryStream, new JpegEncoder());
        //                byte[] imageBytes = memoryStream.ToArray();
        //                base64String = Convert.ToBase64String(imageBytes);
        //            }
        //            return base64String;

        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return null;
        //    }
        //}


        public static async Task<string> GetReducedImageBase64(IFormFile file)
        {
            try
            {
                using (var image = await Image.LoadAsync(file.OpenReadStream()))
                {
                    // Resize the image to 100x100 pixels and preserve the aspect ratio
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(200, 200),
                        Mode = ResizeMode.Max,
                    }));

                    // Save the thumbnail to a MemoryStream
                    using (var thumbnailStream = new MemoryStream())
                    {
                        await image.SaveAsync(thumbnailStream, new JpegEncoder());

                        // Convert the thumbnail to a base64 string
                        var thumbnailBytes = thumbnailStream.ToArray();
                        var thumbnailBase64 = Convert.ToBase64String(thumbnailBytes);
                        return thumbnailBase64;
                    }
                }

            }catch (Exception ex)
            {
                return null;
            }
        }
    }
}
