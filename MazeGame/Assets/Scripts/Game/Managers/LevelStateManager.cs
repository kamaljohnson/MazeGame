using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game.Managers
{
    [Serializable]
    public class LevelStateManager
    {
        public bool isLocked;
        public int stars;
        public List<int> indexOfCoinsCollected = new List<int>();
        public List<int> indexOfDiamondsCollected = new List<int>();
        
        public void LoadLevelState()
        {
            string directory = Application.persistentDataPath + "/" + GameManager.levelName + ".txt";
        
            //file not found making one instead
            if (!File.Exists(directory))
            {
                SaveLevelState();
            }
            else //loading the data from the existing file
            {
                string jsonString = "";        
                if(Application.platform == RuntimePlatform.Android)
                {
                    Debug.Log("loading old state");
                    WWW reader = new WWW(directory);
                    while (!reader.isDone) { }

                    jsonString = reader.text;
                }
                else
                {
                    jsonString = File.ReadAllText(directory);
                }   
                
                var ls = JsonUtility.FromJson<LevelStateManager>(jsonString);
        
                isLocked = ls.isLocked;
                stars = ls.stars;
                indexOfCoinsCollected = ls.indexOfCoinsCollected;
                indexOfDiamondsCollected = ls.indexOfDiamondsCollected;
            }
        }
        
        public void SaveLevelState()
        {
            string directory;
    
            directory = Application.persistentDataPath + "/" + GameManager.levelName + ".txt";

            var jsonString = JsonUtility.ToJson(this);

            var fullPath = directory;
            if (!File.Exists(fullPath))
            {
                LevelStateManager state = new LevelStateManager
                {
                    isLocked = false,
                    indexOfCoinsCollected = new List<int>(),
                    indexOfDiamondsCollected = new List<int>(),
                    stars = 0
                };
            
                jsonString = JsonUtility.ToJson(state);
                File.WriteAllText(fullPath, jsonString);
                Debug.Log("writen : " + jsonString + " to file : " + directory);
            }
            else
            {
                Debug.Log("file exists in " + fullPath);
            
                File.WriteAllText(fullPath, jsonString);
            }
        }
    }
}