using HarmonyLib;
using LSTY.Sdtd.PatronsMod.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.HarmonyPatchers
{
    [HarmonyPatch(typeof(World), nameof(World.SpawnEntityInWorld))]
    public class World_SpawnEntityInWorld_Patcher
    {
        [HarmonyPostfix]
        public static void Postfix(Entity _entity)
        {
            CustomModEvents.RaiseEntitySpawnedEvent(_entity);
        }
    }
}
