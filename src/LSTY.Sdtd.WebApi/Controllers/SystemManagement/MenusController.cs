using IceCoffee.AspNetCore;
using IceCoffee.AspNetCore.Controllers;
using IceCoffee.AspNetCore.Models;
using IceCoffee.AspNetCore.Models.Primitives;
using LSTY.Sdtd.WebApi.Data.Entities;
using LSTY.Sdtd.WebApi.Data.IRepositories;
using LSTY.Sdtd.WebApi.Data.Primitives;
using LSTY.Sdtd.WebApi.Models.SystemManagement;
using LSTY.Sdtd.WebApi.Resources.SystemManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LSTY.Sdtd.WebApi.Controllers.SystemManagement
{
    /// <summary>
    /// MenusController
    /// </summary>
    [Route("SystemManagement/[controller]")]
    public class MenusController : ApiControllerBase
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IRoleMenuRepository _roleMenuRepository;
        private readonly UserInfo _userInfo;
        private readonly IRoleRepository _roleRepository;
        private readonly IStringLocalizer<MenuResource> _localizer;
        public MenusController(
            IMenuRepository sysMenuRepository,
            IRoleMenuRepository sysRoleMenuRepository,
            UserInfo userInfo,
            IRoleRepository roleRepository, 
            IStringLocalizer<MenuResource> localizer)
        {
            _menuRepository = sysMenuRepository;
            _roleMenuRepository = sysRoleMenuRepository;
            _userInfo = userInfo;
            _roleRepository = roleRepository;
            _localizer = localizer;
        }


        #region 获取菜单
        /// <summary>
        /// 获取主页菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DevelopmentResponseType(typeof(ResponseResult<IEnumerable<MenuTreeModel>>))]
        public async Task<IResponseResult> Get()
        {
            IEnumerable<T_Menu> menuList = null;

            // 系统管理员可直接查看所有菜单
            if (_userInfo.RoleName == Roles.Administrator)
            {
                menuList = await _menuRepository.QueryAllAsync();
            }
            else
            {
                IEnumerable<T_RoleMenu> roleMenuList = null;

                // 第1步：根据角色Id找到菜单Id
                roleMenuList = await _roleMenuRepository.QueryByIdAsync(nameof(T_RoleMenu.Fk_RoleId), _userInfo.RoleId);
                if (roleMenuList == null || roleMenuList.Any() == false)
                {
                    return FailedResult(_localizer["GetMenuFailed"]) ;
                }

                // 第2步：获取菜单
                menuList = _menuRepository.QueryByMenuIds(roleMenuList.Select(s => s.Fk_MenuId), true);
            }

            if (menuList == null || menuList.Any() == false)
            {
                return FailedResult(_localizer["GetMenuFailed"]);
            }

            return SucceededResult(ToTree(menuList));
        }

        #endregion

        #region 转化为 tree 数据

        /// <summary>
        /// 获取菜单 tree 数据
        /// </summary>
        /// <param name="menus">菜单集合</param>
        private static IEnumerable<MenuTreeModel> ToTree(IEnumerable<T_Menu> menus)
        {
            var treeNodes = new List<MenuTreeModel>();
            if (menus == null || menus.Any() == false)
            {
                return treeNodes;
            }

            var allMenus = menus;
            var parentMenus = allMenus.Where(t => string.IsNullOrEmpty(t.ParentId.ToString())).OrderBy(m => m.Sort);
            foreach (var item in parentMenus)
            {
                var node = ParseTreeNode(allMenus, item);
                treeNodes.Add(node);
            }

            return treeNodes;
        }

        /// <summary>
        /// 转化为 tree 节点数据
        /// </summary>
        /// <param name="menus">菜单集合</param>
        /// <param name="item">当前菜单节点</param>
        private static MenuTreeModel ParseTreeNode(IEnumerable<T_Menu> menus, T_Menu item)
        {
            var treeNode = new MenuTreeModel()
            {
                Id = item.Id.ToString(),
                CreatedDate = item.CreatedDate,
                Description = item.Description,
                IsEnabled = item.IsEnabled,
                Name = item.Name,
                Sort = item.Sort,
                ParentId = item.ParentId.ToString()
            };

            var subitems = from m in menus
                           where m.Id != item.Id && m.ParentId == item.Id
                           orderby m.Sort
                           select m;
            if (subitems != null && subitems.Any())
            {
                treeNode.Children = new List<MenuTreeModel>();
                foreach (var subitem in subitems)
                {
                    var submodule = ParseTreeNode(menus, subitem);
                    treeNode.Children.Add(submodule);
                }
            }

            return treeNode;
        }
        #endregion
    }
}
