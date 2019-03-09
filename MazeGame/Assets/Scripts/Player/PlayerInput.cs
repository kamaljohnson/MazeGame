using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public class PlayerInput : MonoBehaviour
    {
        
        public List<bool> inputs = new List<bool>()
        {
            false,    //right
            false,    //left
            false,    //forward
            false     //back
        };
    
        public Direction GetInputDirection()
        {
            if (Input.GetKey(KeyCode.D))
            {
                return Direction.Right;
            }
            if (Input.GetKey(KeyCode.A))
            {
                return Direction.Left;
            }
            if (Input.GetKey(KeyCode.W))
            {
                return Direction.Forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                return Direction.Back;
            }

            return Direction.None;
        }
    }
}
