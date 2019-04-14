using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

namespace LevelEditor.Save
{
    public class LevelSaveManager : Editor
    {
        public SaveState state;
        public void Save()
        {
            var directory = Application.streamingAssetsPath + "/Levels/" + "level " + SceneManager.GetActiveScene().name.Split(' ')[1];
            state = new SaveState();

            //getting the data of the current maze
            state.m = new List<MazeData>();

            for (var i = 0; i < LevelEditor.Mazes.childCount; i++)
            {
                LevelEditor.CurrentMaze = LevelEditor.Mazes.GetChild(i).gameObject;
                /*LevelEditor.ReCalculateAllMazeCubes();
                LevelEditor.ReCalculateNodes();
                LevelEditor.RenderPaths();*/

                Debug.Log(LevelEditor.AllMazeCubes.Count.ToString());

                var surfaceMazeCubes = LevelEditor.GetSurfaceMazeCubes();
                var mazeData = new MazeData();
                var position = LevelEditor.CurrentMaze.transform.position;
                mazeData.x = (int)position.x;
                mazeData.y = (int)position.y;
                mazeData.z = (int)position.z;
                
                mazeData.c = new List<MazeCubeData>();
                mazeData.p = new List<Game.Items.Interactable.Portal.SerializableItem>();
                
                List<List<GameObject>> allItems;
                LevelEditor.GetAllMazeItems(out allItems);
                
                //adding all the item data
                for(var itemType = 0; itemType < allItems.Count; itemType++)
                {
                    for (var itemIndex = 0; itemIndex < allItems[itemType].Count; itemIndex++)
                    {
                        //TODO call ConvertToSerializable() for each item
                        switch ((ItemCategories)itemType)
                        {
                            case ItemCategories.Path:
                                break;
                            case ItemCategories.Interactable:
                                switch (allItems[itemType][itemIndex].name)
                                {
                                    case "Portal":
                                        var serializedData = new Game.Items.Interactable.Portal.SerializableItem();
                                        serializedData.ConvertToSerializable(allItems[itemType][itemIndex].GetComponent<Game.Items.Interactable.Portal.Portal>());
                                        mazeData.p.Add(serializedData);
                                        break;
                                }
                                break;
                            case ItemCategories.Collectable:
                                break;
                            case ItemCategories.Enemie:
                                break;
                            case ItemCategories.Decoratable:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }

                //adding all the node data 
                for (int j = 0; j < surfaceMazeCubes.Count; j++)
                {
                    MazeCubeData mcn = new MazeCubeData();
                    mcn.ConvertToSavable(surfaceMazeCubes[j]);
                    mazeData.c.Add(mcn);
                }
                state.m.Add(mazeData);
            }

            Debug.Log("SAVE REPORT");
            for (int i = 0; i < state.m.Count; i++)
            {
                Debug.Log("MAZE " + (i + 1) + " : " + state.m[i].c.Count + " CUBES");
            }

            string jsonString;
            jsonString = JsonUtility.ToJson(state);
            Debug.Log(jsonString);
            File.WriteAllText(directory, jsonString);
        }
    }

    [System.Serializable]
    public class SaveState
    {
        //list of all the maze cubes along with its list of attached nodes
        public List<MazeData> m;
    }

    [System.Serializable]
    public class MazeData
    {
        //maze cube data
        public int x;
        public int y;
        public int z;
        public List<MazeCubeData> c;

        //item data
        public List<Game.Items.Interactable.Portal.SerializableItem> p;    //the list of all portals on the maze
    }

    [System.Serializable]
    public class MazeCubeData
    {
        //maze cube transform
        public int x;
        public int y;
        public int z;

        //the list of nodes the cube has
        public List<Game.Maze.SavableNode> nl;

        public void ConvertToSavable(GameObject mace_cube)
        {
            x = (int)mace_cube.transform.position.x;
            y = (int)mace_cube.transform.position.y;
            z = (int)mace_cube.transform.position.z;

            nl = new List<Game.Maze.SavableNode>();
            for (int j = 0; j < mace_cube.transform.childCount; j++)
            {
                Game.Maze.SavableNode sn = new Game.Maze.SavableNode();
                if (mace_cube.transform.GetChild(j).GetComponent<Game.Maze.Node>() != null)
                {
                    sn.ConvertToSavable(mace_cube.transform.GetChild(j).GetComponent<Game.Maze.Node>());
                    nl.Add(sn);
                }
            }
        }
    }
}