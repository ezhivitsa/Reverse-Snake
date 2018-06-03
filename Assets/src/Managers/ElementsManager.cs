using Assets.src.Enums;
using Assets.src.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.src.Managers
{
    public class ElementsManager
    {
        private List<Element> _elements;
        private List<Wall> _walls;

        public ElementsManager()
        {
            _elements = new List<Element>();
        }

        public void Add(Element element, PositionsManager positionsManager)
        {
            _elements.Add(element);

            var allDirections = new List<DirectionEnum>
            {
                DirectionEnum.Top,
                DirectionEnum.Right,
                DirectionEnum.Bottom,
                DirectionEnum.Left,
            };
            allDirections.ForEach((direction) =>
            {
                var neighborPos = positionsManager.GetPosition(direction, element.BoartPosition);
                var neighbor = GetElementAtPosition(neighborPos);
                if (neighbor != null)
                {
                    element.Neighbors.Add(new NeighborElement
                    {
                        Direction = direction,
                        Element = neighbor,
                    });
                    neighbor.Neighbors.Add(new NeighborElement
                    {
                        Direction = positionsManager.GetReverseDirection(direction),
                        Element = element,
                    });
                }
            });
        }

        public void AddWall(int from, int to)
        {
            var fromElement = _elements.Find(e => e.BoartPosition == from);
            var toElement = _elements.Find(e => e.BoartPosition == from);

            _walls.Add(new Wall
            {
                From = fromElement,
                To = toElement,
            });
        }

        public List<Element> GetFree()
        {
            return _elements.Where(e => e.IsFree).ToList();
        }

        public Element GetFreeRandomElement()
        {
            var freeElements = GetFree();
            var num = Random.Range(0, freeElements.Count);
            return freeElements[num];
        }

        public Element GetElementAtPosition(int position)
        {
            return _elements.Find(e => e.BoartPosition == position);
        }

        public ElementWall GetRandomOpenWall()
        {
            var elements = _elements.Where(e => e.CanAddWall).ToList();
            var num = Random.Range(0, elements.Count);

            var walls = elements[num].OpenWalls;
            var wallNum = Random.Range(0, walls.Count);
            return walls[wallNum];
        }
    }
}
