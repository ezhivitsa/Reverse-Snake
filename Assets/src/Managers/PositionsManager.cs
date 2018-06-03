using Assets.src.Enums;
using UnityEngine;

namespace Assets.src.Managers
{
    public class PositionsManager
    {
        private int _width;
        private int _height;

        public PositionsManager(int height, int width)
        {
            _height = height;
            _width = width;
        }

        public Vector3 GetPositionVector(int heightPos, int widthPos)
        {
            return new Vector3(
                (AppConstants.BoardElementWidth + AppConstants.BorderWidth) * widthPos - AppConstants.OffsetX,
                0.01F,
                (AppConstants.BoardElementWidth + AppConstants.BorderWidth) * heightPos - AppConstants.OffsetZ
            );
        }

        public int GetPositionNumber(int heightPos, int widthPos)
        {
            return heightPos * _height + widthPos;
        }

        public int GetPosition(DirectionEnum direction, int relativePosition)
        {
            var position = -1;
            switch(direction)
            {
                case DirectionEnum.Left:
                    position = relativePosition - _height;
                    if (position < 0)
                    {
                        position += _width * _height;
                    }
                    break;

                case DirectionEnum.Right:
                    position = relativePosition + _height;
                    if (position >= _width * _height)
                    {
                        position %= _width;
                    }
                    break;

                case DirectionEnum.Top:
                    if (relativePosition % _height == 0)
                    {
                        position = relativePosition + _height;
                    }
                    else
                    {
                        position = relativePosition - 1;
                    }
                    break;

                case DirectionEnum.Bottom:
                    if ((relativePosition + 1) % _height == 0)
                    {
                        position = relativePosition + 1 - _height;
                    }
                    else
                    {
                        position = relativePosition + 1;
                    }
                    break;
            }

            return position;
        }

        public DirectionEnum GetReverseDirection(DirectionEnum direction)
        {
            switch (direction)
            {
                case DirectionEnum.Left:
                    return DirectionEnum.Right;

                case DirectionEnum.Right:
                    return DirectionEnum.Left;

                case DirectionEnum.Top:
                    return DirectionEnum.Bottom;

                case DirectionEnum.Bottom:
                    return DirectionEnum.Top;

                default:
                    return DirectionEnum.Top;
            }
        }
    }
}
