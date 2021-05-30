using ImageMagick;
using UnityEngine;

namespace LSTY.Sdtd.PatronsMod.WebApi.MapRendering
{
    public class Constants
    {
        public static readonly TextureFormat DefaultTexFormat = TextureFormat.ARGB32;
        public static int MapBlockSize = 128;
        public const int MapChunkSize = 16;
        public const int MapRegionSize = 512;
        public static int ZoomLevels = 5;
        public static string MapDirectory = string.Empty;

        public static int MapBlockToChunkDiv => MapBlockSize / MapChunkSize;

        public static int MapRegionToChunkDiv => MapRegionSize / MapChunkSize;
    }
}