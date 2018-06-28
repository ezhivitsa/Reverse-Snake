using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Assets.src.Helpers;
using Assets.src.Enums;
using Assets.src.Models;
using Assets.src.InputController;
using System;


namespace Assets.src.InputController
{
    public static class InputController
    {
        internal static void GetInputArg(out DirectionEnum input)
        {
#if UNITY_EDITOR
			PCInput(out input);
#elif UNITY_IOS || UNITY_ANDROID
			MobileInput(out input);
#endif
        }

        private static void PCInput(out DirectionEnum input)
        {
            DirectionEnum direction = DirectionEnum.None;
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                input = DirectionEnum.Left;
                Debug.Log("Left");
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                input = DirectionEnum.Right;
                Debug.Log("Right");
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                input = DirectionEnum.Top;
                Debug.Log("Top");
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                input = DirectionEnum.Bottom;
                Debug.Log("Bottom");
            }
            else
            {
                input = DirectionEnum.None;
                Debug.Log("None");
            }
            
        }

        private static Vector2 fingerDown;
        private static void MobileInput(out DirectionEnum input)
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
                            input = DirectionEnum.Right;
                            return;
                        }
                        else
                        {
                            input = DirectionEnum.Left;
                            return;
                        }
                    }
                    else
                    {
                        if (delta.y > 0)
                        {
                            input = DirectionEnum.Top;
                            return;
                        }
                        else
                        {
                            input = DirectionEnum.Bottom;
                            return;
                        }
                    }
                }
            }

            input = DirectionEnum.None;
        }
    }
}