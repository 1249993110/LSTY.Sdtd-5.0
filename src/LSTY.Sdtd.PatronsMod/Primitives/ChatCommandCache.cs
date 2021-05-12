using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.Primitives
{
    static class ChatCommandCache
    {
        class CacheItem
        {
            public DateTime ReadTime;
            public ChatHook ChatHook;
        }

        //public static string GetKey(string message)
        //{
        //    int index = message.IndexOf(" ");
        //    return index == -1 ? message : message.Substring(0, index);
        //}

        private static Dictionary<string, CacheItem> _cache = new Dictionary<string, CacheItem>();

        public static void Set(string key, ChatHook chatHook)
        {
            if(key.Length > 10)
            {
                return;
            }

            if(_cache.Count > FunctionManager.CommonConfig.ChatCommandCacheMaxCount)
            {
                string _key = null;
                DateTime min = DateTime.MaxValue;
                foreach (var item in _cache)
                {
                    if(item.Value.ReadTime < min)
                    {
                        min = item.Value.ReadTime;
                        _key = item.Key;
                    }
                }

                _cache.Remove(_key);
            }

            _cache.Add(key, new CacheItem() { ReadTime = DateTime.Now, ChatHook = chatHook });
        }

        public static ChatHook Get(string key)
        {
            if (key.Length > 10 || _cache.ContainsKey(key) == false)
            {
                return null;
            }

            var cacheItem = _cache[key];

            cacheItem.ReadTime = DateTime.Now;

            return cacheItem.ChatHook;
        }

        public static void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
