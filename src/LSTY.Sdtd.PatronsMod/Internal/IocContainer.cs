using Autofac;
using Autofac.Core;
using System;
using System.Linq;

namespace LSTY.Sdtd.PatronsMod.Internal
{
    /// <summary>
    /// Ioc container
    /// </summary>
    static class IocContainer
    {
        private static readonly IContainer _container;

        static IocContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();

            var assemblys = AppDomain.CurrentDomain.GetAssemblies();

            builder.RegisterAssemblyTypes(assemblys.First(x => x.FullName.StartsWith("LSTY.Sdtd.PatronsMod.Data")))
                .InNamespace("LSTY.Sdtd.PatronsMod.Data.Repositories").AsImplementedInterfaces().SingleInstance();

            _container = builder.Build();
        }

        /// <summary>
        /// 从上下文中检索服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        /// <summary>
        /// 从上下文中检索服务
        /// </summary>
        /// <returns></returns>
        public static object Resolve(Type type)
        {
            return _container.Resolve(type);
        }

        /// <summary>
        /// 从上下文中检索服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static T Resolve<T>(params Parameter[] parameters)
        {
            return _container.Resolve<T>(parameters);
        }

        /// <summary>
        /// 从上下文中检索服务
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object Resolve(Type type, params Parameter[] parameters)
        {
            return _container.Resolve(type, parameters);
        }
    }
}
