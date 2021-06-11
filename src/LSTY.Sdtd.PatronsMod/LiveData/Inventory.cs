using LSTY.Sdtd.PatronsMod.Extensions;
using System.Collections.Generic;

namespace LSTY.Sdtd.PatronsMod.LiveData
{
    public class Inventory
    {
        public List<InvItem> Bag { get => _bag; set => _bag = value; }
        public List<InvItem> Belt { get => _belt; set => _belt = value; }
        public InvItem[] Equipment { get => _equipment; set => _equipment = value; }

        private List<InvItem> _bag;
        private List<InvItem> _belt;
        private InvItem[] _equipment;

        public static Inventory Create()
        {
            return new Inventory()
            {
                _bag = new List<InvItem>(),
                _belt = new List<InvItem>()
            };
        }

        public void Update(PlayerDataFile pdf)
        {
            ProcessInv(_bag, pdf.bag, pdf.id);
            ProcessInv(_belt, pdf.inventory, pdf.id);
            ProcessEqu(pdf.equipment, pdf.id);
        }

        private void ProcessInv(List<InvItem> target, ItemStack[] sourceFields, int entityId)
        {
            target.Clear();

            foreach (var field in sourceFields)
            {
                InvItem invItem = CreateInvItem(field.itemValue, field.count, entityId);
                if (invItem != null && field.itemValue.Modifications != null)
                {
                    ProcessParts(field.itemValue.Modifications, invItem, entityId);
                }

                target.Add(invItem);
            }
        }

        private void ProcessEqu(Equipment sourceEquipment, int entityId)
        {
            int slotCount = sourceEquipment.GetSlotCount();
            _equipment = new InvItem[slotCount];
            for (int i = 0; i < slotCount; ++i)
            {
                _equipment[i] = CreateInvItem(sourceEquipment.GetSlotItem(i), 1, entityId);
            }
        }

        private void ProcessParts(ItemValue[] parts, InvItem item, int entityId)
        {
            int length = parts.Length;

            InvItem[] itemParts = new InvItem[length];

            for (int i = 0; i < length; ++i)
            {
                InvItem partItem = CreateInvItem(parts[i], 1, entityId);
                if (partItem != null && parts[i].Modifications != null)
                {
                    ProcessParts(parts[i].Modifications, partItem, entityId);
                }

                itemParts[i] = partItem;
            }

            item.Parts = itemParts;
        }

        private InvItem CreateInvItem(ItemValue itemValue, int count, int entityId)
        {
            if (count <= 0 || itemValue == null || itemValue.Equals(ItemValue.None))
            {
                return null;
            }

            ItemClass itemClass = ItemClass.list[itemValue.type];
            int maxAllowed = itemClass.Stacknumber.Value;
            string name = itemClass.GetItemName();

            //string steamId = ConnectionManager.Instance.Clients.ForEntityId(entityId).playerId;

            //var inventoryCheck = FunctionManager.AntiCheat.InventoryCheck;
            //if (inventoryCheck.IsEnabled)
            //{
            //    inventoryCheck.Execute(steamId, name, count, maxAllowed);
            //}

            int quality = itemValue.HasQuality ? itemValue.Quality : -1;

            InvItem item = new InvItem()
            {
                ItemName = name,
                Count = count,
                Quality = quality,
                Icon = itemClass.GetIconName(),
                Iconcolor = itemClass.GetIconTint().ToHex(),
                MaxUseTimes = itemValue.MaxUseTimes,
                UseTimes = itemValue.UseTimes
            };

            return item;
        }
    }
}
