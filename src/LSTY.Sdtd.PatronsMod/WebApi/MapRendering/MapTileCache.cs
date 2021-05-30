using ImageMagick;
using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LSTY.Sdtd.PatronsMod.WebApi.MapRendering
{
    /// <summary>
    /// Special "cache" for map tile folder as both map rendering and webserver access files in there.
    /// Only map rendering tiles are cached. Writing is done by WriteThrough.
    /// </summary>
    public class MapTileCache
    {
        private readonly byte[] _transparentTile;
        private CurrentZoomFile[] _cache;

        public MapTileCache(int tileSize)
        {
            Texture2D tex = new Texture2D(tileSize, tileSize);
            Color nullColor = new Color(0, 0, 0, 0);
            for (int x = 0; x < tileSize; x++)
            {
                for (int y = 0; y < tileSize; y++)
                {
                    tex.SetPixel(x, y, nullColor);
                }
            }

            _transparentTile = tex.EncodeToPNG();
            Object.Destroy(tex);
        }

        public void SetZoomCount(int count)
        {
            _cache = new CurrentZoomFile[count];
            for (int i = 0; i < _cache.Length; i++)
            {
                _cache[i] = new CurrentZoomFile();
            }
        }

        public byte[] LoadTile(int zoomLevel, string fileName)
        {
            try
            {
                lock (_cache)
                {
                    CurrentZoomFile cacheEntry = _cache[zoomLevel];

                    if (cacheEntry.Filename == null || !cacheEntry.Filename.Equals(fileName))
                    {
                        cacheEntry.Filename = fileName;

                        if (!File.Exists(fileName))
                        {
                            cacheEntry.PngData = null;
                            return null;
                        }

                        cacheEntry.PngData = ReadAllBytes(fileName);
                    }

                    return cacheEntry.PngData;
                }
            }
            catch (Exception e)
            {
                CustomLogger.Warn("Error in MapTileCache.LoadTile: " + e);
            }

            return null;
        }

        public void SaveTile(int zoomLevel, byte[] contentPng)
        {
            try
            {
                lock (_cache)
                {
                    CurrentZoomFile cacheEntry = _cache[zoomLevel];

                    string file = cacheEntry.Filename;
                    if (string.IsNullOrEmpty(file))
                    {
                        return;
                    }

                    cacheEntry.PngData = contentPng;

                    using (Stream stream = new FileStream(file, FileMode.Create, FileAccess.ReadWrite, FileShare.None,
                        4096))
                    {
                        stream.Write(contentPng, 0, contentPng.Length);
                    }
                }
            }
            catch (Exception e)
            {
                CustomLogger.Warn("Error in MapTileCache.SaveTile: " + e);
            }
        }

        public void ResetTile(int zoomLevel)
        {
            try
            {
                lock (_cache)
                {
                    _cache[zoomLevel].Filename = null;
                    _cache[zoomLevel].PngData = null;
                }
            }
            catch (Exception e)
            {
                CustomLogger.Warn("Error in MapTileCache.ResetTile: " + e);
            }
        }

        public byte[] GetFileContent(string fileName)
        {
            try
            {
                lock (_cache)
                {
                    foreach (CurrentZoomFile czf in _cache)
                    {
                        if (czf.Filename != null && czf.Filename.Equals(fileName))
                        {
                            return czf.PngData;
                        }
                    }

                    if (File.Exists(fileName) == false)
                    {
                        return _transparentTile;
                    }

                    return ReadAllBytes(fileName);
                }
            }
            catch (Exception e)
            {
                CustomLogger.Warn("Error in MapTileCache.GetFileContent: " + e);
            }

            return null;
        }

        private static byte[] ReadAllBytes(string path)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096))
            {
                int bytesRead = 0;
                int bytesLeft = (int)fileStream.Length;
                byte[] result = new byte[bytesLeft];
                while (bytesLeft > 0)
                {
                    int readThisTime = fileStream.Read(result, bytesRead, bytesLeft);
                    if (readThisTime == 0)
                    {
                        throw new IOException("Unexpected end of stream");
                    }

                    bytesRead += readThisTime;
                    bytesLeft -= readThisTime;
                }

                return result;
            }
        }


        private class CurrentZoomFile
        {
            public string Filename;
            public byte[] PngData;
        }
    }
}
