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
            state.m = new List<Maze_data>();

            for (int i = 0; i < LevelEditor.Mazes.childCount; i++)
            {
                LevelEditor.CurrentMaze = LevelEditor.Mazes.GetChild(i).gameObject;
                LevelEditor.ReCalculateAllMazeCubes();
                LevelEditor.ReCalculateNodes();
                LevelEditor.RenderPaths();

                List<GameObject> surfaceMazeCubes = LevelEditor.GetSurfaceMazeCubes();
                Maze_data maze_data = new Maze_data();
                maze_data.c = new List<MazeCube_data>();

                for (int j = 0; j < surfaceMazeCubes.Count; j++)
                {
                    MazeCube_data mcn = new MazeCube_data();
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
        public List<Maze_data> m;
    }

    [System.Serializable]
    public class Maze_data
    {
        public int x;
        public int y;
        public int z;

        public List<MazeCube_data> c;
    }

    [System.Serializable]
    public class MazeCube_data
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
}