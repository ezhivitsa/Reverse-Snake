using Assets.src;
using Unity.Entities;
using UnityEngine;

public class TargetSystem : ComponentSystem
{
    private struct Group
    {
        public Transform Transform;
        public TargetPosition Position;
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
            (AppConstants.BoardElementWidth + AppConstants.BorderWidth) * (columnPos - 1) - AppConstants.OffsetX,
            1F,
            (AppConstants.BoardElementWidth + AppConstants.BorderWidth) * rowPos - AppConstants.OffsetZ + AppConstants.BorderWidth
        );
    }
}
