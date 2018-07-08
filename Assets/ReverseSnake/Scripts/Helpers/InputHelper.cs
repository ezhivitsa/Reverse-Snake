using Assets.ReverseSnake.Scripts.Enums;
using UnityEngine;

namespace Assets.ReverseSnake.Scripts.Helpers
{
    public static class InputHelper
    {
        internal static DirectionEnum GetInputArg()
        {
        #if UNITY_EDITOR
            return PCInput();
        #elif UNITY_IOS || UNITY_ANDROID
			return MobileInput();
        #endif
        }

        private static DirectionEnum PCInput()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                return DirectionEnum.Left;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                return DirectionEnum.Right;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                return DirectionEnum.Top;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                return DirectionEnum.Bottom;
            }
            else
            {
                return DirectionEnum.None;
            }
        }

        private static Vector2 fingerDown;
        private static DirectionEnum MobileInput()
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    fingerDown = touch.position;
                }
                if (touch.phase == TouchPhase.Ended)
                {
                    Vector2 fingerUp = touch.position;

                    Vector2 delta = fingerUp - fingerDown;

                    float xAbs = Mathf.Abs(delta.x);
                    float yAbs = Mathf.Abs(delta.y);

                    if (xAbs > yAbs)
                    {
                        if (delta.x > 0)
                        {
                            return DirectionEnum.Right;
                        }
                        else
                        {
                            return DirectionEnum.Left;
                        }
                    }
                    else
                    {
                        if (delta.y > 0)
                        {
                            return DirectionEnum.Top;
                        }
                        else
                        {
                            return DirectionEnum.Bottom;
                        }
                    }
                }
            }

            return DirectionEnum.None;
        }
    }
}
