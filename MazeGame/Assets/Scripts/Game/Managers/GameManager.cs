#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif
using Game.Items.Intractable.Portal;
using Game.Player;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Managers
{
    public class GameManager : MonoBehaviour
    {
        public enum GameStates
        {
            Playing,
            InGameUi,
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

        public SceneAsset templateScene;
        
        public void Awake()
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageRead);
            }
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            }
            stateManager = new LevelStateManager();
            Screen.orientation = ScreenOrientation.Portrait;
            levelName = SceneManager.GetActiveScene().name;
            
            DontDestroyOnLoad(this);
            SceneManager.LoadScene(templateScene.name);
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
                    stateManager.SaveLevelState();
                    gameState = GameStates.InMenu;
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
