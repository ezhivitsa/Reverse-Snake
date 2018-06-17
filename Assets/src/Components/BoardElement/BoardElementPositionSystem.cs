using Unity.Entities;
using UnityEngine;

namespace Assets.src.Components.BoardElement
{
    public class BoardElementPositionSystem : ComponentSystem
    {
        private struct Group
        {
            public Transform Transform;
            public BoardElementPosition Position;
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<Group>())
            {
                entity.Transform.position = GetPositionVector(
                    entity.Position.RowPosition,
                    entity.Position.ColumnPosition
                );
            }
        }

        private Vector3 GetPositionVector(int rowPos, int columnPos)
        {
            return new Vector3(
                (AppConstants.BoardElementWidth + AppConstants.BorderWidth) * columnPos - AppConstants.OffsetX,
                0.01F,
                (AppConstants.BoardElementWidth + AppConstants.BorderWidth) * rowPos - AppConstants.OffsetZ
            );
        }
    }
}
