using Assets.src.Enums;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Assets.src.Models
{
    public class Element
    {
        public Vector3 Position { get; set; }

        public int BoartPosition { get; set; }

        public bool ContainsStep { get; set; }

        public TargetElement ChildTarget { get; set; }

        public List<ElementWall> _walls { get; set; }

        public bool IsFree
        {
            get
            {
                return !ContainsStep && ChildTarget == null;
            }
        }

        public bool CanAddWall
        {
            get
            {
                return OpenWalls.Count > 2;
            }
        }

        public List<ElementWall> OpenWalls
        {
            get
            {
                return _walls.Where(w => !w.IsClosed).ToList();
            }
        }

        public Element()
        {
            _walls = new List<ElementWall>
            {
                GetWall(WallSide.Top),
                GetWall(WallSide.Right),
                GetWall(WallSide.Bottom),
                GetWall(WallSide.Left),
            };
        }

        private ElementWall GetWall(WallSide side)
        {
            return new ElementWall
            {
                Element = this,
                IsClosed = false,
                Side = side,
            };
        }
    }
}
