using LSTY.Sdtd.PatronsMod.ExceptionCatch;
using LSTY.Sdtd.PatronsMod.Primitives;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LSTY.Sdtd.PatronsMod
{
    static class FunctionManager
    {
        public static List<IFunction> Functions => _functions;

        private static List<IFunction> _functions;

#pragma warning disable 0649

        public static Functions.CommonConfig CommonConfig;
        public static Functions.GameNotice GameNotice;
        public static Functions.PointsSystem PointsSystem;
        public static Functions.TeleportCity TeleportCity;
        public static Functions.TeleportHome TeleportHome;
        public static Functions.TeleportFriend TeleportFriend;
        public static Functions.GameStore GameStore;
        public static Functions.LotterySystem LotterySystem;
        public static Functions.ExtensionFunctions ExtensionFunctions;

#pragma warning restore 0649
        [CatchException("Error in FunctionManager.Init")]
        public static void Init()
        {
            _functions = new List<IFunction>();

            Type fieldType;
            object function;
            foreach (var item in typeof(FunctionManager).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                fieldType = item.FieldType;

                if (typeof(IFunction).IsAssignableFrom(fieldType))
                {
                    function = Activator.CreateInstance(fieldType);
                    item.SetValue(null, function);

                    _functions.Add(function as IFunction);
                }
            }
        }
    }
}
