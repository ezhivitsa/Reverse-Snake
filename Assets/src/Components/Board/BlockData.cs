using Unity.Entities;

namespace Assets.src.Components.Board
{
    public struct BlockData : IComponentData
    {
        public int fieldHeight;

        public int fieldWidth;
    }
}
