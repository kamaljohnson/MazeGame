using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Camera
{
    public class CameraMovement : MonoBehaviour
    {
        public PlayerInput Input;
        public Transform CameraTransform;
        public float RotationSpeed;
        
        private Transform cameraPivotMazeTransform;
        private Direction tempChangeDirection;
        private Direction changeDirection;

        private bool isChangingOrientation;
        private float rotationAngle;
        
        public void Start()
        {
            changeDirection = Direction.None;
            tempChangeDirection = Direction.None;
            isChangingOrientation = false;
        }

        public void Update()
        {
            HandleInput();
            if (tempChangeDirection != Direction.None && !isChangingOrientation)
            {
                changeDirection = tempChangeDirection;
                tempChangeDirection = Direction.None;
                
                cameraPivotMazeTransform = GameManager.CurrentMazeTransform;
                isChangingOrientation = true;
            }

            if (isChangingOrientation)
            {
                ChangeOrientation();
            }
        }
        
        private void HandleInput()
        {
            var tempDirection = Input.GetCameraOrientationDirection();
            if (tempDirection != Direction.None)
            {
                tempChangeDirection = tempDirection;
            }
        }

        private void ChangeOrientation()
        {
            float tempRotationAngle = Time.deltaTime;
            CameraTransform.RotateAround(cameraPivotMazeTransform.position, Vector3.up, (changeDirection == Direction.Right ? 1: -1 ) * tempRotationAngle * RotationSpeed);
            rotationAngle += tempRotationAngle * RotationSpeed;

            
            if (rotationAngle > 90)
            {
                CameraTransform.eulerAngles = new Vector3((int)CameraTransform.eulerAngles.x, (int)CameraTransform.eulerAngles.y, (int)CameraTransform.eulerAngles.z);
                isChangingOrientation = false;
                rotationAngle = 0;
            }
        }
    }
}