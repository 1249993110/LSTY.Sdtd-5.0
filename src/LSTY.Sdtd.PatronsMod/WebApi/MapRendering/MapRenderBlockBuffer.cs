using ImageMagick;
using System;
using System.IO;
using Unity.Collections;
using UnityEngine;

namespace LSTY.Sdtd.PatronsMod.WebApi.MapRendering
{
    public class MapRenderBlockBuffer
    {
        private readonly Texture2D _blockMap = new Texture2D(Constants.MapBlockSize, Constants.MapBlockSize, Constants.DefaultTexFormat, false);
        private readonly MapTileCache _cache;
        private readonly NativeArray<int> _emptyImageData;
        private readonly Texture2D _zoomBuffer = new Texture2D(Constants.MapBlockSize / 2, Constants.MapBlockSize / 2, Constants.DefaultTexFormat, false);
        private readonly int _zoomLevel;
        private readonly string _folderBase;

        private Vector2i _currentBlockMapPos = new Vector2i(Int32.MinValue, Int32.MinValue);
        private string _currentBlockMapFolder = string.Empty;

        public MapRenderBlockBuffer(int level, MapTileCache cache)
        {
            _zoomLevel = level;
            this._cache = cache;
            _folderBase = Constants.MapDirectory + "/" + _zoomLevel + "/";

            {
                // Initialize empty tile data
                Color nullColor = new Color(0, 0, 0, 0);
                for (int x = 0; x < Constants.MapBlockSize; x++)
                {
                    for (int y = 0; y < Constants.MapBlockSize; y++)
                    {
                        _blockMap.SetPixel(x, y, nullColor);
                    }
                }

                NativeArray<int> blockMapData = _blockMap.GetRawTextureData<int>();
                _emptyImageData = new NativeArray<int>(blockMapData.Length, Allocator.Persistent,
                    NativeArrayOptions.UninitializedMemory);
                blockMapData.CopyTo(_emptyImageData);
            }
        }

        public TextureFormat FormatSelf
        {
            get { return _blockMap.format; }
        }

        public void ResetBlock()
        {
            _currentBlockMapFolder = string.Empty;
            _currentBlockMapPos = new Vector2i(Int32.MinValue, Int32.MinValue);
            _cache.ResetTile(_zoomLevel);
        }

        public void SaveBlock()
        {
            try
            {
                SaveTextureToFile();
            }
            catch (Exception e)
            {
                CustomLogger.Warn("Exception in MapRenderBlockBuffer.SaveBlock(): " + e);
            }
        }

        public bool LoadBlock(Vector2i block)
        {
            lock (_blockMap)
            {
                if (_currentBlockMapPos != block)
                {
                    string folder;
                    if (_currentBlockMapPos.x != block.x)
                    {
                        folder = _folderBase + block.x + '/';

                        Directory.CreateDirectory(folder);
                    }
                    else
                    {
                        folder = _currentBlockMapFolder;
                    }

                    string fileName = folder + block.y + ".png";

                    SaveBlock();
                    LoadTextureFromFile(fileName);

                    _currentBlockMapFolder = folder;
                    _currentBlockMapPos = block;

                    return true;
                }
            }

            return false;
        }

        public void SetPart(Vector2i offset, int partSize, Color32[] pixels)
        {
            if (offset.x + partSize > Constants.MapBlockSize || offset.y + partSize > Constants.MapBlockSize)
            {
                CustomLogger.Error(string.Format("MapBlockBuffer[{0}].SetPart ({1}, {2}, {3}) has blockMap.size ({4}/{5})",
                    _zoomLevel, offset, partSize, pixels.Length, Constants.MapBlockSize, Constants.MapBlockSize));
                return;
            }

            _blockMap.SetPixels32(offset.x, offset.y, partSize, partSize, pixels);
        }

        public Color32[] GetHalfScaled()
        {
            _zoomBuffer.Resize(Constants.MapBlockSize, Constants.MapBlockSize);

            if (_blockMap.format == _zoomBuffer.format)
            {
                NativeArray<byte> dataSrc = _blockMap.GetRawTextureData<byte>();
                NativeArray<byte> dataZoom = _zoomBuffer.GetRawTextureData<byte>();
                dataSrc.CopyTo(dataZoom);
            }
            else
            {
                _zoomBuffer.SetPixels32(_blockMap.GetPixels32());
            }

            TextureScale.Point(_zoomBuffer, Constants.MapBlockSize / 2, Constants.MapBlockSize / 2);

            Color32[] result = _zoomBuffer.GetPixels32();

            return result;
        }

        public void SetPartNative(Vector2i offset, int partSize, NativeArray<int> pixels)
        {
            if (offset.x + partSize > Constants.MapBlockSize || offset.y + partSize > Constants.MapBlockSize)
            {
                CustomLogger.Error(string.Format("MapBlockBuffer[{0}].SetPart ({1}, {2}, {3}) has blockMap.size ({4}/{5})",
                    _zoomLevel, offset, partSize, pixels.Length, Constants.MapBlockSize, Constants.MapBlockSize));
                return;
            }

            NativeArray<int> destData = _blockMap.GetRawTextureData<int>();

            for (int y = 0; y < partSize; y++)
            {
                int srcLineStartIdx = partSize * y;
                int destLineStartIdx = _blockMap.width * (offset.y + y) + offset.x;
                for (int x = 0; x < partSize; x++)
                {
                    destData[destLineStartIdx + x] = pixels[srcLineStartIdx + x];
                }
            }
        }

        public NativeArray<int> GetHalfScaledNative()
        {
            if (_zoomBuffer.format != _blockMap.format || _zoomBuffer.height != Constants.MapBlockSize / 2 || _zoomBuffer.width != Constants.MapBlockSize / 2)
            {
                _zoomBuffer.Resize(Constants.MapBlockSize / 2, Constants.MapBlockSize / 2, _blockMap.format, false);
            }

            ScaleNative(_blockMap, _zoomBuffer);

            return _zoomBuffer.GetRawTextureData<int>();
        }

        private static void ScaleNative(Texture2D sourceTex, Texture2D targetTex)
        {
            NativeArray<int> srcData = sourceTex.GetRawTextureData<int>();
            NativeArray<int> targetData = targetTex.GetRawTextureData<int>();

            int oldWidth = sourceTex.width;
            int oldHeight = sourceTex.height;
            int newWidth = targetTex.width;
            int newHeight = targetTex.height;

            float ratioX = ((float)oldWidth) / newWidth;
            float ratioY = ((float)oldHeight) / newHeight;

            for (var y = 0; y < newHeight; y++)
            {
                var oldLineStart = (int)(ratioY * y) * oldWidth;
                var newLineStart = y * newWidth;
                for (var x = 0; x < newWidth; x++)
                {
                    targetData[newLineStart + x] = srcData[(int)(oldLineStart + ratioX * x)];
                }
            }
        }

        private void LoadTextureFromFile(string fileName)
        {
            byte[] array = _cache.LoadTile(_zoomLevel, fileName);

            if (array != null && _blockMap.LoadImage(array) && _blockMap.height == Constants.MapBlockSize &&
                _blockMap.width == Constants.MapBlockSize)
            {
                return;
            }

            if (array != null)
            {
                CustomLogger.Error("Map image tile " + fileName + " has been corrupted, recreating tile");
            }

            if (_blockMap.format != Constants.DefaultTexFormat || _blockMap.height != Constants.MapBlockSize ||
                _blockMap.width != Constants.MapBlockSize)
            {
                _blockMap.Resize(Constants.MapBlockSize, Constants.MapBlockSize, Constants.DefaultTexFormat,
                    false);
            }

            _blockMap.LoadRawTextureData(_emptyImageData);

        }

        private void SaveTextureToFile()
        {
            byte[] array = _blockMap.EncodeToPNG();

            _cache.SaveTile(_zoomLevel, array);
        }
    }
}