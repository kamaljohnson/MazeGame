using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Game.Items.Intractable.Portal;
using Game.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Managers
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
        
        public static Transform mazeTransform;
        public static Transform playerCubeTransform;
        public static LevelStateManager stateManager;
    
        public static GameStates gameState;

        public static string levelName;
        
        public void Start()
        {
            Screen.orientation = ScreenOrientation.Portrait;
            levelName = SceneManager.GetActiveScene().name;
        }

        public void Update()
        {
            switch (gameState)
            {
                case GameStates.Playing:
                    break;
                case GameStates.Paused:
                    break;
                case GameStates.LevelComplete:
                    stateManager.Save();
                    break;
                case GameStates.Dead:
                    Debug.Log("Dead");
                    if (Portal.CurrentCheckpointDestinationPortal != null)
                    {
                        playerCubeTransform.GetComponent<Movement>().Reset();
                        Portal.CurrentCheckpointDestinationPortal.ActivateCheckpoint();
                        gameState = GameStates.Playing;
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
