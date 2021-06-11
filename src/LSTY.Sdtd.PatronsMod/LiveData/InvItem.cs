namespace LSTY.Sdtd.PatronsMod.LiveData
{
    public class InvItem
    {
        private string _itemName;
        private int _count;
        private int _quality;
        private string _icon;
        private string _iconcolor;
        private int _maxUseTimes;
        private float _useTimes;
        private InvItem[] _parts;

        public string ItemName { get => _itemName; set => _itemName = value; }
        public int Count { get => _count; set => _count = value; }
        public int Quality { get => _quality; set => _quality = value; }
        public string Icon { get => _icon; set => _icon = value; }
        public string Iconcolor { get => _iconcolor; set => _iconcolor = value; }
        public int MaxUseTimes { get => _maxUseTimes; set => _maxUseTimes = value; }
        public float UseTimes { get => _useTimes; set => _useTimes = value; }
        public InvItem[] Parts { get => _parts; set => _parts = value; }

        public InvItem()
        {
        }

        public InvItem(string itemName, int count, int quality, string icon,
            string iconcolor, int maxUseTimes, float useTimes, InvItem[] parts = null)
        {
            this._itemName = itemName;
            this._count = count;
            this._quality = quality;
            this._icon = icon;
            this._iconcolor = iconcolor;
            this._maxUseTimes = maxUseTimes;
            this._useTimes = useTimes;
            this._parts = parts;
        }
    }
}
