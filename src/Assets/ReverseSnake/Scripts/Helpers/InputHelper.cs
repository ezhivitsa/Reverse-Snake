using Assets.ReverseSnake.Scripts.Enums;
using System;
using UnityEngine;

namespace Assets.ReverseSnake.Scripts.Helpers
{
    class InputHelper
    {
        public bool detectSwipeOnlyAfterRelease = true;
        public float SWIPE_THRESHOLD = 100f;
        public float TOP_BOTTOM_GAP = 100f;

        private static Vector2 fingerDown;
        private static Vector2? fingerUp;

        private ReverseSnakeWorld _world;

        public InputHelper(ReverseSnakeWorld world)
        {
            _world = world;
        }

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
                if (touch.position.y < TOP_BOTTOM_GAP || touch.position.y > Screen.height - TOP_BOTTOM_GAP)
                {
                    fingerUp = null;
                    return;
                }

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

                        if (fingerUp.HasValue)
                        {
                            CheckSwipe();
                        }
                    }
                }

                //Detects swipe after finger is released
                if (touch.phase == TouchPhase.Ended)
                {
                    fingerDown = touch.position;
                    if (fingerUp.HasValue)
                    {
                        CheckSwipe();
                    }
                }
            }
        }

        private void CheckSwipe()
        {
            //Check if Vertical swipe
            if (VerticalMove() > SWIPE_THRESHOLD && VerticalMove() > HorizontalValMove())
            {
                if (fingerDown.y - fingerUp.Value.y > 0)//up swipe
                {
                    OnSwipeUp();
                }
                else if (fingerDown.y - fingerUp.Value.y < 0)//Down swipe
                {
                    OnSwipeDown();
                }
                fingerUp = fingerDown;
            }

            //Check if Horizontal swipe
            else if (HorizontalValMove() > SWIPE_THRESHOLD && HorizontalValMove() > VerticalMove())
            {
                if (fingerDown.x - fingerUp.Value.x > 0)//Right swipe
                {
                    OnSwipeRight();
                }
                else if (fingerDown.x - fingerUp.Value.x < 0)//Left swipe
                {
                    OnSwipeLeft();
                }
                fingerUp = fingerDown;
            }
        }

        private float VerticalMove()
        {
            return Mathf.Abs(fingerDown.y - fingerUp.Value.y);
        }

        private float HorizontalValMove()
        {
            return Mathf.Abs(fingerDown.x - fingerUp.Value.x);
        }

        private void OnSwipeUp()
        {
            OnSwipe(DirectionEnum.Top);
        }

        private void OnSwipeDown()
        {
            OnSwipe(DirectionEnum.Bottom);
        }

        private void OnSwipeLeft()
        {
            OnSwipe(DirectionEnum.Left);
        }

        private void OnSwipeRight()
        {
            OnSwipe(DirectionEnum.Right);
        }

        protected virtual void OnSwipe(DirectionEnum direction)
        {
            var entity = _world.CreateEntityWith<SwipeDoneEvent>(out SwipeDoneEvent eventData);
            eventData.Direction = direction;
        }
    }
}
