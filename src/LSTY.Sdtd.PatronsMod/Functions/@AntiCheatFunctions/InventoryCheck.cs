//using LSTY.Sdtd.PatronsMod.Primitives;
//using System;

//namespace LSTY.Sdtd.PatronsMod.AntiCheat
//{
//    class InventoryCheck : SubFunctionBase
//    {
//        /// <summary>
//        /// Invalid items found, params are:
//        /// steamId itemName ownedCount maxAllowed
//        /// </summary>
//        public event Action<string, string, int, int> InvalidItemsFound;

//        public InventoryCheck(IFunction parent) : base(parent)
//        {
//        }

//        public void Execute(string steamId, string itemName, int ownedCount, int maxAllowed)
//        {
//            if (ownedCount > maxAllowed)
//            {
//                InvalidItemsFound.Invoke(steamId, itemName, ownedCount, maxAllowed);

//                //Log.Out("Player with ID " + playerId + " has stack for \"" + name + "\" greater than allowed (" +
//                //         count + " > " + maxAllowed + ")");
//            }
//        }
//    }
//}
