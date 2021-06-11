using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace LSTY.Sdtd.PatronsMod.MapRendering
{
    public class MapRender
    {
        private static MapRender _instance;

        private static readonly object _lockObject = new object();
        public static bool RenderingEnabled = true;
        private readonly MapTileCache _cache = new MapTileCache(Constants.MapBlockSize);
        private readonly Dictionary<Vector2i, Color32[]> _dirtyChunks = new Dictionary<Vector2i, Color32[]>();
        private readonly MicroStopwatch _msw = new MicroStopwatch();
        private readonly MapRenderBlockBuffer[] _zoomLevelBuffers;
        private Coroutine _renderCoroutineRef;
        private bool _renderingFullMap;
        private float _renderTimeout = float.MaxValue;

        private readonly List<Vector2i> _chunksToRender = new List<Vector2i>();
        private readonly List<Vector2i> _chunksRendered = new List<Vector2i>();

        private MapRender()
        {
            Constants.MapDirectory = GameUtils.GetSaveGameDir() + "/LSTY/map";

            lock (_lockObject)
            {
                if (LoadMapInfo() == false)
                {
                    WriteMapInfo();
                }
            }

            _cache.SetZoomCount(Constants.ZoomLevels);

            _zoomLevelBuffers = new MapRenderBlockBuffer[Constants.ZoomLevels];
            for (int i = 0; i < Constants.ZoomLevels; i++)
            {
                _zoomLevelBuffers[i] = new MapRenderBlockBuffer(i, _cache);
            }

            _renderCoroutineRef = ThreadManager.StartCoroutine(RenderCoroutine());
        }

        public static void Init()
        {
            if (_instance == null)
            {
                _instance = new MapRender();
            }
        }

       public static MapRender Instance
        {
            get
            {
                return _instance;
            }
        }

        public static MapTileCache GetTileCache()
        {
            return Instance._cache;
        }

        public static void Shutdown()
        {
            if (Instance._renderCoroutineRef != null)
            {
                ThreadManager.StopCoroutine(Instance._renderCoroutineRef);
                Instance._renderCoroutineRef = null;
            }
        }

        public static void RenderSingleChunk(Chunk chunk)
        {
            if (RenderingEnabled)
            {
                // TODO: Replace with regular thread and a blocking queue / set
                ThreadPool.UnsafeQueueUserWorkItem(obj =>
                {
                    try
                    {
                        if (!Instance._renderingFullMap)
                        {
                            lock (_lockObject)
                            {
                                Chunk c = obj as Chunk;
                                Vector3i cPos = c.GetWorldPos();
                                Vector2i cPos2 = new Vector2i(cPos.x / Constants.MapChunkSize,
                                    cPos.z / Constants.MapChunkSize);

                                ushort[] mapColors = c.GetMapColors();
                                if (mapColors != null)
                                {
                                    Color32[] realColors =
                                        new Color32[Constants.MapChunkSize * Constants.MapChunkSize];
                                    for (int i_colors = 0; i_colors < mapColors.Length; i_colors++)
                                    {
                                        realColors[i_colors] = ShortColorToColor32(mapColors[i_colors]);
                                    }

                                    Instance._dirtyChunks[cPos2] = realColors;

                                    //CustomLogger.Info ("Add Dirty: " + cPos2);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        CustomLogger.Info("Exception in MapRendering.RenderSingleChunk(): " + e);
                    }
                }, chunk);
            }
        }

        public void RenderFullMap()
        {
            MicroStopwatch microStopwatch = new MicroStopwatch();

            string regionSaveDir = GameUtils.GetSaveGameRegionDir();
            RegionFileManager rfm = new RegionFileManager(regionSaveDir, regionSaveDir, 0, false);
            Texture2D fullMapTexture = null;

            Vector2i minChunk, maxChunk;
            Vector2i minPos, maxPos;
            int widthChunks, heightChunks, widthPix, heightPix;
            GetWorldExtent(rfm, out minChunk, out maxChunk, out minPos, out maxPos, out widthChunks, out heightChunks,
                out widthPix, out heightPix);

            CustomLogger.Info(string.Format(
                "RenderMap: min: {0}, max: {1}, minPos: {2}, maxPos: {3}, w/h: {4}/{5}, wP/hP: {6}/{7}",
                minChunk.ToString(), maxChunk.ToString(),
                minPos.ToString(), maxPos.ToString(),
                widthChunks, heightChunks,
                widthPix, heightPix)
            );

            lock (_lockObject)
            {
                for (int i = 0; i < Constants.ZoomLevels; i++)
                {
                    _zoomLevelBuffers[i].ResetBlock();
                }

                if (Directory.Exists(Constants.MapDirectory))
                {
                    Directory.Delete(Constants.MapDirectory, true);
                }

                WriteMapInfo();

                _renderingFullMap = true;

                if (widthPix <= 8192 && heightPix <= 8192)
                {
                    fullMapTexture = new Texture2D(widthPix, heightPix);
                }

                Vector2i curFullMapPos = default(Vector2i);
                Vector2i curChunkPos = default(Vector2i);
                for (curFullMapPos.x = 0; curFullMapPos.x < widthPix; curFullMapPos.x += Constants.MapChunkSize)
                {
                    for (curFullMapPos.y = 0;
                        curFullMapPos.y < heightPix;
                        curFullMapPos.y += Constants.MapChunkSize)
                    {
                        curChunkPos.x = curFullMapPos.x / Constants.MapChunkSize + minChunk.x;
                        curChunkPos.y = curFullMapPos.y / Constants.MapChunkSize + minChunk.y;

                        try
                        {
                            long chunkKey = WorldChunkCache.MakeChunkKey(curChunkPos.x, curChunkPos.y);
                            if (rfm.ContainsChunkSync(chunkKey))
                            {
                                Chunk c = rfm.GetChunkSync(chunkKey);
                                ushort[] mapColors = c.GetMapColors();
                                if (mapColors != null)
                                {
                                    Color32[] realColors =
                                        new Color32[Constants.MapChunkSize * Constants.MapChunkSize];
                                    for (int i_colors = 0; i_colors < mapColors.Length; i_colors++)
                                    {
                                        realColors[i_colors] = ShortColorToColor32(mapColors[i_colors]);
                                    }

                                    _dirtyChunks[curChunkPos] = realColors;
                                    if (fullMapTexture != null)
                                    {
                                        fullMapTexture.SetPixels32(curFullMapPos.x, curFullMapPos.y,
                                            Constants.MapChunkSize, Constants.MapChunkSize, realColors);
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            CustomLogger.Info("Exception: " + e);
                        }
                    }

                    while (_dirtyChunks.Count > 0)
                    {
                        RenderDirtyChunks();
                    }

                    CustomLogger.Info(string.Format("RenderMap: {0}/{1} ({2}%)", curFullMapPos.x, widthPix,
                        (int)((float)curFullMapPos.x / widthPix * 100)));
                }
            }

            rfm.Cleanup();

            if (fullMapTexture != null)
            {
                byte[] array = fullMapTexture.EncodeToPNG();
                File.WriteAllBytes(Constants.MapDirectory + "/map.png", array);
                Object.Destroy(fullMapTexture);
            }

            _renderingFullMap = false;

            CustomLogger.Info("Generating map took: " + microStopwatch.ElapsedMilliseconds + " ms");
            CustomLogger.Info("World extent: " + minPos + " - " + maxPos);
        }

        private void SaveAllBlockMaps()
        {
            for (int i = 0; i < Constants.ZoomLevels; i++)
            {
                _zoomLevelBuffers[i].SaveBlock();
            }
        }

        private readonly WaitForSeconds coroutineDelay = new WaitForSeconds(0.2f);

        private IEnumerator RenderCoroutine()
        {
            while (true)
            {
                lock (_lockObject)
                {
                    if (_dirtyChunks.Count > 0 && _renderTimeout == float.MaxValue)
                    {
                        _renderTimeout = Time.time + 0.5f;
                    }

                    if (Time.time > _renderTimeout || _dirtyChunks.Count > 200)
                    {
                        Profiler.BeginSample("RenderDirtyChunks");
                        RenderDirtyChunks();
                        Profiler.EndSample();
                    }
                }

                yield return coroutineDelay;
            }
        }

        private void RenderDirtyChunks()
        {
            _msw.ResetAndRestart();

            if (_dirtyChunks.Count <= 0)
            {
                return;
            }

            Profiler.BeginSample("RenderDirtyChunks.Prepare");
            _chunksToRender.Clear();
            _chunksRendered.Clear();

            _dirtyChunks.CopyKeysTo(_chunksToRender);

            Vector2i chunkPos = _chunksToRender[0];
            _chunksRendered.Add(chunkPos);

            //CustomLogger.Info ("Start Dirty: " + chunkPos);

            Vector2i block, blockOffset;
            GetBlockNumber(chunkPos, out block, out blockOffset, Constants.MapBlockToChunkDiv,
                Constants.MapChunkSize);

            _zoomLevelBuffers[Constants.ZoomLevels - 1].LoadBlock(block);
            Profiler.EndSample();

            Profiler.BeginSample("RenderDirtyChunks.Work");
            // Write all chunks that are in the same image tile of the highest zoom level 
            Vector2i v_block, v_blockOffset;
            foreach (Vector2i v in _chunksToRender)
            {
                GetBlockNumber(v, out v_block, out v_blockOffset, Constants.MapBlockToChunkDiv,
                    Constants.MapChunkSize);
                if (v_block.Equals(block))
                {
                    //CustomLogger.Info ("Dirty: " + v + " render: true");
                    _chunksRendered.Add(v);
                    if (_dirtyChunks[v].Length != Constants.MapChunkSize * Constants.MapChunkSize)
                    {
                        CustomLogger.Error(string.Format("Rendering chunk has incorrect data size of {0} instead of {1}",
                            _dirtyChunks[v].Length, Constants.MapChunkSize * Constants.MapChunkSize));
                    }

                    _zoomLevelBuffers[Constants.ZoomLevels - 1]
                        .SetPart(v_blockOffset, Constants.MapChunkSize, _dirtyChunks[v]);
                }
            }
            Profiler.EndSample();

            foreach (Vector2i v in _chunksRendered)
            {
                _dirtyChunks.Remove(v);
            }

            // Update lower zoom levels affected by the change of the highest one
            RenderZoomLevel(block);

            Profiler.BeginSample("RenderDirtyChunks.SaveAll");
            SaveAllBlockMaps();
            Profiler.EndSample();
        }

        private void RenderZoomLevel(Vector2i innerBlock)
        {
            Profiler.BeginSample("RenderZoomLevel");
            int level = Constants.ZoomLevels - 1;
            while (level > 0)
            {
                Vector2i block, blockOffset;
                GetBlockNumber(innerBlock, out block, out blockOffset, 2, Constants.MapBlockSize / 2);

                _zoomLevelBuffers[level - 1].LoadBlock(block);

                Profiler.BeginSample("RenderZoomLevel.Transfer");
                if ((_zoomLevelBuffers[level].FormatSelf == TextureFormat.ARGB32 ||
                     _zoomLevelBuffers[level].FormatSelf == TextureFormat.RGBA32) &&
                    _zoomLevelBuffers[level].FormatSelf == _zoomLevelBuffers[level - 1].FormatSelf)
                {
                    _zoomLevelBuffers[level - 1].SetPartNative(blockOffset, Constants.MapBlockSize / 2, _zoomLevelBuffers[level].GetHalfScaledNative());
                }
                else
                {
                    _zoomLevelBuffers[level - 1].SetPart(blockOffset, Constants.MapBlockSize / 2, _zoomLevelBuffers[level].GetHalfScaled());
                }
                Profiler.EndSample();

                level--;
                innerBlock = block;
            }
            Profiler.EndSample();
        }

        private void GetBlockNumber(Vector2i innerPos, out Vector2i block, out Vector2i blockOffset, int scaleFactor, int offsetSize)
        {
            block = default(Vector2i);
            blockOffset = default(Vector2i);
            block.x = (innerPos.x + 16777216) / scaleFactor - 16777216 / scaleFactor;
            block.y = (innerPos.y + 16777216) / scaleFactor - 16777216 / scaleFactor;
            blockOffset.x = (innerPos.x + 16777216) % scaleFactor * offsetSize;
            blockOffset.y = (innerPos.y + 16777216) % scaleFactor * offsetSize;
        }

        private void WriteMapInfo()
        {
            MapInfo mapInfo = new MapInfo()
            {
                BlockSize = Constants.MapBlockSize,
                MaxZoom = Constants.ZoomLevels - 1
            };

            Directory.CreateDirectory(Constants.MapDirectory);
            File.WriteAllText(Constants.MapDirectory + "/mapinfo.json", JsonConvert.SerializeObject(mapInfo), Encoding.UTF8);
        }

        private bool LoadMapInfo()
        {
            try
            {
                string mapInfoPath = Constants.MapDirectory + "/mapinfo.json";
                if (File.Exists(mapInfoPath) == false)
                {
                    return false;
                }

                string json = File.ReadAllText(mapInfoPath, Encoding.UTF8);
                MapInfo mapInfo = JsonConvert.DeserializeObject<MapInfo>(json);
                Constants.MapBlockSize = mapInfo.BlockSize;
                Constants.ZoomLevels = mapInfo.MaxZoom + 1;
                return true;
            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Error in LoadMapInfo");
                return false;
            }
        }

        private void GetWorldExtent(RegionFileManager rfm,
            out Vector2i minChunk, out Vector2i maxChunk,
            out Vector2i minPos, out Vector2i maxPos,
            out int widthChunks, out int heightChunks,
            out int widthPix, out int heightPix)
        {
            minChunk = default(Vector2i);
            maxChunk = default(Vector2i);
            minPos = default(Vector2i);
            maxPos = default(Vector2i);

            long[] keys = rfm.GetAllChunkKeys();
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;
            foreach (long key in keys)
            {
                int x = WorldChunkCache.extractX(key);
                int y = WorldChunkCache.extractZ(key);

                if (x < minX)
                {
                    minX = x;
                }

                if (x > maxX)
                {
                    maxX = x;
                }

                if (y < minY)
                {
                    minY = y;
                }

                if (y > maxY)
                {
                    maxY = y;
                }
            }

            minChunk.x = minX;
            minChunk.y = minY;

            maxChunk.x = maxX;
            maxChunk.y = maxY;

            minPos.x = minX * Constants.MapChunkSize;
            minPos.y = minY * Constants.MapChunkSize;

            maxPos.x = maxX * Constants.MapChunkSize;
            maxPos.y = maxY * Constants.MapChunkSize;

            widthChunks = maxX - minX + 1;
            heightChunks = maxY - minY + 1;

            widthPix = widthChunks * Constants.MapChunkSize;
            heightPix = heightChunks * Constants.MapChunkSize;
        }

        private static Color32 ShortColorToColor32(ushort col)
        {
            byte r = (byte)(256 * ((col >> 10) & 31) / 32);
            byte g = (byte)(256 * ((col >> 5) & 31) / 32);
            byte b = (byte)(256 * (col & 31) / 32);
            const byte a = 255;
            return new Color32(r, g, b, a);
        }
    }
}