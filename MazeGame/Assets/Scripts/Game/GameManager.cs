using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        public enum GameStates
        {
            Playing,
            Paused,
            LevelComplete,
            Dead,
            GameOver,
            InMenu
        }
        
        public static Transform CurrentMazeTransform;
        public static Transform PlayerCubeTransform;
    
        public static GameStates Gamestate;
        
        public void Start()
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }
    }
}
