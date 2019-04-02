using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static Transform CurrentMazeTransform;
    public static Transform PlayerCubeTransform;

    public void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
    }
}
