using Assets.ReverseSnake.Scripts.Enums;
using System;
using UnityEngine;

namespace Assets.ReverseSnake.Scripts.Helpers
{
    public class SwipeEventArgs : EventArgs
    {
        public DirectionEnum direction { get; set; }
    }

    public class InputHelper
    {
        public event EventHandler<SwipeEventArgs> Swipe;

        public bool detectSwipeOnlyAfterRelease = true;
        public float SWIPE_THRESHOLD = 100f;

        private static Vector2 fingerDown;
        private static Vector2 fingerUp;

        public void Update()
        {
        #if UNITY_EDITOR
            PCInput();
        #elif UNITY_IOS || UNITY_ANDROID
			MobileInput();
        #endif
        }

        public void Clear()
        {
            fingerUp = new Vector2();
            fingerDown = new Vector2();
        }

        private void PCInput()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                OnSwipeLeft();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                OnSwipeRight();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                OnSwipeUp();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                OnSwipeDown();
            }
        }

        private void MobileInput()
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    fingerUp = touch.position;
                    fingerDown = touch.position;
                }

                //Detects Swipe while finger is still moving
                if (touch.phase == TouchPhase.Moved)
                {
                    if (!detectSwipeOnlyAfterRelease)
                    {
                        fingerDown = touch.position;
                        CheckSwipe();
                    }
                }

                //Detects swipe after finger is released
                if (touch.phase == TouchPhase.Ended)
                {
                    fingerDown = touch.position;
                    CheckSwipe();
                }
            }
        }

        private void CheckSwipe()
        {
            //Check if Vertical swipe
            if (VerticalMove() > SWIPE_THRESHOLD && VerticalMove() > HorizontalValMove())
            {
                if (fingerDown.y - fingerUp.y > 0)//up swipe
                {
                    OnSwipeUp();
                }
                else if (fingerDown.y - fingerUp.y < 0)//Down swipe
                {
                    OnSwipeDown();
                }
                fingerUp = fingerDown;
            }

            //Check if Horizontal swipe
            else if (HorizontalValMove() > SWIPE_THRESHOLD && HorizontalValMove() > VerticalMove())
            {
                if (fingerDown.x - fingerUp.x > 0)//Right swipe
                {
                    OnSwipeRight();
                }
                else if (fingerDown.x - fingerUp.x < 0)//Left swipe
                {
                    OnSwipeLeft();
                }
                fingerUp = fingerDown;
            }
        }

        private float VerticalMove()
        {
            return Mathf.Abs(fingerDown.y - fingerUp.y);
        }

        private float HorizontalValMove()
        {
            return Mathf.Abs(fingerDown.x - fingerUp.x);
        }

        private void OnSwipeUp()
        {
            SwipeEventArgs args = new SwipeEventArgs
            {
                direction = DirectionEnum.Top
            };
            OnSwipe(args);
        }

        private void OnSwipeDown()
        {
            SwipeEventArgs args = new SwipeEventArgs
            {
                direction = DirectionEnum.Bottom
            };
            OnSwipe(args);
        }

        private void OnSwipeLeft()
        {
            SwipeEventArgs args = new SwipeEventArgs
            {
                direction = DirectionEnum.Left
            };
            OnSwipe(args);
        }

        private void OnSwipeRight()
        {
            SwipeEventArgs args = new SwipeEventArgs
            {
                direction = DirectionEnum.Right
            };
            OnSwipe(args);
        }

        protected virtual void OnSwipe(SwipeEventArgs e)
        {
            Swipe?.Invoke(this, e);
        }
    }
}
