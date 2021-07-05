using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.Internal
{
    static class CustomModEvents
    {
        public static event Action<Entity> EntitySpawned;

        public static void RaiseEntitySpawnedEvent(Entity entity)
        {
            EntitySpawned?.Invoke(entity);
        }
    }
}
