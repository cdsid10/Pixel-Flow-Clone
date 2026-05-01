using UnityEngine;
using Scripts.Enums;

namespace Scripts.Grid
{
    public class GridCube
    {
        public ColorTypeEnum colorType;
        public bool isDestroyed;

        public GridCube(ColorTypeEnum colorType)
        {
            this.colorType = colorType;
            this.isDestroyed = false;
        }
    }
}