using ImageMagick;
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
using UnityEngine;
using Color = System.Drawing.Color;

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

                int index = iconName.LastIndexOf("__");
                string iconColor = null;
                if (index != -1)
                {
                    iconColor = iconName.Substring(index + 2, 6);
                    iconName = iconName.Remove(index) + ".png";
                }
                
                string iconPath = FindIconPath(iconName);
                if (iconPath == null)
                {
                    return new Response() { StatusCode = HttpStatusCode.NotFound };
                }
                
                if (iconColor == null)
                {
                    var stream = File.OpenRead(iconPath);

                    return Response.FromStream(stream, "image/png");
                }

                MemoryStream memoryStream = new MemoryStream();
                using (var image = new MagickImage(iconPath, MagickFormat.Png))
                {
                    using (var pc = image.GetPixels())
                    {
                        int r = Convert.ToInt32(iconColor.Substring(0, 2), 16);
                        int g = Convert.ToInt32(iconColor.Substring(2, 2), 16);
                        int b = Convert.ToInt32(iconColor.Substring(4, 2), 16);

                        foreach (Pixel p in pc)
                        {
                            p.SetChannel(0, (byte)(p[0] * r / 255));
                            p.SetChannel(1, (byte)(p[1] * g / 255));
                            p.SetChannel(2, (byte)(p[2] * b / 255));
                        }

                        image.Write(memoryStream, MagickFormat.Png);
                    }
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                return Response.FromStream(memoryStream, "image/png");

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
