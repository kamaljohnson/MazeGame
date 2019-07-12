using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{

    public enum InputTypes
    {
        Keyboard,
        SwipeInput,
        JoystickInput
    }
    
    public class PlayerInput : MonoBehaviour
    {
        public InputTypes inputType;
        public bool lockInputTillJunction;

        public Joystick movementJoystick;
        public Joystick attackJoystick;
        [Range(1, 10)]
        public float joystickSensitivity;
        
        [HideInInspector]
        public List<bool> playerMovementInputs = new List<bool>
        {
            false,    //right
            false,    //left
            false,    //forward
            false     //back
        };
        
        [HideInInspector]
        public List<bool> cameraOrientationInput = new List<bool>
        {
            false,    //right
            false     //left
        };
        
        private Vector3 _firstTouchPos;
        private Vector3 _lastTouchPos;

        public float screenPartitionPercentage;
        
        [Header("Player Movement Input")]
        public float touchOffDragScreenPercent;
        public float touchOnDragScreenPercent;
        private float _touchOffDragDistance;
        private float _touchOnDragDistance;

        [Header("Camera Orientation Input")] 
        public float touchDragScreenPercentCameraOrientation;
        private float _touchDragDistanceCameraOrientation;

        private bool _upperPartitionInteraction;

        public static int RotationCount;

        public void Start()
        {
            if (inputType != InputTypes.JoystickInput)
            {
                movementJoystick.gameObject.SetActive(false);
                attackJoystick.gameObject.SetActive(false);
            }
            RotationCount = 0;
            if (Application.platform == RuntimePlatform.Android)
            {
                _touchOnDragDistance = Screen.height * touchOnDragScreenPercent/100;
                _touchOffDragDistance = Screen.height * touchOffDragScreenPercent/100;
                _touchDragDistanceCameraOrientation = Screen.height * touchDragScreenPercentCameraOrientation/100;
            }
            else
            {
                inputType = InputTypes.Keyboard;
                _touchOnDragDistance = 10;
                _touchOffDragDistance = 10;
                _touchDragDistanceCameraOrientation = 10;
            }
        }

        public void Update()
        {
            ResetInputs();
            
            switch (inputType)
            {
                case InputTypes.Keyboard:
                    HandleKeyboardInputs();
                    break;
                case InputTypes.SwipeInput:
                    HandleSwipeInputs();
                    break;
                case InputTypes.JoystickInput:
                    HandleJoystickInputs();
                    break;
            }
        }

        /*
         * returns the change in the camera orientation
         */
        public Direction GetCameraOrientationDirection()
        {
            if (cameraOrientationInput[0])
            {
                return Direction.Right;
            }
            if (cameraOrientationInput[1])
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
            if (playerMovementInputs[((int) Direction.Right + RotationCount)%4])
            {
                return Direction.Right;
            }
            if (playerMovementInputs[((int) Direction.Left + RotationCount)%4])
            {
                return Direction.Left;
            }
            if (playerMovementInputs[((int) Direction.Forward + RotationCount)%4])
            {
                return Direction.Forward;
            }
            if (playerMovementInputs[((int) Direction.Back + RotationCount)%4])
            {
                return Direction.Back;
            }
            return Direction.None;
        }

        private void HandleKeyboardInputs()
        {
            if (Input.GetKey(KeyCode.D))
            {
                playerMovementInputs[(int)Direction.Right] = true;
            }
            if (Input.GetKey(KeyCode.A))
            {
                playerMovementInputs[(int)Direction.Left] = true;
            }
            if (Input.GetKey(KeyCode.W))
            {
                playerMovementInputs[(int)Direction.Forward] = true;
            }
            if (Input.GetKey(KeyCode.S))
            {
                playerMovementInputs[(int)Direction.Back] = true;
            }
            
            if(Input.GetKeyDown(KeyCode.E))
            {
                cameraOrientationInput[0] = true;
            }
            if(Input.GetKeyDown(KeyCode.Q))
            {
                cameraOrientationInput[1] = true;
            }
        }
        
        private void HandleSwipeInputs()
        {
            if (Input.touchCount == 1) // user is touching the screen with a single touch
            {
                var touch = Input.GetTouch(0); // get the touch
                
                //checking in which part of the screen the touch lies
                if (touch.phase == TouchPhase.Began)
                {
                    Debug.Log("touch input began");
                    _upperPartitionInteraction = touch.position.y >= Screen.height * screenPartitionPercentage / 100;
                    _firstTouchPos = touch.position;
                    _lastTouchPos = touch.position;
                }
                
                Debug.Log("Partition Interaction : " + (_upperPartitionInteraction ? "upper": "lower"));
                
                if (_upperPartitionInteraction)
                {
                    switch (touch.phase)
                    {
                        /*
                         *the _lastTuchPos is updated
                         *it also handles the onScreenTouch drag input
                         */
                        case TouchPhase.Moved:
                            _lastTouchPos = touch.position;
                            //Check if drag distance is greater than touchOnDragDistance
                            if (Mathf.Abs(_lastTouchPos.x - _firstTouchPos.x) > _touchOnDragDistance ||
                                Mathf.Abs(_lastTouchPos.y - _firstTouchPos.y) > _touchOnDragDistance) //its a drag
                            {
                                /* check if the drag is vertical or horizontal
                                 *
                                */
                                if (_lastTouchPos.x - _firstTouchPos.x > 0 && _lastTouchPos.y - _firstTouchPos.y > 0)
                                {
                                    playerMovementInputs[(int) Direction.Forward] = true;
                                }

                                if (_lastTouchPos.x - _firstTouchPos.x < 0 && _lastTouchPos.y - _firstTouchPos.y < 0)
                                {
                                    playerMovementInputs[(int) Direction.Back] = true;
                                }

                                if (_lastTouchPos.x - _firstTouchPos.x < 0 && _lastTouchPos.y - _firstTouchPos.y > 0)
                                {
                                    playerMovementInputs[(int) Direction.Left] = true;
                                }

                                if (_lastTouchPos.x - _firstTouchPos.x > 0 && _lastTouchPos.y - _firstTouchPos.y < 0)
                                {
                                    playerMovementInputs[(int) Direction.Right] = true;
                                }

                                _firstTouchPos = _lastTouchPos;
                            }

                            break;
                        //check if the finger is removed from the screen
                        case TouchPhase.Ended:
                            _lastTouchPos = touch.position;

                            //Check if drag distance is greater than touchOffDragDistance
                            if (Mathf.Abs(_lastTouchPos.x - _firstTouchPos.x) > _touchOffDragDistance ||
                                Mathf.Abs(_lastTouchPos.y - _firstTouchPos.y) > _touchOffDragDistance) //its a drag
                            {
                                /* check if the drag is vertical or horizontal
                                 *
                                 */
                                if (_lastTouchPos.x - _firstTouchPos.x > 0 && _lastTouchPos.y - _firstTouchPos.y > 0)
                                {
                                    playerMovementInputs[(int) Direction.Forward] = true;
                                }

                                if (_lastTouchPos.x - _firstTouchPos.x < 0 && _lastTouchPos.y - _firstTouchPos.y < 0)
                                {
                                    playerMovementInputs[(int) Direction.Back] = true;
                                }

                                if (_lastTouchPos.x - _firstTouchPos.x < 0 && _lastTouchPos.y - _firstTouchPos.y > 0)
                                {
                                    playerMovementInputs[(int) Direction.Left] = true;
                                }

                                if (_lastTouchPos.x - _firstTouchPos.x > 0 && _lastTouchPos.y - _firstTouchPos.y < 0)
                                {
                                    playerMovementInputs[(int) Direction.Right] = true;
                                }
                            }

                            break;
                    }
                }
                else
                {
                    switch (touch.phase)
                    {
                        //check if the finger is removed from the screen
                        case TouchPhase.Ended:
                            _lastTouchPos = touch.position; 

                            //Check if drag distance is greater than touchOffDragDistance
                            if (Mathf.Abs(_lastTouchPos.x - _firstTouchPos.x) > _touchDragDistanceCameraOrientation ||
                                Mathf.Abs(_lastTouchPos.y - _firstTouchPos.y) > _touchDragDistanceCameraOrientation) //its a drag
                            {
                                if (_lastTouchPos.x - _firstTouchPos.x < 0)
                                {
                                    cameraOrientationInput[1] = true;
                                }

                                if (_lastTouchPos.x - _firstTouchPos.x > 0)
                                {
                                    cameraOrientationInput[0] = true;
                                }
                            }

                            break;
                    }
                }
            }
        }
        
        private void HandleJoystickInputs()
        {
            // Handles movement of the player cube
            if (movementJoystick.Horizontal >= 1/joystickSensitivity && movementJoystick.Vertical >= 1/joystickSensitivity)
            {
                playerMovementInputs[(int)Direction.Forward] = true;
            }
            if (movementJoystick.Horizontal <= -1/joystickSensitivity && movementJoystick.Vertical <= -1/joystickSensitivity)
            {
                playerMovementInputs[(int)Direction.Back] = true;
            }
            if (movementJoystick.Horizontal >= 1/joystickSensitivity && movementJoystick.Vertical <= -1/joystickSensitivity)
            {
                playerMovementInputs[(int)Direction.Right] = true;
            }
            if (movementJoystick.Horizontal <= -1/joystickSensitivity && movementJoystick.Vertical >= 1/joystickSensitivity)
            {
                playerMovementInputs[(int)Direction.Left] = true;
            }
            
            //TODO: handle attack joystick commands
            
            //TODO: this should be changed to touch inputs
            /*if(Input.GetKeyDown(KeyCode.E))
            {
                cameraOrientationInput[0] = true;
            }
            if(Input.GetKeyDown(KeyCode.Q))
            {
                cameraOrientationInput[1] = true;
            }*/
        }
        
        private void ResetInputs()
        {
            for (var i = 0; i < playerMovementInputs.Count; i++)
            {
                playerMovementInputs[i] = false;
            }

            for (var i = 0; i < cameraOrientationInput.Count; i++)
            {
                cameraOrientationInput[i] = false;
            }
        }
    }
}
