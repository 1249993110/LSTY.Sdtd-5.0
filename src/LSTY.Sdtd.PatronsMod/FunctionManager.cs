using LSTY.Sdtd.PatronsMod.Primitives;
using System;
using System.Collections.Generic;

namespace LSTY.Sdtd.PatronsMod
{
    static class FunctionManager
    {
        public static List<IFunction> Functions;

#pragma warning disable 0649

        public static Functions.CommonConfig CommonConfig;
        public static Functions.GameNotice GameNotice;
        public static Functions.AntiCheat AntiCheat;
        public static Functions.PointsSystem PointsSystem;
        public static Functions.TeleportCity TeleportCity;

#pragma warning restore 0649

        public static void Init()
        {
            Functions = new List<IFunction>();

            Type fieldType;
            IFunction function;
            foreach (var item in typeof(FunctionManager).GetFields())
            {
                fieldType = item.FieldType;
                if (typeof(IFunction).IsAssignableFrom(fieldType))
                {
                    function = Activator.CreateInstance(fieldType) as IFunction;
                    item.SetValue(null, function);

                    Functions.Add(function);
                }
            }
        }
    }
}
