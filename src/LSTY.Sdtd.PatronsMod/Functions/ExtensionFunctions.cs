using IceCoffee.Common.Xml;
using LSTY.Sdtd.PatronsMod.Internal;
using LSTY.Sdtd.PatronsMod.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LSTY.Sdtd.PatronsMod.Functions
{
    public class ExtensionFunctions : IFunction
    {
        [ConfigNode(XmlNodeType.Element)]
        public DeathPenalty DeathPenalty { get; set; }

        [ConfigNode(XmlNodeType.Element)]
        public OnlineReward OnlineReward { get; set; }

        [ConfigNode(XmlNodeType.Element)]
        public ZombieKillReward ZombieKillReward { get; set; }

        public string FunctionName => nameof(ExtensionFunctions);

        public ExtensionFunctions()
        {
            Type type = null;
            foreach (var item in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                type = item.PropertyType;
                if (type.IsSubclassOf(typeof(FunctionBase)))
                {
                    FunctionBase function = Activator.CreateInstance(type) as FunctionBase;
                    item.SetValue(this, function);

                    FunctionManager.Functions.Add(function);
                }
            }
        }
    }
}
