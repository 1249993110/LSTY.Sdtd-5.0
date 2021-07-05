using Nancy;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using SkiaSharp;

namespace LSTY.Sdtd.PatronsMod.WebApi.Modules
{
    public class ItemIconsModule : NancyModule
    {
        public ItemIconsModule()
        {
            Get("/itemicons/{iconName}", _ =>
            {
                string iconName = _.iconName;
                if (iconName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) == false)
                {
                    return new Response() { StatusCode = HttpStatusCode.NotFound };
                }

                string iconColor = Request.Query["iconColor"];

                string iconPath = FindIconPath(iconName);
                if (iconPath == null)
                {
                    return new Response() { StatusCode = HttpStatusCode.NotFound };
                }

                Stream stream = File.OpenRead(iconPath);

                if (string.IsNullOrEmpty(iconColor))
                {
                    return Response.FromStream(stream, "image/png");
                }

                int r, g, b;
                try
                {
                    r = Convert.ToInt32(iconColor.Substring(0, 2), 16);
                    g = Convert.ToInt32(iconColor.Substring(2, 2), 16);
                    b = Convert.ToInt32(iconColor.Substring(4, 2), 16);
                }
                catch
                {
                    CustomLogger.Warn("Error in ItemIconsModule: Base conversion failed");
                    return new Response() { StatusCode = HttpStatusCode.BadRequest };
                }


                using (var image = SKBitmap.Decode(stream))
                {
                    int width = image.Width;
                    int height = image.Height;

                    for (int i = 0; i < width; ++i)
                    {
                        for (int j = 0; j < height; ++j)
                        {
                            var color = image.GetPixel(i, j);

                            image.SetPixel(i, j, new SKColor(
                                (byte)(color.Red * r / 255),
                                (byte)(color.Green * g / 255),
                                (byte)(color.Blue * b / 255),
                                color.Alpha));
                        }
                    }

                    stream = new MemoryStream();
                    using (SKManagedWStream wstream = new SKManagedWStream(stream))
                    {
                        image.PeekPixels().Encode(wstream, SKEncodedImageFormat.Png, 100);
                    }
                }

                stream.Seek(0, SeekOrigin.Begin);

                return Response.FromStream(stream, "image/png");

            }, null, "itemicons");
        }

        private static string FindIconPath(string iconName)
        {
            string path = "Data/ItemIcons/" + iconName;
            if (File.Exists(path))
            {
                return path;
            }

            foreach (Mod mod in ModManager.GetLoadedMods())
            {
                path = mod.Path + "/ItemIcons/" + iconName;
                if (File.Exists(path))
                {
                    return path;
                }

                foreach (string dir in Directory.GetDirectories(mod.Path))
                {
                    path = dir + "/" + iconName;
                    if (File.Exists(path))
                    {
                        return path;
                    }

                    foreach (string subDir in Directory.GetDirectories(dir))
                    {
                        path = subDir + "/" + iconName;
                        if (File.Exists(path))
                        {
                            return path;
                        }
                    }
                }
            }

            return null;
        }
    }
}
