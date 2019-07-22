using System;
using System.Collections.Generic;
using Game.Items.Path.Ice;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace Game.Items.Enemies.Knight
{
    public class Knight : MonoBehaviour, IItems
    {
        private float _stepSize;

        private Direction _movementDirection;

        private bool _movementSnappedFull;
        public bool _atJunction;
        public bool _canSeePlayer;

        public List<bool> knightWallRayCastData = new List<bool>
        {
            false,    //right raycast data
            false,    //left raycast data
            false,    //forward raycast data
            false,    //back raycast data
            false    //down raycast data
        };
        
        public List<bool> knightPlayerRayCastData = new List<bool>
        {
            false,    //right raycast data
            false,    //left raycast data
            false,    //forward raycast data
            false,    //back raycast data
            false    //down raycast data
        };
        
        public void Start()
        {
            Init();
        }

        public void Update()
        {
            if (_movementSnappedFull)
            {
                CheckJunction();
            }

            if(_atJunction)
            {
                CheckForPlayer();
            }

            if (_canSeePlayer)
            {
                Move();
            }
        }

        private void CheckJunction()
        { 
            //getting the ray-cast data
            var transformPosition = transform.position;

            //right ray-cast data
            knightWallRayCastData[(int) Direction.Right] = Physics.Raycast(transformPosition + Helper.DirectionVector[(int)Direction.Down] * 0.01f, Helper.DirectionVector[(int)Direction.Right], out _, _stepSize + 0.1f * _stepSize);
            var color = knightWallRayCastData[(int) Direction.Right] ? Color.red : Color.green;
            Debug.DrawRay(transformPosition + Helper.DirectionVector[(int)Direction.Down] * 0.01f, transform.right * (_stepSize + 0.1f * _stepSize), color);

            //left ray-cast data
            knightWallRayCastData[(int) Direction.Left] = Physics.Raycast(transformPosition + Helper.DirectionVector[(int)Direction.Down] * 0.01f, Helper.DirectionVector[(int)Direction.Left], out _, _stepSize + 0.1f * _stepSize);
            color = knightWallRayCastData[(int) Direction.Left] ? Color.red : Color.green;
            Debug.DrawRay(transformPosition + Helper.DirectionVector[(int)Direction.Down] * 0.01f, -transform.right * (_stepSize + 0.1f * _stepSize), color);
            
            //forward ray-cast data
            knightWallRayCastData[(int) Direction.Forward] = Physics.Raycast(transformPosition + Helper.DirectionVector[(int)Direction.Down] * 0.01f, Helper.DirectionVector[(int)Direction.Forward], out _, _stepSize + 0.1f * _stepSize);
            color = knightWallRayCastData[(int) Direction.Forward] ? Color.red : Color.green;
            Debug.DrawRay(transformPosition + Helper.DirectionVector[(int)Direction.Down] * 0.01f, transform.forward * (_stepSize + 0.1f * _stepSize), color);
            
            //back ray-cast data
            knightWallRayCastData[(int) Direction.Back] = Physics.Raycast(transformPosition + Helper.DirectionVector[(int)Direction.Down] * 0.01f, Helper.DirectionVector[(int)Direction.Back], out _, _stepSize + 0.1f * _stepSize);
            color = knightWallRayCastData[(int) Direction.Back] ? Color.red : Color.green;
            Debug.DrawRay(transformPosition + Helper.DirectionVector[(int)Direction.Down] * 0.01f, -transform.forward * (_stepSize + 0.1f * _stepSize) , color);
            
            
            //checking if the there is a new path in the perpendicular direction of travel
            if (!Ice.onIce)
            {
                _atJunction = true;
            }
            else
            {
                switch (_movementDirection)
                {
                    case Direction.Right:
                        if (knightWallRayCastData[(int)Direction.Right])
                        {
                            _atJunction = true;                        
                        }
                        if (!knightWallRayCastData[(int) Direction.Forward] || !knightWallRayCastData[(int) Direction.Back])
                        {
                            if (!Ice.onIce)
                            {
                                _atJunction = true;
                            }
                        }
                        break;
                    case Direction.Left:
                        if (knightWallRayCastData[(int)Direction.Left])
                        {
                            _atJunction = true;                        
                        }
                        if (!knightWallRayCastData[(int) Direction.Forward] || !knightWallRayCastData[(int) Direction.Back])
                        {
                            if (!Ice.onIce)
                            {
                                _atJunction = true;
                            }
                        }
                        break;
                    case Direction.Forward:
                        if (knightWallRayCastData[(int)Direction.Forward])
                        {
                            _atJunction = true;                        
                        }
                        if (!knightWallRayCastData[(int) Direction.Right] || !knightWallRayCastData[(int) Direction.Left])
                        {
                            if (!Ice.onIce)
                            {
                                _atJunction = true;
                            }
                        }
                        break;
                    case Direction.Back:
                        if (knightWallRayCastData[(int)Direction.Back])
                        {
                            _atJunction = true;                        
                        }
                        if (!knightWallRayCastData[(int) Direction.Right] || !knightWallRayCastData[(int) Direction.Left])
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

        private void CheckForPlayer()
        {
                        //getting the ray-cast data
            var transformPosition = transform.position;

            Color hitColor = new Color(0.54f, 0f, 1f);
            Color notHitColor = new Color(0f, 1f, 0.87f);
            
            //right ray-cast data
            knightPlayerRayCastData[(int) Direction.Right] = Physics.Raycast(transformPosition + Helper.DirectionVector[(int)Direction.Down] * 0.01f, Helper.DirectionVector[(int)Direction.Right], out _, 20);
            var color = knightWallRayCastData[(int) Direction.Right] ? hitColor : notHitColor;
            Debug.DrawRay(transformPosition + Helper.DirectionVector[(int)Direction.Down] * 0.01f, transform.right * 20, color);

            //left ray-cast data
            knightPlayerRayCastData[(int) Direction.Left] = Physics.Raycast(transformPosition + Helper.DirectionVector[(int)Direction.Down] * 0.01f, Helper.DirectionVector[(int)Direction.Left], out _, 20);
            color = knightWallRayCastData[(int) Direction.Left] ? hitColor : notHitColor;
            Debug.DrawRay(transformPosition + Helper.DirectionVector[(int)Direction.Down] * 0.01f, -transform.right * 20, color);
            
            //forward ray-cast data
            knightPlayerRayCastData[(int) Direction.Forward] = Physics.Raycast(transformPosition + Helper.DirectionVector[(int)Direction.Down] * 0.01f, Helper.DirectionVector[(int)Direction.Forward], out _, 20);
            color = knightWallRayCastData[(int) Direction.Forward] ? hitColor : notHitColor;
            Debug.DrawRay(transformPosition + Helper.DirectionVector[(int)Direction.Down] * 0.01f, transform.forward * 20, color);
            
            //back ray-cast data
            knightPlayerRayCastData[(int) Direction.Back] = Physics.Raycast(transformPosition + Helper.DirectionVector[(int)Direction.Down] * 0.01f, Helper.DirectionVector[(int)Direction.Back], out _, 20);
            color = knightWallRayCastData[(int) Direction.Back] ? hitColor : notHitColor;
            Debug.DrawRay(transformPosition + Helper.DirectionVector[(int)Direction.Down] * 0.01f, -transform.forward * 20, color);

            var flag = false;
            for (int directionIndex = 0; directionIndex < 4; directionIndex++)
            {
                if (knightPlayerRayCastData[directionIndex])
                {
                    _movementDirection = (Direction) directionIndex;
                    flag = true;
                    break;
                }
            }

            if (flag)
            {
                _canSeePlayer = true;
            }
            else
            {
                _canSeePlayer = false;
                _movementDirection = Direction.None;
            }
           
        }

        public void Move()
        {
            
        }

        private void Init()
        {
            _stepSize = transform.lossyScale.x;
            _movementDirection = Direction.None;
            _canSeePlayer = false;
            _atJunction = true;
            _movementSnappedFull = true;
        }

        public ItemCategories GetItemType()
        {
            return ItemCategories.Enemie;
        }
    }
    
    [Serializable]
    public class SerializableItem
    {
        public int x;
        public int y;
        public int z;

        public int u;
        public int v;
        public int w;

        public void ConvertToSerializable(Knight knight)
        {
            var transform = knight.transform;
            var position = transform.position;
            x = (int)position.x;
            y = (int)position.y;
            z = (int)position.z;

            var eulerAngles = transform.eulerAngles;
            u = (int) eulerAngles.x;
            v = (int) eulerAngles.y;
            w = (int) eulerAngles.z;
        }
    }
}
