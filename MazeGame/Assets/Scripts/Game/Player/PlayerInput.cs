using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace Game
{
    public class PlayerInput : MonoBehaviour
    {
        [HideInInspector]
        public List<bool> PlayerMovementInputs = new List<bool>
        {
            false,    //right
            false,    //left
            false,    //forward
            false     //back
        };
        
        [HideInInspector]
        public List<bool> CameraOrientationInput = new List<bool>
        {
            false,    //right
            false     //left
        };
        
        private Vector3 _firstTouchPos;
        private Vector3 _lastTouchPos;

        public float ScreenPartitionPercentage;
        
        [Header("Player Movement Input")]
        public float TouchOffDragScreenPercent;
        public float TouchOnDragScreenPercent;
        private float TouchOffDragDistance;
        private float TouchOnDragDistance;

        [Header("Camera Orientation Input")] 
        public float TouchDragScreenPercentCameraOrientation;
        private float TouchDragDistanceCameraOrientation;

        private bool upperPartitionInteraction;

        public void Start()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                TouchOnDragDistance = Screen.height * TouchOnDragScreenPercent/100;
                TouchOffDragDistance = Screen.height * TouchOffDragDistance/100;
                TouchDragDistanceCameraOrientation = Screen.height * TouchDragScreenPercentCameraOrientation/100;
            }
            else
            {
                TouchOnDragDistance = 10;
                TouchOffDragDistance = 10;
                TouchDragDistanceCameraOrientation = 10;
            }
        }

        public void Update()
        {
            ResetInputs();
            
            HandleKeyboardInputs();
            HandleTouchInputs();
        }

        /*
         * returns the change in the camera orientation
         */
        public Direction GetCameraOrientationDirection()
        {
            if (CameraOrientationInput[(int) Direction.Right])
            {
                return Direction.Right;
            }
            if (CameraOrientationInput[(int) Direction.Left])
            {
                return Direction.Left;
            }
            return Direction.None;
        }

        /*
         * returns the movement direction of the swipe input
         */
        public Direction GetInputPlayerMovementDirection()
        {
            if (PlayerMovementInputs[(int) Direction.Right])
            {
                return Direction.Right;
            }
            if (PlayerMovementInputs[(int) Direction.Left])
            {
                return Direction.Left;
            }
            if (PlayerMovementInputs[(int) Direction.Forward])
            {
                return Direction.Forward;
            }
            if (PlayerMovementInputs[(int) Direction.Back])
            {
                return Direction.Back;
            }
            return Direction.None;
        }

        private void HandleKeyboardInputs()
        {
            if (Input.GetKey(KeyCode.D))
            {
                PlayerMovementInputs[(int)Direction.Right] = true;
            }
            if (Input.GetKey(KeyCode.A))
            {
                PlayerMovementInputs[(int)Direction.Left] = true;
            }
            if (Input.GetKey(KeyCode.W))
            {
                PlayerMovementInputs[(int)Direction.Forward] = true;
            }
            if (Input.GetKey(KeyCode.S))
            {
                PlayerMovementInputs[(int)Direction.Back] = true;
            }
            
            if(Input.GetKeyDown(KeyCode.E))
            {
                CameraOrientationInput[(int) Direction.Right] = true;
            }
            if(Input.GetKeyDown(KeyCode.Q))
            {
                CameraOrientationInput[(int) Direction.Left] = true;
            }
        }
        
        private void HandleTouchInputs()
        {
            if (Input.touchCount == 1) // user is touching the screen with a single touch
            {
                var touch = Input.GetTouch(0); // get the touch
                
                //checking in which part of the screen the touch lies
                if (touch.phase == TouchPhase.Began)
                {
                    upperPartitionInteraction = touch.position.y >= Screen.height * ScreenPartitionPercentage / 100;
                    _firstTouchPos = touch.position;
                    _lastTouchPos = touch.position;
                }

                if (upperPartitionInteraction)
                {
                    Debug.Log("in upper part of the screen");
                    switch (touch.phase)
                    {
                        /*
                         *the _lastTuchPos is updated
                         *it also handles the onScreenTouch drag input
                         */
                        case TouchPhase.Moved:
                            _lastTouchPos = touch.position;
                            //Check if drag distance is greater than touchOnDragDistance
                            if (Mathf.Abs(_lastTouchPos.x - _firstTouchPos.x) > TouchOnDragDistance ||
                                Mathf.Abs(_lastTouchPos.y - _firstTouchPos.y) > TouchOnDragDistance) //its a drag
                            {
                                /* check if the drag is vertical or horizontal
                                 *
                                */
                                if (_lastTouchPos.x - _firstTouchPos.x > 0 && _lastTouchPos.y - _firstTouchPos.y > 0)
                                {
                                    PlayerMovementInputs[(int) Direction.Forward] = true;
                                }

                                if (_lastTouchPos.x - _firstTouchPos.x < 0 && _lastTouchPos.y - _firstTouchPos.y < 0)
                                {
                                    PlayerMovementInputs[(int) Direction.Back] = true;
                                }

                                if (_lastTouchPos.x - _firstTouchPos.x < 0 && _lastTouchPos.y - _firstTouchPos.y > 0)
                                {
                                    PlayerMovementInputs[(int) Direction.Left] = true;
                                }

                                if (_lastTouchPos.x - _firstTouchPos.x > 0 && _lastTouchPos.y - _firstTouchPos.y < 0)
                                {
                                    PlayerMovementInputs[(int) Direction.Right] = true;
                                }

                                _firstTouchPos = _lastTouchPos;
                            }

                            break;
                        //check if the finger is removed from the screen
                        case TouchPhase.Ended:
                            _lastTouchPos = touch.position;

                            //Check if drag distance is greater than touchOffDragDistance
                            if (Mathf.Abs(_lastTouchPos.x - _firstTouchPos.x) > TouchOffDragDistance ||
                                Mathf.Abs(_lastTouchPos.y - _firstTouchPos.y) > TouchOffDragDistance) //its a drag
                            {
                                /* check if the drag is vertical or horizontal
                                 *
                                 */
                                if (_lastTouchPos.x - _firstTouchPos.x > 0 && _lastTouchPos.y - _firstTouchPos.y > 0)
                                {
                                    PlayerMovementInputs[(int) Direction.Forward] = true;
                                }

                                if (_lastTouchPos.x - _firstTouchPos.x < 0 && _lastTouchPos.y - _firstTouchPos.y < 0)
                                {
                                    PlayerMovementInputs[(int) Direction.Back] = true;
                                }

                                if (_lastTouchPos.x - _firstTouchPos.x < 0 && _lastTouchPos.y - _firstTouchPos.y > 0)
                                {
                                    PlayerMovementInputs[(int) Direction.Left] = true;
                                }

                                if (_lastTouchPos.x - _firstTouchPos.x > 0 && _lastTouchPos.y - _firstTouchPos.y < 0)
                                {
                                    PlayerMovementInputs[(int) Direction.Right] = true;
                                }
                            }

                            break;
                    }
                }
                else
                {
                    Debug.Log("in lower part of the screen");
                    switch (touch.phase)
                    {
                        //check if the finger is removed from the screen
                        case TouchPhase.Ended:
                            _lastTouchPos = touch.position; 

                            //Check if drag distance is greater than touchOffDragDistance
                            if (Mathf.Abs(_lastTouchPos.x - _firstTouchPos.x) > TouchDragDistanceCameraOrientation ||
                                Mathf.Abs(_lastTouchPos.y - _firstTouchPos.y) > TouchDragDistanceCameraOrientation) //its a drag
                            {
                                if (_lastTouchPos.x - _firstTouchPos.x < 0)
                                {
                                    CameraOrientationInput[(int) Direction.Left] = true;
                                }

                                if (_lastTouchPos.x - _firstTouchPos.x > 0)
                                {
                                    CameraOrientationInput[(int) Direction.Right] = true;
                                }
                            }

                            break;
                    }
                }
            }
        }

        private void ResetInputs()
        {
            for (var i = 0; i < PlayerMovementInputs.Count; i++)
            {
                PlayerMovementInputs[i] = false;
            }

            for (var i = 0; i < CameraOrientationInput.Count; i++)
            {
                CameraOrientationInput[i] = false;
            }
        }
    }
}
