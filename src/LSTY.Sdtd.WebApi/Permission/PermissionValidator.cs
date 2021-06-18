using IceCoffee.AspNetCore.Permission;
using LSTY.Sdtd.WebApi.Data.IRepositories;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LSTY.Sdtd.WebApi.Permission
{
    /// <summary>
    /// 许可校验器
    /// </summary>
    public class PermissionValidator : IPermissionValidator
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IPermissionCache _cache;

        public PermissionValidator(IPermissionRepository permissionRepository, IPermissionCache cache)
        {
            _permissionRepository = permissionRepository;
            _cache = cache;
        }

        public async Task<bool> CheckPermissionAsync(string roleId, string routePattern, PermissionType permissionTypeRequired)
        {
            routePattern = string.IsNullOrEmpty(routePattern) ? "/" : "/" + routePattern + "/";
            string key = roleId + ":" + routePattern;
            if (_cache.TryGetValue(key, out bool cacheEntry) == false)
            {
                // Key 不存在，查询数据库获取
                cacheEntry = await _permissionRepository.CheckPermissionAsync(roleId, (byte)permissionTypeRequired, routePattern);

                var cacheEntryOptions = new MemoryCacheEntryOptions();
                // 在缓存中保留此时间，暂定为 1 分钟，如果访问则重置时间
                cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(1D));
                // 设置绝对过期时间，暂定为 10 分钟，要实现精准的控制应在改动用户角色菜单关系时更新缓存
                cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10D);

                _cache.Set(key, cacheEntry, cacheEntryOptions);
            }

            return cacheEntry;
        }
    }
}
