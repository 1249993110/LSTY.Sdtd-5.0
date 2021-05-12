using UnityEngine;

namespace LSTY.Sdtd.PatronsMod.Primitives
{
    class Position
    {
        private readonly int _x;
        private readonly int _y;
        private readonly int _z;

        public int X => _x;
        public int Y => _y;
        public int Z => _z;

        public Position(int x, int y, int z)
        {
            this._x = x;
            this._y = y;
            this._z = z;
        }

        public Position(double x, double y, double z)
        {
            this._x = (int)x;
            this._y = (int)y;
            this._z = (int)z;
        }

        public Position(Vector3 v)
        {
            _x = (int)v.x;
            _y = (int)v.y;
            _z = (int)v.z;
        }

        public override string ToString()
        {
            return $"{_x} {_y} {_z}";
        }
    }
}
