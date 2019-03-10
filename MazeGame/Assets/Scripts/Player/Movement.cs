using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public class Movement : MonoBehaviour
    {
        public GameObject ParentMaze;
        public Transform PlayerCube;
        public PlayerInput Input;

        public float stepSize = 1 / 3f;

        public Transform RightAnchor;
        public Transform LeftAnchor;
        public Transform ForwardAnchor;
        public Transform BackAnchor;

        public float Speed = 400f;
        private float tempAngleRotated;
        private float angleRotated;
        
        private Direction movementDirection;
        private Direction tempMovementDirection;
        private bool movementSnapped_full;
        private bool movementSnapped_half;
        private int snapCount;

        private bool atVerticalDownEdge;
        private bool atVerticalUpEdge;
        
        private bool atJunction;

        public List<bool> PlayerRayCastData = new List<bool>()
        {
            false,    //right raycast data
            false,    //left raycast data
            false,    //forward raycast data
            false,    //back raycast data
            false,    //down raycast data
        };
        private List<Vector3> DirectionRotation = new List<Vector3>()
        {
            new Vector3(0, 0, -1),    //right axis rotation
            new Vector3(0, 0, 1),     //left axis rotation
            new Vector3(1, 0, 0),     //forward axis rotation
            new Vector3(-1, 0, 0),    //back axis rotation
        };
        private List<Vector3> DirectionVector = new List<Vector3>()
        {
            new Vector3(1, 0, 0),        //right direction vector
            new Vector3(-1, 0, 0),       //left direction vector
            new Vector3(0, 0, 1),        //forward direction vector
            new Vector3(0, 0, -1),       //back direction vector
            new Vector3(0, -1, 0),       //down direction vector
            new Vector3(0, 1, 0),        //up direction vector
        };
        
        public void Start()
        {
            movementSnapped_full = true;
            movementSnapped_half = true;
            snapCount = 0;
            atJunction = true;
            movementDirection = Direction.None;
            tempMovementDirection = Direction.None;

            atVerticalUpEdge = false;
            atVerticalDownEdge = false;
        }

        public void Update()
        {
            if (ParentMaze.GetComponent<Maze.MazeRotator>().IsRotating)    //no calculations will be made or applied while the maze is rotating
                return;

            HandleInput();

            if(movementSnapped_full)
            {
                CheckJunction();
                IntermediateMove();
            }

            if (atJunction)
            {
                UpdateMoveableDirections();
            }
            
            if(!atJunction)
            {
                Move();
            }
        }
      
        public void HandleInput()
        {
            Direction tempdirection = Input.GetInputDirection();
            if (tempdirection != Direction.None)
            {
                tempMovementDirection = tempdirection;
            }
        }
        
        public void UpdateMoveableDirections()
        {
            if(tempMovementDirection == Direction.None)
                return;
            if (tempMovementDirection == movementDirection)
            {
                movementDirection = Direction.None;
                tempMovementDirection = Direction.None;
                atJunction = true;
                return;
            }
            
            if (!PlayerRayCastData[(int) tempMovementDirection])
            {
                movementDirection = tempMovementDirection;
                tempMovementDirection = Direction.None;
                atJunction = false;
            }
        }

        public void IntermediateMove()
        {
            if (tempMovementDirection == Direction.None)
            {
                return;
            }
            
            if (tempMovementDirection != movementDirection)
            {
                if (tempMovementDirection == Direction.Right && movementDirection == Direction.Left)
                {
                    movementDirection = tempMovementDirection;
                    tempMovementDirection = Direction.None;
                }
                else if (tempMovementDirection == Direction.Left && movementDirection == Direction.Right)
                {
                    movementDirection = tempMovementDirection;
                    tempMovementDirection = Direction.None;
                }
                else if (tempMovementDirection == Direction.Forward && movementDirection == Direction.Back)
                {
                    movementDirection = tempMovementDirection;
                    tempMovementDirection = Direction.None;
                }
                else if (tempMovementDirection == Direction.Back && movementDirection == Direction.Forward)
                {
                    movementDirection = tempMovementDirection;
                    tempMovementDirection = Direction.None;
                }
            }
        }
        
        public void CheckJunction()
        {
            //getting the raycast data
            Color _color;

            //right raycast data
            PlayerRayCastData[(int) Direction.Right] = Physics.Raycast(transform.position + DirectionVector[(int)Direction.Down] * 0.01f, DirectionVector[(int)Direction.Right], out RaycastHit hit, stepSize + 0.1f * stepSize);
            if(PlayerRayCastData[(int) Direction.Right]){_color = Color.red;}else{_color = Color.green;}
            Debug.DrawRay(transform.position + DirectionVector[(int)Direction.Down] * 0.01f, transform.right * (stepSize + 0.1f * stepSize), _color);

            //left raycast data
            PlayerRayCastData[(int) Direction.Left] = Physics.Raycast(transform.position + DirectionVector[(int)Direction.Down] * 0.01f, DirectionVector[(int)Direction.Left], out hit, stepSize + 0.1f * stepSize);
            if(PlayerRayCastData[(int) Direction.Left]){_color = Color.red;}else{_color = Color.green;}
            Debug.DrawRay(transform.position + DirectionVector[(int)Direction.Down] * 0.01f, -transform.right * (stepSize + 0.1f * stepSize), _color);
            
            //forward raycast data
            PlayerRayCastData[(int) Direction.Forward] = Physics.Raycast(transform.position + DirectionVector[(int)Direction.Down] * 0.01f, DirectionVector[(int)Direction.Forward], out hit, stepSize + 0.1f * stepSize);
            if(PlayerRayCastData[(int) Direction.Forward]){_color = Color.red;}else{_color = Color.green;}
            Debug.DrawRay(transform.position + DirectionVector[(int)Direction.Down] * 0.01f, transform.forward * (stepSize + 0.1f * stepSize), _color);
            
            //back raycast data
            PlayerRayCastData[(int) Direction.Back] = Physics.Raycast(transform.position + DirectionVector[(int)Direction.Down] * 0.01f, DirectionVector[(int)Direction.Back], out hit, stepSize + 0.1f * stepSize);
            if(PlayerRayCastData[(int) Direction.Back]){_color = Color.red;}else{_color = Color.green;}
            Debug.DrawRay(transform.position + DirectionVector[(int)Direction.Down] * 0.01f, -transform.forward * (stepSize + 0.1f * stepSize) , _color);
            
            
            //checking if the there is a new path in the perpendicular dirction of travel
            switch (movementDirection)
            {
                case Direction.Right:
                    if (PlayerRayCastData[(int) Direction.Right] ||
                        !PlayerRayCastData[(int) Direction.Forward] || !PlayerRayCastData[(int) Direction.Back])
                    {
                        atJunction = true;
                    }
                    break;
                case Direction.Left:
                    if (PlayerRayCastData[(int) Direction.Left] ||
                        !PlayerRayCastData[(int) Direction.Forward] || !PlayerRayCastData[(int) Direction.Back])
                    {
                        atJunction = true;
                    }
                    break;
                case Direction.Forward:
                    if (PlayerRayCastData[(int) Direction.Forward] ||
                        !PlayerRayCastData[(int) Direction.Right] || !PlayerRayCastData[(int) Direction.Left])
                    {
                        atJunction = true;
                    }break;
                case Direction.Back:
                    if (PlayerRayCastData[(int) Direction.Back] ||
                        !PlayerRayCastData[(int) Direction.Right] || !PlayerRayCastData[(int) Direction.Left])
                    {
                        atJunction = true;
                    }
                    break;
                case Direction.None:
                    atJunction = true;
                    break;
            }
        }
        
        public void Move()
        {
            float deltaAngle = Speed * Time.deltaTime;
            switch (movementDirection)
            {
                case Direction.Right:
                    PlayerCube.RotateAround(RightAnchor.position, -transform.forward * stepSize, deltaAngle);
                    break;
                case Direction.Left:
                    PlayerCube.RotateAround(LeftAnchor.position, transform.forward * stepSize, deltaAngle);
                    break;
                case Direction.Forward:
                    PlayerCube.RotateAround(ForwardAnchor.position, transform.right * stepSize, deltaAngle);
                    break;
                case Direction.Back:
                    PlayerCube.RotateAround(BackAnchor.position, -transform.right * stepSize, deltaAngle);
                    break;
            }

            tempAngleRotated += deltaAngle;
            angleRotated += deltaAngle;
            
            if (tempAngleRotated >= 90)
            {

                if (!CheckGroundUnderneath())
                {
                    PlayerCube.localEulerAngles = DirectionRotation[(int) movementDirection] * 90;
                    PlayerCube.localPosition = DirectionVector[(int) movementDirection] * stepSize;

                    switch (movementDirection)
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
                    
                    atVerticalDownEdge = true;
                }
                else
                {                   
                    if (atVerticalDownEdge)
                    {
                        transform.position += DirectionVector[(int) Direction.Up] * stepSize;
                        transform.eulerAngles += DirectionRotation[(int) movementDirection] * 90;
                        atVerticalDownEdge = false;
                    }

                    transform.position += DirectionVector[(int) movementDirection] * stepSize;
                    transform.eulerAngles = Vector3.zero;
                    
                    if (snapCount == 2)
                    {
                        movementSnapped_full = true;
                        snapCount = 0;
                    }
                    else
                    {
                        snapCount++;
                    }

                   
                    PlayerCube.localPosition = Vector3.zero;
                    PlayerCube.localEulerAngles = Vector3.zero;

                    movementSnapped_half = true;
                    angleRotated = 0;
                }
                
                tempAngleRotated = 0;
            }
            else
            {
                movementSnapped_half = false;
                movementSnapped_full = false;
            }

            if (movementSnapped_half && !movementSnapped_full)
            {
                if (CheckWallInfront())
                {
                    atVerticalUpEdge = true;
                    snapCount++;
                    
                    transform.eulerAngles -= DirectionRotation[(int) movementDirection] * 90;
                    
                    PlayerCube.localPosition = Vector3.zero;
                    PlayerCube.localEulerAngles = Vector3.zero;
                    
                    switch (movementDirection)
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

        public bool CheckWallInfront()
        {
            Color _color;

            PlayerRayCastData[(int)movementDirection] = Physics.Raycast(PlayerCube.position + DirectionVector[(int)Direction.Down] * 0.01f, DirectionVector[(int)movementDirection], out RaycastHit hit, stepSize + 0.1f * stepSize);
            if (PlayerRayCastData[(int) movementDirection]){_color = new Color(1f, 0.8f, 0.36f);}else{_color = new Color(0.6f, 0.84f, 1f);}
            Debug.DrawRay(PlayerCube.position + DirectionVector[(int)Direction.Down] * 0.01f, -DirectionVector[(int)movementDirection] * (stepSize + 0.1f * stepSize), _color);
            
            return PlayerRayCastData[(int)movementDirection];
        }

        public bool CheckGroundUnderneath()
        {
            Color _color;

            PlayerRayCastData[(int)Direction.Down] = Physics.Raycast(PlayerCube.position + DirectionVector[(int)Direction.Down] * 0.01f, DirectionVector[(int)Direction.Down], out RaycastHit hit, stepSize + 0.1f * stepSize);
            if (PlayerRayCastData[(int) Direction.Down]){_color = Color.red;}else{_color = Color.green;}
            Debug.DrawRay(PlayerCube.position + DirectionVector[(int)Direction.Down] * 0.01f, DirectionVector[(int)Direction.Up] * (stepSize + 0.1f * stepSize), _color);
        
            
            return PlayerRayCastData[(int)Direction.Down];
        }

        public void SetParentMaze(GameObject Maze)
        {
            this.ParentMaze = Maze;
            transform.parent= this.ParentMaze.transform;
        }
    }
}