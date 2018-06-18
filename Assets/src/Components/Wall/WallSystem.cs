using Assets.src.Enums;
using Unity.Entities;
using UnityEngine;

namespace Assets.src.Components.Wall
{
    public class WallSystem : ComponentSystem
    {
        private struct Group
        {
            public Transform Transform;
            public WallData WallData;
            public WallPosition Position;
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<Group>())
            {
                entity.Transform.position = GetPositionVector(
                    entity.WallData,
                    entity.Position.RowPosition,
                    entity.Position.ColumnPosition
                );
                Debug.Log(GetPositionVector(
                    entity.WallData,
                    entity.Position.RowPosition,
                    entity.Position.ColumnPosition
                ));
            }
        }

        private Vector3 GetPositionVector(WallData wallData, int rowPos, int columnPos)
        {
            switch (wallData.Directon)
            {
                case DirectionEnum.Top:
                    return new Vector3(
                        (AppConstants.BoardElementWidth + AppConstants.BorderWidth) * columnPos - AppConstants.OffsetX - AppConstants.BoardElementWidth / 2 - AppConstants.BorderWidth,
                        0.01F,
                        (AppConstants.BoardElementWidth + AppConstants.BorderWidth) * rowPos - AppConstants.OffsetZ + AppConstants.BorderWidth / 2
                    );

                default:
                    return new Vector3(0, 0, 0);
            }
        }
    }
}
