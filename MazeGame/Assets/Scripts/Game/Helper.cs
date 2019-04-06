using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game
{
    public static class Helper
    {
        public static readonly List<Vector3> DirectionRotation = new List<Vector3>
        {
            new Vector3(1, 0, 0),     //forward axis rotation
            new Vector3(0, 0, 1),     //left axis rotation
            new Vector3(-1, 0, 0),    //back axis rotation
            new Vector3(0, 0, -1),    //right axis rotation
        };
        public static readonly List<Vector3> DirectionVector = new List<Vector3>
        {
            new Vector3(0, 0, 1),        //forward direction vector
            new Vector3(-1, 0, 0),       //left direction vector
            new Vector3(0, 0, -1),       //back direction vector
            new Vector3(1, 0, 0),        //right direction vector
            new Vector3(0, -1, 0),       //down direction vector
            new Vector3(0, 1, 0)        //up direction vector
        };
    }
    
    public enum Direction
    {
        Forward,
        Left,
        Back,
        Right,
        Down,
        Up,
        None
    };
}