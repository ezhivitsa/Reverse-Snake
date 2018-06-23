using Assets.src;
using Unity.Entities;
using UnityEngine;

public class SnakeStepSystem : ComponentSystem
{
    private struct Group
    {
        public Transform Transform;
        public SnakeStepPosition SnakePosition;
        public SnakeStepData Data;
    }

    protected override void OnUpdate()
    {
        foreach (var entity in GetEntities<Group>())
        {
            entity.Transform.position = GetPositionVector(
                entity.SnakePosition.RowPosition,
                entity.SnakePosition.ColumnPosition
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
