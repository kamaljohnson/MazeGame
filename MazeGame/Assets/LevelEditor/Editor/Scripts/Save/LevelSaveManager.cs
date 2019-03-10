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
            string Directory = Application.streamingAssetsPath + "/Levels/" + "level " + SceneManager.GetActiveScene().name.Split(' ')[1];
            state = new SaveState();

            //getting the data of the current maze
            state.m = new List<MazeData>();

            for (int i = 0; i < LevelEditor.Mazes.childCount; i++)
            {
                LevelEditor.CurrentMaze = LevelEditor.Mazes.GetChild(i).gameObject;
                LevelEditor.ReCalculateAllMazeCubes();
                LevelEditor.ReCalculateNodes();
                LevelEditor.RenderPaths();

                List<GameObject> surfaceMazeCubes = LevelEditor.GetSurfaceMazeCubes();
                MazeData maze_data = new MazeData();
                maze_data.c = new List<MazeCubeData>();

                List<List<GameObject>> allItems = LevelEditor.GetMazeItems();
                
                //adding all the item data
                for(int _itemType = 0; _itemType < allItems.Count; _itemType++)
                {
                    //TODO call convertToSerializable() for each item
                }

                //adding all the node data 
                for (int j = 0; j < surfaceMazeCubes.Count; j++)
                {
                    MazeCubeData mcn = new MazeCubeData();
                    mcn.ConvertToSavable(surfaceMazeCubes[j]);
                    maze_data.c.Add(mcn);
                }
                state.m.Add(maze_data);
            }

            Debug.Log("SAVE REPORT");
            for (int i = 0; i < state.m.Count; i++)
            {
                Debug.Log("MAZE " + (i + 1) + " : " + state.m[i].c.Count + " CUBES");
            }

            string jsonString;
            jsonString = JsonUtility.ToJson(state);
            Debug.Log(jsonString);
            File.WriteAllText(Directory, jsonString);
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
        public List<Game.Items.Portal.SerializablePortalData> p;    //the list of all portals on the maze
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

                sn.ConvertToSavable(mace_cube.transform.GetChild(j).GetComponent<Game.Maze.Node>());

                nl.Add(sn);
            }
        }
    }

    //TODO: create save feature for items
    //itemType:itemIndex:serializedData
}