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

        private List<ElementWall> _walls { get; set; }

        public List<NeighborElement> Neighbors { get; set; }

        public bool IsFree
        {
            get
            {
                return !ContainsStep && ChildTarget == null;
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
                CreateWall(DirectionEnum.Top),
                CreateWall(DirectionEnum.Right),
                CreateWall(DirectionEnum.Bottom),
                CreateWall(DirectionEnum.Left),
            };
            Neighbors = new List<NeighborElement>();
        }

        public Element GetNeighbor(DirectionEnum direction)
        {
            var neighbor = Neighbors.Find(n => n.Direction == direction);
            return neighbor != null ? neighbor.Element : null;
        }

        public NeighborElement GetNeighbor(Element element)
        {
            return Neighbors.Find(n => n.Element == element);
        }

        public void CloseWall(DirectionEnum direction)
        {
            var wall = _walls.Find(w => w.Side == direction);
            wall.IsClosed = true;
        }

        public bool CanGoTo(Element element)
        {
            var neighbor = GetNeighbor(element);
            var wall = _walls.Find(w => w.Side == neighbor.Direction);
            return !wall.IsClosed;
        }

        private ElementWall CreateWall(DirectionEnum side)
        {
            return new ElementWall
            {
                Element = this,
                IsClosed = false,
                Side = side,
            };
        }

        public List<ElementWall> GetAvailableWalls()
        {
            return _walls.Where((w) =>
            {
                var neighbor = Neighbors.Find(n => n.Direction == w.Side);
                return neighbor.Element.OpenWalls.Count > 2;
            }).ToList();
        }
    }
}
