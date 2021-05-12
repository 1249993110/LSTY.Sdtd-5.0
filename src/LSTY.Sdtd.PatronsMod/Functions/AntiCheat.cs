using IceCoffee.Common.Xml;
using LSTY.Sdtd.PatronsMod.AntiCheat;
using LSTY.Sdtd.PatronsMod.Primitives;
using System.Xml;

namespace LSTY.Sdtd.PatronsMod.Functions
{
    class AntiCheat : FunctionBase
    {
        /// <summary>
        /// Inventory check
        /// </summary>
        [ConfigNode(XmlNodeType.Element)]
        public InventoryCheck InventoryCheck { get; set; }

        public AntiCheat()
        {
            InventoryCheck = new InventoryCheck(this);
        }
    }
}
