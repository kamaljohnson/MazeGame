using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public class Movement : MonoBehaviour
    {
        public GameObject ParentMaze;
        public Transform PlayerCube;
        public PlayerInput Input;

        public float StepSize;

        public Transform RightAnchor;
        public Transform LeftAnchor;
        public Transform ForwardAnchor;
        public Transform BackAnchor;

        public float Speed = 400f;
        private float _tempAngleRotated;
        
        private Direction _movementDirection;
        private Direction _tempMovementDirection;
        private bool _movementSnappedFull;
        private bool _movementSnappedHalf;
        private int _snapCount;

        private bool _atVerticalDownEdge;
        private bool _atVerticalUpEdge;
        
        private bool _atJunction;

        public List<bool> PlayerRayCastData = new List<bool>()
        {
            false,    //right raycast data
            false,    //left raycast data
            false,    //forward raycast data
            false,    //back raycast data
            false,    //down raycast data
        };
        
        public void Start()
        {
            StepSize = PlayerCube.transform.lossyScale.x;
            _movementSnappedFull = true;
            _movementSnappedHalf = true;
            _snapCount = 0;
            _atJunction = true;
            _movementDirection = Direction.None;
            _tempMovementDirection = Direction.None;

            _atVerticalUpEdge = false;
            _atVerticalDownEdge = false;
        }

        public void Update()
        {
            if (ParentMaze.GetComponent<Maze.MazeRotator>().IsRotating)    //no calculations will be made or applied while the maze is rotating
                return;
            
            HandleInput();

            if(_movementSnappedFull)
            {
                CheckJunction();
            }

            if (_atJunction)
            {
                UpdateMoveableDirections();
            }
            else if(_movementSnappedFull)
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
            Direction tempdirection = Input.GetInputPlayerMovementDirection();
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
            
            if (!PlayerRayCastData[(int) _tempMovementDirection])
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
            
            if (_tempMovementDirection != _movementDirection)
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
            //getting the raycast data
            Color color;
            RaycastHit hit;
            
            //right raycast data
            PlayerRayCastData[(int) Direction.Right] = Physics.Raycast(transform.position + Helper.DirectionVector[(int)Direction.Down] * 0.01f, Helper.DirectionVector[(int)Direction.Right], out hit, StepSize + 0.1f * StepSize);
            if(PlayerRayCastData[(int) Direction.Right]){color = Color.red;}else{color = Color.green;}
            Debug.DrawRay(transform.position + Helper.DirectionVector[(int)Direction.Down] * 0.01f, transform.right * (StepSize + 0.1f * StepSize), color);

            //left raycast data
            PlayerRayCastData[(int) Direction.Left] = Physics.Raycast(transform.position + Helper.DirectionVector[(int)Direction.Down] * 0.01f, Helper.DirectionVector[(int)Direction.Left], out hit, StepSize + 0.1f * StepSize);
            if(PlayerRayCastData[(int) Direction.Left]){color = Color.red;}else{color = Color.green;}
            Debug.DrawRay(transform.position + Helper.DirectionVector[(int)Direction.Down] * 0.01f, -transform.right * (StepSize + 0.1f * StepSize), color);
            
            //forward raycast data
            PlayerRayCastData[(int) Direction.Forward] = Physics.Raycast(transform.position + Helper.DirectionVector[(int)Direction.Down] * 0.01f, Helper.DirectionVector[(int)Direction.Forward], out hit, StepSize + 0.1f * StepSize);
            if(PlayerRayCastData[(int) Direction.Forward]){color = Color.red;}else{color = Color.green;}
            Debug.DrawRay(transform.position + Helper.DirectionVector[(int)Direction.Down] * 0.01f, transform.forward * (StepSize + 0.1f * StepSize), color);
            
            //back raycast data
            PlayerRayCastData[(int) Direction.Back] = Physics.Raycast(transform.position + Helper.DirectionVector[(int)Direction.Down] * 0.01f, Helper.DirectionVector[(int)Direction.Back], out hit, StepSize + 0.1f * StepSize);
            if(PlayerRayCastData[(int) Direction.Back]){color = Color.red;}else{color = Color.green;}
            Debug.DrawRay(transform.position + Helper.DirectionVector[(int)Direction.Down] * 0.01f, -transform.forward * (StepSize + 0.1f * StepSize) , color);
            
            
            //checking if the there is a new path in the perpendicular dirction of travel
            switch (_movementDirection)
            {
                case Direction.Right:
                    if (PlayerRayCastData[(int) Direction.Right] ||
                        !PlayerRayCastData[(int) Direction.Forward] || !PlayerRayCastData[(int) Direction.Back])
                    {
                        _atJunction = true;
                    }
                    break;
                case Direction.Left:
                    if (PlayerRayCastData[(int) Direction.Left] ||
                        !PlayerRayCastData[(int) Direction.Forward] || !PlayerRayCastData[(int) Direction.Back])
                    {
                        _atJunction = true;
                    }
                    break;
                case Direction.Forward:
                    if (PlayerRayCastData[(int) Direction.Forward] ||
                        !PlayerRayCastData[(int) Direction.Right] || !PlayerRayCastData[(int) Direction.Left])
                    {
                        _atJunction = true;
                    }break;
                case Direction.Back:
                    if (PlayerRayCastData[(int) Direction.Back] ||
                        !PlayerRayCastData[(int) Direction.Right] || !PlayerRayCastData[(int) Direction.Left])
                    {
                        _atJunction = true;
                    }
                    break;
                case Direction.None:
                    _atJunction = true;
                    break;
            }
        }

        private void Move()
        {
            float deltaAngle = Speed * Time.deltaTime;
            switch (_movementDirection)
            {
                case Direction.Right:
                    PlayerCube.RotateAround(RightAnchor.position, -transform.forward * StepSize, deltaAngle);
                    break;
                case Direction.Left:
                    PlayerCube.RotateAround(LeftAnchor.position, transform.forward * StepSize, deltaAngle);
                    break;
                case Direction.Forward:
                    PlayerCube.RotateAround(ForwardAnchor.position, transform.right * StepSize, deltaAngle);
                    break;
                case Direction.Back:
                    PlayerCube.RotateAround(BackAnchor.position, -transform.right * StepSize, deltaAngle);
                    break;
            }

            _tempAngleRotated += deltaAngle;
            
            if (_tempAngleRotated >= 90)
            {
                if (!CheckGroundUnderneath())
                {
                    PlayerCube.localEulerAngles = Helper.DirectionRotation[(int) _movementDirection] * 90;
//                    PlayerCube.localPosition = Helper.DirectionVector[(int) _movementDirection] * StepSize;

                    switch (_movementDirection)
                    {
                        case Direction.Right:
                            ParentMaze.GetComponent<Maze.MazeRotator>().Rotate(transform.forward, Speed);
                            break;
                        case Direction.Left:
                            ParentMaze.GetComponent<Maze.MazeRotator>().Rotate(-transform.forward, Speed);
                            break;
                        case Direction.Forward:
                            ParentMaze.GetComponent<Maze.MazeRotator>().Rotate(-transform.right, Speed);
                            break;
                        case Direction.Back:
                            ParentMaze.GetComponent<Maze.MazeRotator>().Rotate(transform.right, Speed);
                            break;
                    }
                    
                    _atVerticalDownEdge = true;
                }
                else
                {                   
                    if (_atVerticalDownEdge)
                    {
                        transform.position += Helper.DirectionVector[(int) Direction.Up] * StepSize;
                        transform.eulerAngles += Helper.DirectionRotation[(int) _movementDirection] * 90;
                        _atVerticalDownEdge = false;
                    }

                    transform.position += Helper.DirectionVector[(int) _movementDirection] * StepSize;
                    transform.eulerAngles = Vector3.zero;
                    
                    if (_snapCount == 2)
                    {
                        _movementSnappedFull = true;
                        _snapCount = 0;
                    }
                    else
                    {
                        _snapCount++;
                    }

                   
                    PlayerCube.localPosition = Vector3.zero;
                    PlayerCube.localEulerAngles = Vector3.zero;

                    _movementSnappedHalf = true;
                }
                
                _tempAngleRotated = 0;
            }
            else
            {
                _movementSnappedHalf = false;
                _movementSnappedFull = false;
            }

            if (_movementSnappedHalf && !_movementSnappedFull)
            {
                if (CheckWallInfront())
                {
                    _atVerticalUpEdge = true;
                    _snapCount++;
                    
                    transform.eulerAngles -= Helper.DirectionRotation[(int) _movementDirection] * 90;
                    
                    PlayerCube.localPosition = Vector3.zero;
                    PlayerCube.localEulerAngles = Vector3.zero;
                    
                    switch (_movementDirection)
                    {
                        case Direction.Right:
                            ParentMaze.GetComponent<Maze.MazeRotator>().Rotate(-transform.forward, Speed);
                            break;
                        case Direction.Left:
                            ParentMaze.GetComponent<Maze.MazeRotator>().Rotate(transform.forward, Speed);
                            break;
                        case Direction.Forward:
                            ParentMaze.GetComponent<Maze.MazeRotator>().Rotate(transform.right, Speed);
                            break;
                        case Direction.Back:
                            ParentMaze.GetComponent<Maze.MazeRotator>().Rotate(-transform.right, Speed);
                            break;
                    }                
                }
            }
            
        }

        private bool CheckWallInfront()
        {
            Color color;
            RaycastHit hit;
            
            PlayerRayCastData[(int)_movementDirection] = Physics.Raycast(PlayerCube.position + Helper.DirectionVector[(int)Direction.Down] * 0.01f, Helper.DirectionVector[(int)_movementDirection], out hit, StepSize + 0.1f * StepSize);
            if (PlayerRayCastData[(int) _movementDirection]){color = new Color(1f, 0.8f, 0.36f);}else{color = new Color(0.6f, 0.84f, 1f);}
            Debug.DrawRay(PlayerCube.position + Helper.DirectionVector[(int)Direction.Down] * 0.01f, -Helper.DirectionVector[(int)_movementDirection] * (StepSize + 0.1f * StepSize), color);
            
            return PlayerRayCastData[(int)_movementDirection];
        }

        private bool CheckGroundUnderneath()
        {
            Color color;
            RaycastHit hit;
            
            PlayerRayCastData[(int)Direction.Down] = Physics.Raycast(PlayerCube.position + Helper.DirectionVector[(int)Direction.Down] * 0.01f, Helper.DirectionVector[(int)Direction.Down], out hit, StepSize + 0.1f * StepSize);
            if (PlayerRayCastData[(int) Direction.Down]){color = Color.red;}else{color = Color.green;}
            Debug.DrawRay(PlayerCube.position + Helper.DirectionVector[(int)Direction.Down] * 0.01f, Helper.DirectionVector[(int)Direction.Up] * (StepSize + 0.1f * StepSize), color);
        
            
            return PlayerRayCastData[(int)Direction.Down];
        }

        public void SetParentMaze(GameObject maze)
        {
            ParentMaze = maze;
            transform.parent = ParentMaze.transform;
        }
    }
}