using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SkiaSharp;

namespace LSTY.Sdtd.WebApi.Utils
{
    public interface ICaptchaGenerator
    {
        byte[] GenerateImageAsByteArray(string captchaCode, SKEncodedImageFormat imageFormat = SKEncodedImageFormat.Png, int imageQuality = 100);
        Stream GenerateImageAsStream(string captchaCode, SKEncodedImageFormat imageFormat = SKEncodedImageFormat.Png, int imageQuality = 100);
    }
}
