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
                    entity.Position
                );
                entity.Transform.rotation = GetRotationQuaternion(entity.Position);

                entity.Renderer.material = GetMaterial(entity.WallData);
            }
        }

        private Vector3 GetPositionVector(WallData wallData, WallPosition position)
        {
            var yPos = wallData.IsActive ? 0.01F : -0.95F;
            Vector3 result;

            switch (position.Direction)
            {
                case DirectionEnum.Top:
                    result = CalculatePosition(position.ColumnPosition, position.RowPosition, -1/2F, -1, yPos, 0, 1/2F);
                    break;

                case DirectionEnum.Bottom:
                    result = CalculatePosition(position.ColumnPosition, position.RowPosition, -1/2F, -1, yPos, 1, 3/2F);
                    break;

                case DirectionEnum.Left:
                    result = CalculatePosition(position.ColumnPosition, position.RowPosition, 0, -1/2F, yPos, 1/2F, 1);
                    break;

                case DirectionEnum.Right:
                    result = CalculatePosition(position.ColumnPosition, position.RowPosition, -1, -3/2F, yPos, 1/2F, 1);
                    break;

                default:
                    return new Vector3(0, 0, 0);
            }

            return result - new Vector3(AppConstants.OffsetX, 0, AppConstants.OffsetZ);
        }

        private Vector3 CalculatePosition(
            float columnPos,
            float rowPos,
            float elementXCoeff,
            float borderXCoeff,
            float yPos,
            float elementZCoeff,
            float borderZCoeff
        )
        {
            return new Vector3(
                AppConstants.BoardElementWidth * (columnPos + elementXCoeff) + AppConstants.BorderWidth * (columnPos + borderXCoeff),
                yPos,
                AppConstants.BoardElementWidth * (rowPos + elementZCoeff) + AppConstants.BorderWidth * (rowPos + borderZCoeff)
            );
        }

        private Quaternion GetRotationQuaternion(WallPosition wallPosition)
        {
            switch (wallPosition.Direction)
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
