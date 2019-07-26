using System.Collections;
using System.Collections.Generic;
using Game.Managers;
using UnityEngine;

namespace Game.Camera
{
    public class CameraMovement : MonoBehaviour
    {
        public PlayerInput input;
        public Transform cameraTransform;
        public float rotationStep;
        
        private Transform _cameraPivotMazeTransform;
        private Direction _tempChangeDirection;
        private Direction _changeDirection;

        private bool _isChangingOrientation;
        private float _angle;
        
        public void Start()
        {
            _changeDirection = Direction.None;
            _tempChangeDirection = Direction.None;
            _isChangingOrientation = false;
        }

        public void Update()
        {
            if (GameManager.gameState == GameManager.GameStates.Playing)
            {
                HandleInput();
            }
            if (_tempChangeDirection != Direction.None && !_isChangingOrientation)
            {
                PlayerInput.RotationCount += _tempChangeDirection == Direction.Right ? 1: -1;
                if (PlayerInput.RotationCount > 3)
                {
                    PlayerInput.RotationCount = 0;
                }

                if (PlayerInput.RotationCount < 0)
                {
                    PlayerInput.RotationCount = 3;
                }
                _changeDirection = _tempChangeDirection;
                _tempChangeDirection = Direction.None;
                
                _cameraPivotMazeTransform = GameManager.mazeTransform;
                _isChangingOrientation = true;
            }

            if (_isChangingOrientation)
            {
                StartCoroutine(nameof(ChangeOrientation));
            }
        }
        
        private void HandleInput()
        {
            var tempDirection = input.GetCameraOrientationDirection();
            if (tempDirection != Direction.None)
            {
                _tempChangeDirection = tempDirection;
            }
        }

        private IEnumerator ChangeOrientation()
        {
            cameraTransform.RotateAround(_cameraPivotMazeTransform.position, Vector3.up, (_changeDirection == Direction.Right ? 1 : -1) * rotationStep);
            _angle += rotationStep;
            if (_angle == 90)
            {
                _isChangingOrientation = false;
                _angle = 0;
            }
            yield return new WaitForSeconds(90 / rotationStep);
        }
    }
}