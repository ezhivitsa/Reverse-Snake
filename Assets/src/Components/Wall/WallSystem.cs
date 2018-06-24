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
            public MeshRenderer Renderer;
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
                entity.Transform.rotation = GetRotationQuaternion(entity.WallData);

                entity.Renderer.material = GetMaterial(entity.WallData);
            }
        }

        private Vector3 GetPositionVector(WallData wallData, float rowPos, float columnPos)
        {
            var yPos = wallData.IsActive ? 0.01F : -0.95F;
            Vector3 result;

            switch (wallData.Direction)
            {
                case DirectionEnum.Top:
                    result = new Vector3(
                        AppConstants.BoardElementWidth * (columnPos - 1/2F) + AppConstants.BorderWidth * (columnPos - 1),
                        yPos,
                        AppConstants.BoardElementWidth * rowPos + AppConstants.BorderWidth * (rowPos + 1/2F)
                    );
                    break;

                case DirectionEnum.Bottom:
                    result = new Vector3(
                        AppConstants.BoardElementWidth * (columnPos - 1/2F) + AppConstants.BorderWidth * (columnPos - 1),
                        yPos,
                        AppConstants.BoardElementWidth * (rowPos + 1) + AppConstants.BorderWidth * (rowPos + 3/2F)
                    );
                    break;

                case DirectionEnum.Left:
                    result = new Vector3(
                        AppConstants.BoardElementWidth * columnPos + AppConstants.BorderWidth * (columnPos - 1/2F),
                        yPos,
                        AppConstants.BoardElementWidth * (rowPos + 1/2F) + AppConstants.BorderWidth * (rowPos + 1)
                    );
                    break;

                case DirectionEnum.Right:
                    result = new Vector3(
                        AppConstants.BoardElementWidth * (columnPos - 1) + AppConstants.BorderWidth * (columnPos - 3/2F),
                        yPos,
                        AppConstants.BoardElementWidth * (rowPos + 1/2F) + AppConstants.BorderWidth * (rowPos + 1)
                    );
                    break;

                default:
                    return new Vector3(0, 0, 0);
            }

            return result - new Vector3(AppConstants.OffsetX, 0, AppConstants.OffsetZ);
        }

        private Quaternion GetRotationQuaternion(WallData wallData)
        {
            switch (wallData.Direction)
            {
                case DirectionEnum.Left:
                case DirectionEnum.Right:
                    return Quaternion.Euler(0, 90, 0);

                default:
                    return Quaternion.Euler(0, 0, 0);
            }
        }

        private Material GetMaterial(WallData data)
        {
            return data.IsActive ? data.WallClose : data.WallOpen;
        }
    }
}
