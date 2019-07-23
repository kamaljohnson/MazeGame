using System.Collections.Generic;
using System.Text;
using Game.Items.Path.Ice;
using Game.Managers;
using UnityEngine;

namespace Game.Player
{
    public class Movement : MonoBehaviour
    {        
        public Transform playerCube;
        public PlayerInput input;
        
        public float stepSize;

        public Transform rightAnchor;
        public Transform leftAnchor;
        public Transform forwardAnchor;
        public Transform backAnchor;

        public float speed = 400f;
        private float _tempAngleRotated;
        
        private Direction _movementDirection;
        private Direction _tempMovementDirection;
        public bool movementSnappedFull;
        public bool movementSnappedHalf;
        private int _snapCount;

        private bool _atVerticalDownEdge;
        private bool _atVerticalUpEdge;
        
        private bool _atJunction;

        public List<bool> playerRayCastData = new List<bool>()
        {
            false,    //right raycast data
            false,    //left raycast data
            false,    //forward raycast data
            false,    //back raycast data
            false,    //down raycast data
        };
        
        public void Start()
        {
            Reset();
        }

        public void Update()
        {
            if (GameManager.mazeTransform.gameObject.GetComponent<Maze.MazeRotator>().isRotating)    //no calculations will be made or applied while the maze is rotating
                return;
            
            HandleInput();

            if (movementSnappedFull)
            {
               CheckJunction();
            }

            if (_atJunction)
            {
                UpdateMoveableDirections();
            }
            else if(movementSnappedFull)
            {
                IntermediateMove();
            }
            
            if(!_atJunction)
            {
                Move();
            }
        }

        private void HandleInput()
        {
            Direction tempdirection = input.GetInputPlayerMovementDirection();
            if (tempdirection != Direction.None)
            {
                _tempMovementDirection = tempdirection;
            }
        }

        private void UpdateMoveableDirections()
        {
            if(_tempMovementDirection == Direction.None)
                return;
            
            if (_tempMovementDirection == _movementDirection)
            {
                _movementDirection = Direction.None;
                _tempMovementDirection = Direction.None;
                _atJunction = true;
                return;
            }
            
            if (!playerRayCastData[(int) _tempMovementDirection])
            {
                _movementDirection = _tempMovementDirection;
                _tempMovementDirection = Direction.None;
                _atJunction = false;
            }
        }

        private void IntermediateMove()
        {
            if (_tempMovementDirection == Direction.None)
            {
                return;
            }
            
            if (_tempMovementDirection != _movementDirection && !Ice.onIce)
            {
                if (_tempMovementDirection == Direction.Right && _movementDirection == Direction.Left)
                {
                    _movementDirection = _tempMovementDirection;
                    _tempMovementDirection = Direction.None;
                }
                else if (_tempMovementDirection == Direction.Left && _movementDirection == Direction.Right)
                {
                    _movementDirection = _tempMovementDirection;
                    _tempMovementDirection = Direction.None;
                }
                else if (_tempMovementDirection == Direction.Forward && _movementDirection == Direction.Back)
                {
                    _movementDirection = _tempMovementDirection;
                    _tempMovementDirection = Direction.None;
                }
                else if (_tempMovementDirection == Direction.Back && _movementDirection == Direction.Forward)
                {
                    _movementDirection = _tempMovementDirection;
                    _tempMovementDirection = Direction.None;
                }
            }
        }

        private void CheckJunction()
        {   
            //getting the ray-cast data
            var transformPosition = transform.position;

            //right ray-cast data
            playerRayCastData[(int) Direction.Right] = Physics.Raycast(transformPosition + Helper.DirectionVector[(int)Direction.Down] * 0.01f, Helper.DirectionVector[(int)Direction.Right], out _, stepSize + 0.1f * stepSize);
            var color = playerRayCastData[(int) Direction.Right] ? Color.red : Color.green;
            Debug.DrawRay(transformPosition + Helper.DirectionVector[(int)Direction.Down] * 0.01f, transform.right * (stepSize + 0.1f * stepSize), color);

            //left ray-cast data
            playerRayCastData[(int) Direction.Left] = Physics.Raycast(transformPosition + Helper.DirectionVector[(int)Direction.Down] * 0.01f, Helper.DirectionVector[(int)Direction.Left], out _, stepSize + 0.1f * stepSize);
            color = playerRayCastData[(int) Direction.Left] ? Color.red : Color.green;
            Debug.DrawRay(transformPosition + Helper.DirectionVector[(int)Direction.Down] * 0.01f, -transform.right * (stepSize + 0.1f * stepSize), color);
            
            //forward ray-cast data
            playerRayCastData[(int) Direction.Forward] = Physics.Raycast(transformPosition + Helper.DirectionVector[(int)Direction.Down] * 0.01f, Helper.DirectionVector[(int)Direction.Forward], out _, stepSize + 0.1f * stepSize);
            color = playerRayCastData[(int) Direction.Forward] ? Color.red : Color.green;
            Debug.DrawRay(transformPosition + Helper.DirectionVector[(int)Direction.Down] * 0.01f, transform.forward * (stepSize + 0.1f * stepSize), color);
            
            //back ray-cast data
            playerRayCastData[(int) Direction.Back] = Physics.Raycast(transformPosition + Helper.DirectionVector[(int)Direction.Down] * 0.01f, Helper.DirectionVector[(int)Direction.Back], out _, stepSize + 0.1f * stepSize);
            color = playerRayCastData[(int) Direction.Back] ? Color.red : Color.green;
            Debug.DrawRay(transformPosition + Helper.DirectionVector[(int)Direction.Down] * 0.01f, -transform.forward * (stepSize + 0.1f * stepSize) , color);
            
            
            //checking if the there is a new path in the perpendicular direction of travel
            if (!input.lockInputTillJunction && !Ice.onIce)
            {
                _atJunction = true;
            }
            else
            {
                switch (_movementDirection)
                {
                    case Direction.Right:
                        if (playerRayCastData[(int)Direction.Right])
                        {
                            _atJunction = true;                        
                        }
                        if (!playerRayCastData[(int) Direction.Forward] || !playerRayCastData[(int) Direction.Back])
                        {
                            if (!Ice.onIce)
                            {
                                _atJunction = true;
                            }
                        }
                        break;
                    case Direction.Left:
                        if (playerRayCastData[(int)Direction.Left])
                        {
                            _atJunction = true;                        
                        }
                        if (!playerRayCastData[(int) Direction.Forward] || !playerRayCastData[(int) Direction.Back])
                        {
                            if (!Ice.onIce)
                            {
                                _atJunction = true;
                            }
                        }
                        break;
                    case Direction.Forward:
                        if (playerRayCastData[(int)Direction.Forward])
                        {
                            _atJunction = true;                        
                        }
                        if (!playerRayCastData[(int) Direction.Right] || !playerRayCastData[(int) Direction.Left])
                        {
                            if (!Ice.onIce)
                            {
                                _atJunction = true;
                            }
                        }
                        break;
                    case Direction.Back:
                        if (playerRayCastData[(int)Direction.Back])
                        {
                            _atJunction = true;                        
                        }
                        if (!playerRayCastData[(int) Direction.Right] || !playerRayCastData[(int) Direction.Left])
                        {
                            if (!Ice.onIce)
                            {
                                _atJunction = true;
                            }
                        }
                        break;
                    case Direction.None:
                        _atJunction = true;
                        break;
                }
            }
        }

        private void Move()
        {
            float deltaAngle = speed * Time.deltaTime;
            if(!Ice.onIce)
            {
                switch (_movementDirection)
                {
                    case Direction.Right:
                        playerCube.RotateAround(rightAnchor.position, -transform.forward * stepSize, deltaAngle);
                        break;
                    case Direction.Left:
                        playerCube.RotateAround(leftAnchor.position, transform.forward * stepSize, deltaAngle);
                        break;
                    case Direction.Forward:
                        playerCube.RotateAround(forwardAnchor.position, transform.right * stepSize, deltaAngle);
                        break;
                    case Direction.Back:
                        playerCube.RotateAround(backAnchor.position, -transform.right * stepSize, deltaAngle);
                        break;
                }
            }
            else
            {
                playerCube.transform.localPosition = Vector3.Lerp(playerCube.transform.localPosition,
                    playerCube.transform.localPosition + Helper.DirectionVector[(int) _movementDirection], Time.deltaTime * 1.5f);
            }

            _tempAngleRotated += deltaAngle;
            
            if (_tempAngleRotated >= 90)
            {
                if (!CheckGroundUnderneath())
                {
                    playerCube.localEulerAngles = Helper.DirectionRotation[(int) _movementDirection] * 90;
                    
                    switch (_movementDirection)
                    {
                        case Direction.Right:
                            GameManager.mazeTransform.gameObject.GetComponent<Maze.MazeRotator>().Rotate(transform.forward, speed);
                            break;
                        case Direction.Left:
                            GameManager.mazeTransform.gameObject.GetComponent<Maze.MazeRotator>().Rotate(-transform.forward, speed);
                            break;
                        case Direction.Forward:
                            GameManager.mazeTransform.gameObject.GetComponent<Maze.MazeRotator>().Rotate(-transform.right, speed);
                            break;
                        case Direction.Back:
                            GameManager.mazeTransform.gameObject.GetComponent<Maze.MazeRotator>().Rotate(transform.right, speed);
                            break;
                    }
                    
                    _atVerticalDownEdge = true;
                }
                else
                {                   
                    if (_atVerticalDownEdge)
                    {
                        transform.position += Helper.DirectionVector[(int) Direction.Up] * stepSize;
                        transform.eulerAngles += Helper.DirectionRotation[(int) _movementDirection] * 90;
                        _atVerticalDownEdge = false;
                    }

                    transform.position += Helper.DirectionVector[(int) _movementDirection] * stepSize;
                    transform.eulerAngles = Vector3.zero;
                    
                    if (_snapCount == 2)
                    {
                        movementSnappedFull = true;
                        _snapCount = 0;
                    }
                    else
                    {
                        _snapCount++;
                    }

                   
                    playerCube.localPosition = Vector3.zero;
                    playerCube.localEulerAngles = Vector3.zero;

                    movementSnappedHalf = true;
                }
                
                _tempAngleRotated = 0;
            }
            else
            {
                movementSnappedHalf = false;
                movementSnappedFull = false;
            }

            if (movementSnappedHalf && !movementSnappedFull)
            {
                if (CheckWallInfront())
                {
                    _atVerticalUpEdge = true;
                    _snapCount++;
                    
                    transform.eulerAngles -= Helper.DirectionRotation[(int) _movementDirection] * 90;
                    
                    playerCube.localPosition = Vector3.zero;
                    playerCube.localEulerAngles = Vector3.zero;
                    
                    switch (_movementDirection)
                    {
                        case Direction.Right:
                            GameManager.mazeTransform.gameObject.GetComponent<Maze.MazeRotator>().Rotate(-transform.forward, speed);
                            break;
                        case Direction.Left:
                            GameManager.mazeTransform.gameObject.GetComponent<Maze.MazeRotator>().Rotate(transform.forward, speed);
                            break;
                        case Direction.Forward:
                            GameManager.mazeTransform.gameObject.GetComponent<Maze.MazeRotator>().Rotate(transform.right, speed);
                            break;
                        case Direction.Back:
                            GameManager.mazeTransform.gameObject.GetComponent<Maze.MazeRotator>().Rotate(-transform.right, speed);
                            break;
                    }                
                }
            }
            
        }

        private bool CheckWallInfront()
        {
            Color color;
            RaycastHit hit;
            
            playerRayCastData[(int)_movementDirection] = Physics.Raycast(playerCube.position + Helper.DirectionVector[(int)Direction.Down] * 0.01f, Helper.DirectionVector[(int)_movementDirection], out hit, stepSize + 0.1f * stepSize);
            if (playerRayCastData[(int) _movementDirection]){color = new Color(1f, 0.8f, 0.36f);}else{color = new Color(0.6f, 0.84f, 1f);}
            Debug.DrawRay(playerCube.position + Helper.DirectionVector[(int)Direction.Down] * 0.01f, -Helper.DirectionVector[(int)_movementDirection] * (stepSize + 0.1f * stepSize), color);
            
            return playerRayCastData[(int)_movementDirection];
        }

        private bool CheckGroundUnderneath()
        {
            Color color;
            RaycastHit hit;
            
            playerRayCastData[(int)Direction.Down] = Physics.Raycast(playerCube.position + Helper.DirectionVector[(int)Direction.Down] * 0.01f, Helper.DirectionVector[(int)Direction.Down], out hit, stepSize + 0.1f * stepSize);
            if (playerRayCastData[(int) Direction.Down]){color = Color.red;}else{color = Color.green;}
            Debug.DrawRay(playerCube.position + Helper.DirectionVector[(int)Direction.Down] * 0.01f, Helper.DirectionVector[(int)Direction.Up] * (stepSize + 0.1f * stepSize), color);
        
            
            return playerRayCastData[(int)Direction.Down];
        }

        public void SetParentMaze(GameObject maze)
        {
            GameManager.mazeTransform = maze.transform;
            transform.parent = GameManager.mazeTransform;
        }

        public void Reset()
        {
            var transform1 = playerCube.transform;
            
            stepSize = transform1.lossyScale.x;
            movementSnappedFull = true;
            movementSnappedHalf = true;
            _snapCount = 0;
            _atJunction = true;
            _movementDirection = Direction.None;
            _tempMovementDirection = Direction.None;

            _atVerticalUpEdge = false;
            _atVerticalDownEdge = false;
            
            transform1.localPosition = Vector3.zero;
            transform1.localEulerAngles = Vector3.zero;
        }
    }
}