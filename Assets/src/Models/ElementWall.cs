using Assets.src.Enums;

namespace Assets.src.Models
{
    public class ElementWall
    {
        public Element Element { get; set; }

        public DirectionEnum Side { get; set; }

        public bool IsClosed { get; set; }
    }
}
