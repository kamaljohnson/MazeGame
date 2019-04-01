using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Camera
{
    public class CameraMovement : MonoBehaviour
    {
        public PlayerInput Input;
        public Transform CameraTransform;
        public Transform CameraPivotMazeTransform;
        
        private Direction CameraChangeDirection;

        public void Update()
        {
            HandleInput();
            if (CameraChangeDirection != Direction.None)
            {
                ChangeOrientation();
            }
        }
        
        private void HandleInput()
        {
            var tempDirection = Input.GetInputCameraOrientationChangeDirection();
            if (tempDirection != Direction.None)
            {
                CameraChangeDirection = tempDirection;
            }
        }

        private void ChangeOrientation()
        {
            switch (CameraChangeDirection)
            {
                case Direction.Right:
                    CameraTransform.RotateAround(CameraPivotMazeTransform.position, Vector3.up, 90);
                    CameraChangeDirection = Direction.None;
                    break;
                case Direction.Left:
                    CameraTransform.RotateAround(CameraPivotMazeTransform.position, Vector3.up, -90);
                    CameraChangeDirection = Direction.None;
                    break;
            }
        }
    }
}