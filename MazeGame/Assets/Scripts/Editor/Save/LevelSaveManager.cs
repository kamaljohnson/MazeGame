using System;
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
                mazeData.l = new List<Game.Items.Intractable.Laser.SerializableItem>();
                mazeData.s = new List<Game.Items.Intractable.Spike.SerializableItem>();
                
                mazeData.b = new List<Game.Items.Activators.Button.SerializableItem>();
                
                mazeData.h = new List<Game.Items.Enemies.Hammer.SerializableItem>();
                mazeData.gr = new List<Game.Items.Enemies.Guardian.SerializableItem>();
                mazeData.bl = new List<Game.Items.Enemies.Blade.SerializableItem>();
                mazeData.k = new List<Game.Items.Enemies.Knight.SerializableItem>();
                
                mazeData.i = new List<Game.Items.Path.Ice.SerializableItem>();
                mazeData.f = new List<Game.Items.Path.Fire.SerializableItem>();
                
                mazeData.co = new List<Game.Items.Collectables.Coin.SerializableItem>();
                mazeData.d = new List<Game.Items.Collectables.Diamond.SerializableItem>();
                
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
                                        var serializedIceData = new Game.Items.Path.Ice.SerializableItem();
                                        serializedIceData.ConvertToSerializable(allItems[itemType][itemIndex].GetComponent<Game.Items.Path.Ice.Ice>());
                                        mazeData.i.Add(serializedIceData);
                                        Debug.Log("Ice");
                                        break;
                                    
                                    case "Fire":
                                        Debug.Log("Fire");
                                        var fire = allItems[itemType][itemIndex].GetComponent<Game.Items.Path.Fire.Fire>();
                                        var serializedFireData = new Game.Items.Path.Fire.SerializableItem();
                                        serializedFireData.ConvertToSerializable(fire);
                                        mazeData.f.Add(serializedFireData);
                                        break;
                                }
                                break;
                            case ItemCategories.Intractable:
                                switch (allItems[itemType][itemIndex].name)
                                {
                                    case "Spike":
                                        var serializedSpikeData = new Game.Items.Intractable.Spike.SerializableItem();
                                        serializedSpikeData.ConvertToSerializable(allItems[itemType][itemIndex].GetComponent<Game.Items.Intractable.Spike.Spike>());
                                        mazeData.s.Add(serializedSpikeData);
                                        Debug.Log("Spike");
                                        break;
                                    case "Portal":
                                        var serializedPortalData = new Game.Items.Intractable.Portal.SerializableItem();
                                        serializedPortalData.ConvertToSerializable(allItems[itemType][itemIndex].GetComponent<Game.Items.Intractable.Portal.Portal>());
                                        mazeData.p.Add(serializedPortalData);
                                        Debug.Log("Portal");
                                        break;
                                    case "Gate":
                                        var serializedGateData = new Game.Items.Intractable.Gate.SerializableItem();
                                        serializedGateData.ConvertToSerializable(allItems[itemType][itemIndex].GetComponent<Game.Items.Intractable.Gate.Gate>());
                                        mazeData.g.Add(serializedGateData);
                                        Debug.Log("Gate");
                                        break;
                                    case "Laser":
                                        var serializedLaserData = new Game.Items.Intractable.Laser.SerializableItem();
                                        serializedLaserData.ConvertToSerializable(allItems[itemType][itemIndex].GetComponent<Game.Items.Intractable.Laser.Laser>());
                                        mazeData.l.Add(serializedLaserData);
                                        Debug.Log("Laser");
                                        break;
                                }
                                break;
                            case ItemCategories.Activator:
                                switch (allItems[itemType][itemIndex].name)
                                {
                                    case "Button":
                                        Debug.Log("Button");
                                        var button = allItems[itemType][itemIndex].GetComponent<Game.Items.Activators.Button.Button>();
                                        var serializedButtonData = new Game.Items.Activators.Button.SerializableItem();
                                        serializedButtonData.ConvertToSerializable(button);
                                        mazeData.b.Add(serializedButtonData);
                                        break;
                                }
                                break;
                            case ItemCategories.Collectable:
                                switch (allItems[itemType][itemIndex].name)
                                {
                                    case "Coin":
                                        Debug.Log("Coin");
                                        var coin = allItems[itemType][itemIndex].GetComponent<Game.Items.Collectables.Coin.Coin>();
                                        var serializableCoinData = new Game.Items.Collectables.Coin.SerializableItem();
                                        serializableCoinData.ConvertToSerializable(coin);
                                        mazeData.co.Add(serializableCoinData);
                                        break;
                                    case "Diamond":
                                        Debug.Log("Diamond");
                                        var diamond = allItems[itemType][itemIndex].GetComponent<Game.Items.Collectables.Diamond.Diamond>();
                                        var serializableDiamondData = new Game.Items.Collectables.Diamond.SerializableItem();
                                        serializableDiamondData.ConvertToSerializable(diamond);
                                        mazeData.d.Add(serializableDiamondData);
                                        break;
                                }
                                break;
                            case ItemCategories.Enemy:
                                switch (allItems[itemType][itemIndex].name)
                                {
                                    case "Hammer":
                                        Debug.Log("Hammer");
                                        var hammer = allItems[itemType][itemIndex].GetComponent<Game.Items.Enemies.Hammer.Hammer>();
                                        var serializedHammerData = new Game.Items.Enemies.Hammer.SerializableItem();
                                        serializedHammerData.ConvertToSerializable(hammer);
                                        mazeData.h.Add(serializedHammerData);
                                        break;
                                    case "Guardian":                
                                        Debug.Log("Guardian");
                                        var guardian = allItems[itemType][itemIndex].GetComponent<Game.Items.Enemies.Guardian.Guardian>();
                                        var serializedGuardianData = new Game.Items.Enemies.Guardian.SerializableItem();
                                        serializedGuardianData.ConvertToSerializable(guardian);
                                        mazeData.gr.Add(serializedGuardianData);
                                        break;
                                    case "Blade":
                                        Debug.Log("Blade");
                                        var blade = allItems[itemType][itemIndex].GetComponent<Game.Items.Enemies.Blade.Blade>();
                                        var serializedBladeData = new Game.Items.Enemies.Blade.SerializableItem();
                                        serializedBladeData.ConvertToSerializable(blade);
                                        mazeData.bl.Add(serializedBladeData);
                                        break;
                                    case "Knight":
                                        Debug.Log("Knight");
                                        var knight = allItems[itemType][itemIndex].GetComponent<Game.Items.Enemies.Knight.Knight>();
                                        var serializableKnightData = new Game.Items.Enemies.Knight.SerializableItem();
                                        serializableKnightData.ConvertToSerializable(knight);
                                        mazeData.k.Add(serializableKnightData);
                                        break;
                                }
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

            var jsonString = JsonUtility.ToJson(state);
            Debug.Log(jsonString);
            File.WriteAllText(directory, jsonString);
        }
    }

    [Serializable]
    public class SaveState
    {
        //list of all the maze cubes along with its list of attached nodes
        public List<MazeData> m;
    }

    [Serializable]
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
        public List<Game.Items.Intractable.Laser.SerializableItem> l;    //the list of all lasers on the maze
        public List<Game.Items.Intractable.Spike.SerializableItem> s;    //the list of all buttons on the maze
        
        public List<Game.Items.Activators.Button.SerializableItem> b;    //the list of all buttons on the maze
        
        public List<Game.Items.Enemies.Hammer.SerializableItem> h;    //the list of all hammers on the maze
        public List<Game.Items.Enemies.Guardian.SerializableItem> gr;    //the list of all guardians on the maze
        public List<Game.Items.Enemies.Blade.SerializableItem> bl;    //the list of all blade on the maze
        public List<Game.Items.Enemies.Knight.SerializableItem> k;    //the list of all knight on the maze
        
        public List<Game.Items.Path.Ice.SerializableItem> i;    //the list of all ice on the maze
        public List<Game.Items.Path.Fire.SerializableItem> f;    //the list of all fire on the maze
        
        public List<Game.Items.Collectables.Coin.SerializableItem> co;    //the list of all fire on the maze
        public List<Game.Items.Collectables.Diamond.SerializableItem> d;    //the list of all fire on the maze
    }

    [Serializable]
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