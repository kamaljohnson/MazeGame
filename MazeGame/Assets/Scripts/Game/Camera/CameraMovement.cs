using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Camera
{
    public class CameraMovement : MonoBehaviour
    {
        public PlayerInput Input;
        public Transform CameraTransform;
        
        private Transform CameraPivotMazeTransform;
        private Direction CameraChangeDirection;

        public void Start()
        {
            CameraChangeDirection = Direction.None;
        }

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
            var tempDirection = Input.GetCameraOrientationDirection();
            if (tempDirection != Direction.None)
            {
                CameraChangeDirection = tempDirection;
            }
        }

        private void ChangeOrientation()
        {
            CameraPivotMazeTransform = GameManager.CurrentMazeTransfomr;
            
            switch (CameraChangeDirection)
            {
                case Direction.Right:
                    CameraTransform.RotateAround(CameraPivotMazeTransform.position, Vector3.up, 45);
                    CameraChangeDirection = Direction.None;
                    break;
                case Direction.Left:
                    CameraTransform.RotateAround(CameraPivotMazeTransform.position, Vector3.up, -45);
                    CameraChangeDirection = Direction.None;
                    break;
            }
        }
    }
}