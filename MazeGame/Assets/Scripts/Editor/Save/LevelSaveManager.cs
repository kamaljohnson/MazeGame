﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Game.Items;
using UnityEngine.UI;

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

                var surfaceMazeCubes = LevelEditor.GetSurfaceMazeCubes();
                var mazeData = new MazeData();
                var position = LevelEditor.CurrentMaze.transform.position;
                mazeData.x = (int)position.x;
                mazeData.y = (int)position.y;
                mazeData.z = (int)position.z;
                
                mazeData.c = new List<MazeCubeData>();
                mazeData.p = new List<Game.Items.Intractable.Portal.SerializableItem>();
                mazeData.g = new List<Game.Items.Intractable.Gate.SerializableItem>();
                mazeData.s = new List<Game.Items.Intractable.Spike.SerializableItem>();
                mazeData.b = new List<Game.Items.Activators.Button.SerializableItem>();
                
                mazeData.i = new List<Game.Items.Path.Ice.SerializableItem>();
                
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
                                switch (allItems[itemType][itemIndex].name)
                                {
                                    case "Ice":
                                        Debug.Log("Ice");
                                        var serializedIceData = new Game.Items.Path.Ice.SerializableItem();
                                        serializedIceData.ConvertToSerializable(allItems[itemType][itemIndex].GetComponent<Game.Items.Path.Ice.Ice>());
                                        mazeData.i.Add(serializedIceData);
                                        break;
                                }
                                break;
                            case ItemCategories.Intractable:
                                switch (allItems[itemType][itemIndex].name)
                                {
                                    case "Spike":
                                        Debug.Log("Spike");
                                        var serializedSpikeData = new Game.Items.Intractable.Spike.SerializableItem();
                                        serializedSpikeData.ConvertToSerializable(allItems[itemType][itemIndex].GetComponent<Game.Items.Intractable.Spike.Spike>());
                                        mazeData.s.Add(serializedSpikeData);
                                        break;
                                    case "Portal":
                                        Debug.Log("Portal");
                                        var serializedPortalData = new Game.Items.Intractable.Portal.SerializableItem();
                                        serializedPortalData.ConvertToSerializable(allItems[itemType][itemIndex].GetComponent<Game.Items.Intractable.Portal.Portal>());
                                        mazeData.p.Add(serializedPortalData);
                                        break;
                                    case "Gate":
                                        Debug.Log("Gate");
                                        var serializedGateData = new Game.Items.Intractable.Gate.SerializableItem();
                                        serializedGateData.ConvertToSerializable(allItems[itemType][itemIndex].GetComponent<Game.Items.Intractable.Gate.Gate>());
                                        mazeData.g.Add(serializedGateData);
                                        break;
                                }
                                break;
                            case ItemCategories.Activator:
                                switch (allItems[itemType][itemIndex].name)
                                {
                                    case "Button":
                                        Debug.Log("Button");
                                        var button = allItems[itemType][itemIndex].GetComponent<Game.Items.Activators.Button.Button>();
                                        var serializedData = new Game.Items.Activators.Button.SerializableItem();
                                        serializedData.ConvertToSerializable(button);
                                        mazeData.b.Add(serializedData);
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
        public List<Game.Items.Intractable.Portal.SerializableItem> p;    //the list of all portals on the maze
        public List<Game.Items.Intractable.Gate.SerializableItem> g;    //the list of all gates on the maze
        public List<Game.Items.Intractable.Spike.SerializableItem> s;    //the list of all buttons on the maze
        public List<Game.Items.Activators.Button.SerializableItem> b;    //the list of all buttons on the maze
        
        public List<Game.Items.Path.Ice.SerializableItem> i;    //the list of all ice on the maze
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

        public void ConvertToSavable(GameObject maceCube)
        {
            x = (int)maceCube.transform.position.x;
            y = (int)maceCube.transform.position.y;
            z = (int)maceCube.transform.position.z;

            nl = new List<Game.Maze.SavableNode>();
            for (int j = 0; j < maceCube.transform.childCount; j++)
            {
                Game.Maze.SavableNode sn = new Game.Maze.SavableNode();
                if (maceCube.transform.GetChild(j).GetComponent<Game.Maze.Node>() != null)
                {
                    sn.ConvertToSavable(maceCube.transform.GetChild(j).GetComponent<Game.Maze.Node>());
                    nl.Add(sn);
                }
            }
        }
    }
}