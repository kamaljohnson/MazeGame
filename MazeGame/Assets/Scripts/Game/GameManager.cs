using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Game.Items.Intractable.Portal;
using Game.Player;
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
        
        public static Transform MazeTransform;
        public static Transform PlayerCubeTransform;
    
        public static GameStates Gamestate;
        
        public void Start()
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }

        public void Update()
        {
            switch (Gamestate)
            {
                case GameStates.Playing:
                    break;
                case GameStates.Paused:
                    break;
                case GameStates.LevelComplete:
                    break;
                case GameStates.Dead:
                    Debug.Log("Dead");
                    if (Portal.CurrentCheckpointDestinationPortal != null)
                    {
                        PlayerCubeTransform.GetComponent<Movement>().Reset();
                        Portal.CurrentCheckpointDestinationPortal.ActivateCheckpoint();
                        Gamestate = GameStates.Playing;
                    }
                    break;
                case GameStates.GameOver:
                    Debug.Log("Game Over");
                    break;
                case GameStates.InMenu:
                    break;
            }
        }
    }
}
