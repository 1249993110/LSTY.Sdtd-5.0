using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.MapRendering
{
    public class MapInfo
    {
        [JsonProperty("blockSize")]
        public int BlockSize { get; set; }

        [JsonProperty("maxZoom")]
        public int MaxZoom { get; set; }
    }
}
