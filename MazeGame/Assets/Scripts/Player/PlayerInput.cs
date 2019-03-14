using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

namespace Game.Player
{
    public class PlayerInput : MonoBehaviour
    {
        
        public List<bool> Inputs = new List<bool>
        {
            false,    //right
            false,    //left
            false,    //forward
            false     //back
        };

        private Vector3 _firstTouchPos;
        private Vector3 _lastTouchPos;

        public float TouchOffDragDistance;
        public float TouchOnDragDistance;
        
        public void Update()
        {
            ResetInputs();
            
            HandleKeyboardInput();
            HandleTouchInput();
        }
        
        public Direction GetInputDirection()
        {
            if (Inputs[(int) Direction.Right])
            {
                return Direction.Right;
            }
            if (Inputs[(int) Direction.Left])
            {
                return Direction.Left;
            }
            if (Inputs[(int) Direction.Forward])
            {
                return Direction.Forward;
            }
            if (Inputs[(int) Direction.Back])
            {
                return Direction.Back;
            }
            return Direction.None;
        }

        private void HandleKeyboardInput()
        {
            if (Input.GetKey(KeyCode.D))
            {
                Inputs[(int)Direction.Right] = true;
            }
            if (Input.GetKey(KeyCode.A))
            {
                Inputs[(int)Direction.Left] = true;
            }
            if (Input.GetKey(KeyCode.W))
            {
                Inputs[(int)Direction.Forward] = true;
            }
            if (Input.GetKey(KeyCode.S))
            {
                Inputs[(int)Direction.Back] = true;
            }
        }
        
        private void HandleTouchInput()
        {
            if (Input.touchCount == 1) // user is touching the screen with a single touch
            {
                var touch = Input.GetTouch(0); // get the touch
                switch (touch.phase)
                {
                    //check for the first touch
                    case TouchPhase.Began:
                        _firstTouchPos = touch.position;
                        _lastTouchPos = touch.position;
                        break;
                    /*
                     *the _lastTuchPos is updated
                     *it also handles the onScreenTouch drag input
                     */
                    case TouchPhase.Moved:
                        _lastTouchPos = touch.position;
                        //Check if drag distance is greater than 20% of the screen height
                        if (Mathf.Abs(_lastTouchPos.x - _firstTouchPos.x) > TouchOnDragDistance || Mathf.Abs(_lastTouchPos.y - _firstTouchPos.y) > TouchOnDragDistance)    //its a drag
                        {
                            /* check if the drag is vertical or horizontal
                             *
                            */
                            if (_lastTouchPos.x - _firstTouchPos.x > 0 && _lastTouchPos.y - _firstTouchPos.y > 0)
                            {
                                Inputs[(int)Direction.Forward] = true;
                            }
                            if (_lastTouchPos.x - _firstTouchPos.x < 0 && _lastTouchPos.y - _firstTouchPos.y < 0)
                            {   
                                Inputs[(int)Direction.Back] = true;
                            }
                            if (_lastTouchPos.x - _firstTouchPos.x < 0 && _lastTouchPos.y - _firstTouchPos.y > 0)
                            {   
                                Inputs[(int)Direction.Left] = true;
                            }
                            if (_lastTouchPos.x - _firstTouchPos.x > 0 && _lastTouchPos.y - _firstTouchPos.y < 0)
                            {   
                                Inputs[(int)Direction.Right] = true;
                            }
                            _firstTouchPos = _lastTouchPos;
                        }
                        break;
                    //check if the finger is removed from the screen
                    case TouchPhase.Ended:
                        _lastTouchPos = touch.position;  //last touch position. Ommitted if you use list
    
                        //Check if drag distance is greater than 20% of the screen height
                        if (Mathf.Abs(_lastTouchPos.x - _firstTouchPos.x) > TouchOffDragDistance || Mathf.Abs(_lastTouchPos.y - _firstTouchPos.y) > TouchOffDragDistance)    //its a drag
                        {
                            /* check if the drag is vertical or horizontal
                             *
                             */
                            if (_lastTouchPos.x - _firstTouchPos.x > 0 && _lastTouchPos.y - _firstTouchPos.y > 0)
                            {
                                Inputs[(int)Direction.Forward] = true;
                            }
                            if (_lastTouchPos.x - _firstTouchPos.x < 0 && _lastTouchPos.y - _firstTouchPos.y < 0)
                            {   
                                Inputs[(int)Direction.Back] = true;
                            }
                            if (_lastTouchPos.x - _firstTouchPos.x < 0 && _lastTouchPos.y - _firstTouchPos.y > 0)
                            {   
                                Inputs[(int)Direction.Left] = true;
                            }
                            if (_lastTouchPos.x - _firstTouchPos.x > 0 && _lastTouchPos.y - _firstTouchPos.y < 0)
                            {   
                                Inputs[(int)Direction.Right] = true;
                            }
                        }

                        break;
                }
            }
        }

        private void ResetInputs()
        {
            for (var i = 0; i < Inputs.Count; i++)
            {
                Inputs[i] = false;
            }
        }
    }
}
