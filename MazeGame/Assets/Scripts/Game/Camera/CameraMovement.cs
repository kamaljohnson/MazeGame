using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Camera
{
    public class CameraMovement : MonoBehaviour
    {
        public PlayerInput Input;
        public Transform CameraTransform;
        public float RotationStep;
        
        private Transform cameraPivotMazeTransform;
        private Direction tempChangeDirection;
        private Direction changeDirection;

        private bool isChangingOrientation;
        private float angle;
        
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
                StartCoroutine(nameof(ChangeOrientation));
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

        private IEnumerator ChangeOrientation()
        {
            CameraTransform.RotateAround(cameraPivotMazeTransform.position, Vector3.up, (changeDirection == Direction.Right ? 1 : -1) * RotationStep);
            angle += RotationStep;
            if (angle == 90)
            {
                isChangingOrientation = false;
                angle = 0;
            }
            yield return new WaitForSeconds(90 / RotationStep);
        }
    }
}