using System;
using System.Collections.Generic;
using Game.Items;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace LevelEditor
{
    public enum Modes
    {
        MazeBody,      //maze body editing mode
        MazeLayout,    //maze structure editing mode
        Items,          //add and delete items on the maze
        MazePos,       //can set the position of the maze
        MazePivot,     //set the pivot of the maze
    }

    public enum ItemCategories
    {
        Path,           //[ICE, FIRE]
        Intractable,    //[PORTAL, LASER, SPIKES, GATES]
        Activator,      //[BUTTON]
        Collectable,    //[COIN, DIAMOND, COLLECTION POINT]
        Enemie,         //[GUARDIAN, KNIGHT, HAMMER]
        Decoratable,    //[PLANT_01, FOUNTAIN, ...]
    }

    public class LevelEditor : EditorWindow
    {

        public static Transform Mazes;
        public static GameObject CurrentMaze;

        //path of the prefabs
        public string mazeCubesFilePath;
        public string mazeWallsFilePath;
        public string itemFilePath;

        //reference of corresponding prefabs
        private static List<Object> _typesOfMazeCubes;
        private static List<Object> _typesOfMazeWalls;
        private static List<List<Object>> _typesOfItems;
        private int _totalNumberOfMazeCubeTypes;
        private int _totalNumberOfMazeWallTypes;
        public static Object CurrentMazeCubePrefab;
        public static Object CurrentMazeWallPrefab;
        public static Object CurrentItemPrefab;
        public static ItemCategories CurrentItemType;

        //actual list of gameobject in the scene
        public static List<GameObject> AllMazeCubes = new List<GameObject>();
        public static List<GameObject> AllMazeWalls = new List<GameObject>();
        public static List<List<GameObject>> AllItems = new List<List<GameObject>>();

        #region local variables
        private Vector2 _prefabScrollViewValue = Vector2.zero;
        private ItemCategories _currentItemCatgoryToggled;
        #endregion

        public static Modes EditorMode;
        public static bool InactiveNodeEditing;

        [MenuItem("Window/LevelEditor")]
        public static void ShowWindow()
        {
            LevelEditor window = GetWindow<LevelEditor>();

            window.title = "Level Editor";
        }

        private void OnEnable()
        {
            _totalNumberOfMazeCubeTypes = 0;
            mazeCubesFilePath = "Assets/Prefabs/MazeCubes/MazeCube 1";
            mazeWallsFilePath = "Assets/Prefabs/MazeWalls/MazeWall 1";
            itemFilePath = "Assets/Prefabs/Items/";
            AddMazePrefabs();
            AddItemPrefabs();
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            var style = new GUIStyle();
            style.fontSize = 20;
            GUILayout.Label("LEVEL EDITOR", style);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);


            if (EditorMode == Modes.Items)
            {
                _prefabScrollViewValue = GUILayout.BeginScrollView(_prefabScrollViewValue, GUI.skin.scrollView);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                var _style = new GUIStyle();
                _style.fontSize = 15;
                GUILayout.Label("ITEMS", _style);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                //display all the items available
                GUILayout.BeginVertical();
                for (int typeIndex = 0; typeIndex < _typesOfItems.Count; typeIndex++)
                {
                    GUILayout.BeginVertical();
                    GUILayout.Label(Enum.GetName(typeof(ItemCategories), (ItemCategories)typeIndex));
                    for (int itemIndex = 0; itemIndex < _typesOfItems[typeIndex].Count;)
                    {
                        int _horizontalStackingLimit = 6;
                        GUILayout.BeginHorizontal();
                        for (int k = 0; k < _horizontalStackingLimit; k++)
                        {
                            DrawCustomItemButtons(typeIndex, itemIndex);
                            itemIndex++;
                            if (itemIndex == _typesOfItems[typeIndex].Count && k < _horizontalStackingLimit)
                            {
                                GUILayout.FlexibleSpace();
                                break;
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }
                if (GUILayout.Button("Refresh", GUILayout.Height(30)))
                {
                    AddItemPrefabs();
                }
                GUILayout.EndVertical();
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                //display all the items in the scene
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("ADDED ITEMS", _style);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.BeginVertical();
                
                for (int itemType = 0; itemType < Enum.GetNames(typeof(ItemCategories)).Length; itemType++)
                {
                    GUIStyle toggleButtonStyleNormal = "Button";
                    var toggleButtonStyleToggled = new GUIStyle(toggleButtonStyleNormal);
                    toggleButtonStyleToggled.normal.background = toggleButtonStyleToggled.active.background;

                    bool isToggled = itemType == (int)_currentItemCatgoryToggled;
                    GUILayout.BeginVertical();
                    if (GUILayout.Button(Enum.GetName(typeof(ItemCategories), (ItemCategories)itemType), isToggled ? toggleButtonStyleToggled : toggleButtonStyleNormal, GUILayout.Height(25)))
                    {
                        _currentItemCatgoryToggled = (ItemCategories)itemType;
                    }
                    if (itemType == (int)_currentItemCatgoryToggled)
                    {
                        for (int itemIndex = 0; itemIndex < AllItems[itemType].Count; itemIndex++)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label((itemIndex + 1).ToString() + " " + AllItems[itemType][itemIndex].tag);
                            if (GUILayout.Button("Edit", GUILayout.Height(20)))
                            {
                                Selection.SetActiveObjectWithContext(AllItems[itemType][itemIndex], AllItems[itemType][itemIndex]);
                                SceneView.lastActiveSceneView.FrameSelected();
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();
                //display all the items available
                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Maze Cubes");
                GUILayout.Label("Maze Walls");
                GUILayout.EndHorizontal();

                _prefabScrollViewValue = GUILayout.BeginScrollView(_prefabScrollViewValue, GUI.skin.scrollView);
                if (CurrentMazeCubePrefab == null && _totalNumberOfMazeCubeTypes > 0)
                {
                    CurrentMazeCubePrefab = _typesOfMazeCubes[0];
                }
                int largestCollection = Mathf.Max(_totalNumberOfMazeCubeTypes, _totalNumberOfMazeWallTypes);
                for (int i = 0; i < largestCollection; i++)
                {
                    GUILayout.BeginHorizontal();
                    if (i < _totalNumberOfMazeCubeTypes)
                    {
                        DrawCustomMazeButtons(i);
                    }
                    else
                    {
                        GUILayout.FlexibleSpace();
                    }

                    if (i < _totalNumberOfMazeWallTypes)
                    {
                        DrawCustomMazeWallButtons(i);
                    }
                    else
                    {
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Refresh", GUILayout.Height(40)))
                {
                    AddMazePrefabs();
                }
                GUILayout.EndScrollView();
            }
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            int _size = 65;
            if (Mazes == null)
            {
                if (GUILayout.Button("Create Maze Holder", GUILayout.Height(_size)))
                {
                    SetMazeHolder();
                }
            }
            else if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Maze.Maze_e>() == null)
            {
                if (GUILayout.Button("Initialize Maze", GUILayout.Height(_size)))
                {
                    SetMazeParent();
                }
            }
            else
            {

                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                if (CurrentMaze != null)
                {
                    Modes tempMode = (Modes) EditorGUILayout.EnumPopup("", EditorMode);
                    if (tempMode != EditorMode)
                    {
                        EditorMode = tempMode;
                        if (EditorMode != Modes.Items)
                        {
                            HideAllMazeItems();
                        }
                        else
                        {
                            ShowAllMazeItems();
                        }
                        //TODO: re-store all the items after re-calculating stuff
                        ReCalculateAllMazeCubes();
                    }

                    switch (EditorMode)
                    {
                        case Modes.MazeBody:
                            break;
                        case Modes.MazeLayout:
                            InactiveNodeEditing = GUILayout.Toggle(InactiveNodeEditing, "set inactive nodes");

                            if (GUILayout.Button("reset paths"))
                            {
                                if (!EditorUtility.DisplayDialog("Warning!!",
                                    "Resetting the maze layout will delete the current progress completely", "Cancel",
                                    "Reset"))
                                {
                                    ResetPaths();
                                    ReCalculateAllMazeCubes();
                                    ReCalculateNodes();
                                }
                            }
                            break;
                        case Modes.Items:
                            ReCalculateAllItems();
                            break;
                    }
                }

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Save", GUILayout.Height(30)))
                {
                    Save.LevelSaveManager sm = CreateInstance<Save.LevelSaveManager>();
                    sm.Save();
                }

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Render", GUILayout.Height(30)))
                {
                    Maze.MazeEditorScript.EndNode = null;
                    Maze.MazeEditorScript.StartNode = null;
                    ReCalculateAllMazeCubes();
                    ReCalculateNodes();
                    RenderPaths();
                }

                GUILayout.EndHorizontal();

            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        
        public void DrawCustomMazeButtons(int index)
        {
            bool selected = CurrentMazeCubePrefab == _typesOfMazeCubes[index];

            Texture2D previewImage = AssetPreview.GetAssetPreview((GameObject)_typesOfMazeCubes[index]);

            GUIContent buttonContent = new GUIContent(previewImage);

            bool isToggleDown = GUILayout.Toggle(selected, buttonContent, GUI.skin.button, GUILayout.Height(125), GUILayout.Width(125));
            if (isToggleDown)
            {
                CurrentMazeCubePrefab = _typesOfMazeCubes[index];
            }
        }

        public void DrawCustomMazeWallButtons(int index)
        {
            var selected = CurrentMazeWallPrefab == _typesOfMazeWalls[index];
            var previewImage = AssetPreview.GetAssetPreview((GameObject)_typesOfMazeWalls[index]);
            var buttonContent = new GUIContent(previewImage);
            var isToggleDown = GUILayout.Toggle(selected, buttonContent, GUI.skin.button, GUILayout.Height(125), GUILayout.Width(125));
            
            if (isToggleDown)
            {
                CurrentMazeWallPrefab = _typesOfMazeWalls[index];
            }
        }

        public void DrawCustomItemButtons(int typeIndex, int itemIndex)
        {
            var selected = CurrentItemPrefab == _typesOfItems[typeIndex][itemIndex];
            var previewImage = AssetPreview.GetAssetPreview((GameObject)_typesOfItems[typeIndex][itemIndex]);
            var buttonContent = new GUIContent(previewImage);

            var isToggleDown = GUILayout.Toggle(selected, buttonContent, GUI.skin.button, GUILayout.Height(35), GUILayout.Width(35));
            if (isToggleDown)
            {
                CurrentItemPrefab = _typesOfItems[typeIndex][itemIndex];
                CurrentItemType = (ItemCategories)typeIndex;
            }
        }

        public void AddMazePrefabs()
        {
            _typesOfMazeCubes = new List<Object>();
            _typesOfMazeWalls = new List<Object>();

            _totalNumberOfMazeCubeTypes = 0;
            _totalNumberOfMazeWallTypes = 0;

            var i = int.Parse(mazeCubesFilePath.Split(' ')[1]);

            var n = 30;
            do
            {
                if (AssetDatabase.LoadAssetAtPath(mazeCubesFilePath.Split(' ')[0] + " " + i.ToString() + ".prefab", typeof(Object)))
                {
                    _typesOfMazeCubes.Add(AssetDatabase.LoadAssetAtPath(mazeCubesFilePath.Split(' ')[0] + " " + i.ToString() + ".prefab", typeof(Object)));
                    _totalNumberOfMazeCubeTypes++;
                }
                if (AssetDatabase.LoadAssetAtPath(mazeWallsFilePath.Split(' ')[0] + " " + i.ToString() + ".prefab", typeof(Object)))
                {
                    _typesOfMazeWalls.Add(AssetDatabase.LoadAssetAtPath(mazeWallsFilePath.Split(' ')[0] + " " + i.ToString() + ".prefab", typeof(Object)));
                    _totalNumberOfMazeWallTypes++;
                }
                i++;
                n--;
            } while (n > 0);

            CurrentMazeCubePrefab = _typesOfMazeCubes[0];
            CurrentMazeWallPrefab = _typesOfMazeWalls[0];
        }

        public void AddItemPrefabs()
        {
            _typesOfItems = new List<List<Object>> {
                new List<Object>(), //Path items
                new List<Object>(), //Intractable items
                new List<Object>(), //Activator items
                new List<Object>(), //Collectable items
                new List<Object>(), //Enemie items
                new List<Object>(), //Decoratable items
            };
            AllItems = new List<List<GameObject>>
            {
                new List<GameObject>(), //Path items
                new List<GameObject>(), //Intractable items
                new List<GameObject>(), //Activator items
                new List<GameObject>(), //Collectable items
                new List<GameObject>(), //Enemie items
                new List<GameObject>(), //Decoratable items
            };

            for (var itemIndex = 0; itemIndex < Enum.GetNames(typeof(ItemCategories)).Length; itemIndex++)
            {
                int n = 0;
                do
                {
                    Object itemObject = AssetDatabase.LoadAssetAtPath(itemFilePath + Enum.GetName(typeof(ItemCategories), (ItemCategories)itemIndex) + "/" + n.ToString() + ".prefab", typeof(Object));
                    if (itemObject != null)
                    {
                        _typesOfItems[itemIndex].Add(itemObject);
                    }
                    else
                    {
                        break;
                    }
                    n++;
                } while (n < 30);
            }
            CurrentItemPrefab = _typesOfItems[0][0];

        }

        public static void SetMazeHolder()
        {
            if(Mazes == null)
            {
                Mazes = Selection.activeTransform;
                if (Mazes == null)
                {
                    GameObject tempMazesGameobject = new GameObject();
                    Mazes = tempMazesGameobject.transform;
                }
            }

            Mazes.transform.position = Vector3.zero;
            Mazes.transform.eulerAngles = Vector3.zero;
            Mazes.name = "Mazes";
            
            Selection.SetActiveObjectWithContext(Mazes, Mazes);
            SceneView.lastActiveSceneView.FrameSelected();
        }
        
        public static void SetMazeParent()
        {
            CurrentMaze = Selection.activeGameObject;
            if (CurrentMaze == null)
            {
                var tempMazesGameobject = new GameObject();
                CurrentMaze = tempMazesGameobject;
            }
            CurrentMaze.name = "Maze " + Mazes.childCount.ToString();
            
            CurrentMaze.transform.parent = Mazes;
            CurrentMaze.transform.position = Vector3.zero;

            Selection.SetActiveObjectWithContext(CurrentMaze, CurrentMaze);
            SceneView.lastActiveSceneView.FrameSelected();

            if (CurrentMazeCubePrefab == null)
            {
                Debug.LogError("MazeCube prefab not assigned");
                return;
            }


            if (CurrentMaze.GetComponent<Maze.Maze_e>() == null)
            {
                CurrentMaze.AddComponent<Maze.Maze_e>();
            }

            ReCalculateAllMazeCubes();

            if (AllMazeCubes.Count == 0)
            {
                GameObject obj = Instantiate((GameObject)CurrentMazeCubePrefab, CurrentMaze.transform);
                AllMazeCubes.Add(obj);
            }
        }

        public static void HideAllMazeItems()
        {
            for (int i = 0; i < AllItems.Count; i++)
            {
                for (int j = 0; j < AllItems[i].Count; j++)
                {
                    AllItems[i][j].SetActive(false);
                }
            }
        }

        public static void ShowAllMazeItems()
        {
            for (int i = 0; i < AllItems.Count; i++)
            {
                for (int j = 0; j < AllItems[i].Count; j++)
                {
                    AllItems[i][j].SetActive(true);
                }
            }
        }
        
        public static void ReCalculateAllItems()
        {
            AllItems = new List<List<GameObject>>();
            for (int i = 0; i < Enum.GetNames(typeof(ItemCategories)).Length; i++)
            {
                AllItems.Add(new List<GameObject>());
            }
            
            for (int i = 0; i < CurrentMaze.transform.childCount; i++)
            {
                Transform cube = CurrentMaze.transform.GetChild(i);
                for (int j = 0; j < cube.childCount; j++)
                {
                    Game.Items.IItems item = cube.GetChild(j).GetComponent<Game.Items.IItems>();
                    if (item != null)    //i.e this is an item
                    {
                        AllItems[(int)item.GetItemType()].Add(cube.GetChild(j).gameObject);
                    }
                }
            }
        }

        public static void ReCalculateAllMazeCubes()
        {
            HideAllMazeItems();
            for (int i = 0; i < AllMazeCubes.Count; i++)
            {
                //removing all the deleted maze cubes
                if (AllMazeCubes[i] == null)
                {
                    AllMazeCubes.RemoveAt(i);
                }
            }
            
            for (int i = 0; i < CurrentMaze.transform.childCount; i++)
            {
                CurrentMaze.transform.GetChild(i).gameObject.name = CurrentMaze.name + " cube";
                //adding all the created maze cubes to the list
                if (!AllMazeCubes.Contains(CurrentMaze.transform.GetChild(i).gameObject))
                {
                    AllMazeCubes.Add(CurrentMaze.transform.GetChild(i).gameObject);
                }
            }
            
            ReCalculateNodes();
            if (EditorMode == Modes.Items)
            {
                ShowAllMazeItems();
            }
        }

        public static void ReCalculateNodes()
        {
            HideAllMazeItems();
            List<GameObject> tempWallList = new List<GameObject>();    //used to delete all the maze walls
            List<GameObject> tempNodeList = new List<GameObject>();    //used to delete the nodes which are not needed
            
            //create sub nodes for the new maze cube
            for (int i = 0; i < AllMazeCubes.Count; i++)
            {
                RaycastHit hit;
                foreach (var direction in Helper.Vector3Directions)
                {
                    bool flag = false;
                    if (Physics.Raycast(AllMazeCubes[i].transform.position, direction, out hit, 1.1f))
                    {
                        for (int j = 0; j < AllMazeCubes[i].transform.childCount; j++)
                        {
                            if (AllMazeCubes[i].transform.GetChild(j).forward == direction && AllMazeCubes[i].transform.GetChild(j).GetComponent<Game.Maze.Node>() != null)
                            {
                                tempNodeList.Add(AllMazeCubes[i].transform.GetChild(j).gameObject);
                            }
                        }
                        
                        if (hit.transform.CompareTag("MazeCube"))
                        {
                            flag = true;
                        }
                    }

                    if (!flag)
                    {
                        for (int j = 0; j < AllMazeCubes[i].transform.childCount; j++)
                        {
                            if(AllMazeCubes[i].transform.GetChild(j).GetComponent<Game.Maze.Node>() == null)
                                continue;
                            
                            if (AllMazeCubes[i].transform.GetChild(j).transform.forward == direction)
                            {
                                flag = true;
                            }

                            for (int k = 0; k < AllMazeCubes[i].transform.GetChild(j).childCount; k++)
                            {
                                //adding all the maze wall gameObjects to the tempList
                                tempWallList.Add(AllMazeCubes[i].transform.GetChild(j).GetChild(k).gameObject);    
                            }
                        }
                    }
                    
                    if (!flag)
                    {
                        GameObject node = new GameObject();

                        node.AddComponent<Game.Maze.Node>();
                        node.GetComponent<Game.Maze.Node>().parentCubePos = AllMazeCubes[i].transform.position;

                        node.transform.parent = AllMazeCubes[i].transform;
                        node.transform.position = AllMazeCubes[i].transform.position + direction * 0.5f;
                        node.transform.forward = direction;
                    }
                }
                if (EditorMode == Modes.Items)
                {
                    ShowAllMazeItems();
                }
            }

            for (int i = 0; i < AllMazeCubes.Count; i++)
            {
                for (int j = 0; j < AllMazeCubes[i].transform.childCount; j++)
                {
                    if (AllMazeCubes[i].transform.GetChild(j).GetComponent<Game.Maze.Node>() != null)
                    {
                        AllMazeCubes[i].transform.GetChild(j).GetComponent<Game.Maze.Node>().ReCalculateNeighbourInterations();
                        AllMazeCubes[i].transform.GetChild(j).GetComponent<Game.Maze.Node>().CalculateRenderNodePath();
                    }
                }
            }

            //destroying all the maze wall gameObjects
            foreach (var o in tempWallList)
            {
                DestroyImmediate(o);
            }
            //destroying all the nodes which are not needed
            foreach (var o in tempNodeList)
            {
                DestroyImmediate(o);
            }
        }

        public void ResetPaths()
        {
            ReCalculateAllMazeCubes();
            Debug.Log("the path is reset");
            for (int i = 0; i < AllMazeCubes.Count; i++)
            {
                for (int j = 0; j < AllMazeCubes[i].transform.childCount; j++)
                {
                    if (AllMazeCubes[i].transform.GetChild(j).GetComponent<Game.Maze.Node>() != null)
                    {
                        AllMazeCubes[i].transform.GetChild(j).GetComponent<Game.Maze.Node>().Reset();
                    }
                }
            }
        }

        public static void RenderPaths()
        {
            HideAllMazeItems();
            
            for (int i = 0; i < AllMazeWalls.Count; i++)
            {
                DestroyImmediate(AllMazeWalls[i].gameObject);
            }
            AllMazeWalls = new List<GameObject>();

            for (int i = 0; i < AllMazeCubes.Count; i++)
            {
                for (int j = 0; j < AllMazeCubes[i].transform.childCount; j++)
                {
                    Game.Maze.Node node = AllMazeCubes[i].transform.GetChild(j).GetComponent<Game.Maze.Node>();
                    if (node == null)
                        continue;
                    node.CalculateRenderNodePath();
                }
            }
                        
            for (int i = 0; i < AllMazeCubes.Count; i++)
            {
                for (int j = 0; j < AllMazeCubes[i].transform.childCount; j++)
                {
                    Game.Maze.Node node = AllMazeCubes[i].transform.GetChild(j).GetComponent<Game.Maze.Node>();
                    if (node == null)
                        continue;
                    
                    float offset = 1 / 2f - 1 / 12f;
                    float height_offset = 1 / 12f;

                    float external_offset = 1 / 2f + 1 / 12f;

                    float w_size = 4 / 6f;
                    float c_size = 1 / 6f;
                    float h_size = 1 / 6f;

                    //rendering walls
                    if (node.rrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab, node.transform.position + node.transform.right * offset + node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                        tempobj.name = "r";
                        AllMazeWalls.Add(tempobj);
                    }
                    if (node.lrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab, node.transform.position - node.transform.right * offset + node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                        tempobj.name = "l";
                        AllMazeWalls.Add(tempobj);
                    }
                    if (node.urender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab, node.transform.position + node.transform.up * offset + node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                        tempobj.name = "u";
                        AllMazeWalls.Add(tempobj);
                    }
                    if (node.drender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab, node.transform.position - node.transform.up * offset + node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                        tempobj.name = "d";
                        AllMazeWalls.Add(tempobj);
                    }

                    //rendering corner
                    if (node.rUrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position + (node.transform.right + node.transform.up) * offset +
                            node.transform.forward * height_offset, node.transform.rotation, node.transform);
                        tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                        tempobj.name = "ru";
                        AllMazeWalls.Add(tempobj);
                    }
                    if (node.rDrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position + (node.transform.right - node.transform.up) * offset +
                            node.transform.forward * height_offset, node.transform.rotation, node.transform);
                        tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                        tempobj.name = "rd";
                        AllMazeWalls.Add(tempobj);
                    }
                    if (node.lUrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position + (-node.transform.right + node.transform.up) * offset +
                            node.transform.forward * height_offset, node.transform.rotation, node.transform);
                        tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                        tempobj.name = "lu";
                        AllMazeWalls.Add(tempobj);
                    }
                    if (node.lDrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position + (-node.transform.right - node.transform.up) * offset +
                            node.transform.forward * height_offset, node.transform.rotation, node.transform);
                        tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                        tempobj.name = "ld";
                        AllMazeWalls.Add(tempobj);
                    }

                    //rendering external edges
                    if (node.eRrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab, node.transform.position + node.transform.right * external_offset + node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                        tempobj.name = "er";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.eLrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position - node.transform.right * external_offset +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                        tempobj.name = "el";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.eUrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position + node.transform.up * external_offset +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                        tempobj.name = "eu";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.eDrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position - node.transform.up * external_offset +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                        tempobj.name = "ed";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.erUrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position +
                            (node.transform.right * external_offset + node.transform.up * offset) +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                        tempobj.name = "eru";
                        AllMazeWalls.Add(tempobj);

                    }

                    if (node.erDrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position +
                            (node.transform.right * external_offset - node.transform.up * offset) +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                        tempobj.name = "erd";
                        AllMazeWalls.Add(tempobj);

                    }

                    if (node.elUrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position +
                            (-node.transform.right * external_offset + node.transform.up * offset) +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                        tempobj.name = "elu";
                        AllMazeWalls.Add(tempobj);

                    }

                    if (node.elDrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position +
                            (-node.transform.right * external_offset - node.transform.up * offset) +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                        tempobj.name = "eld";
                        AllMazeWalls.Add(tempobj);

                    }

                    if (node.euRrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position +
                            (node.transform.right * offset + node.transform.up * external_offset) +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 90);
                        tempobj.name = "eur";
                        AllMazeWalls.Add(tempobj);

                    }

                    if (node.euLrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position +
                            (-node.transform.right * offset + node.transform.up * external_offset) +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 90);
                        tempobj.name = "eul";
                        AllMazeWalls.Add(tempobj);

                    }

                    if (node.edRrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position +
                            (node.transform.right * offset - node.transform.up * external_offset) +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 90);
                        tempobj.name = "edr";
                        AllMazeWalls.Add(tempobj);

                    }

                    if (node.edLrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position +
                            (-node.transform.right * offset - node.transform.up * external_offset) +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 90);
                        tempobj.name = "edl";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.eerUrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position +
                            (node.transform.right + node.transform.up) * external_offset +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localScale = new Vector3(h_size, h_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                        tempobj.name = "eeru";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.eerDrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position +
                            (node.transform.right - node.transform.up) * external_offset +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localScale = new Vector3(h_size, h_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                        tempobj.name = "eerd";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.eelUrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position +
                            (-node.transform.right + node.transform.up) * external_offset +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localScale = new Vector3(h_size, h_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                        tempobj.name = "eelu";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.eelDrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position +
                            (-node.transform.right - node.transform.up) * external_offset +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localScale = new Vector3(h_size, h_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                        tempobj.name = "eeld";
                        AllMazeWalls.Add(tempobj);
                    }

                }
            }

            if (EditorMode == Modes.Items)
            {
                ShowAllMazeItems();
            }
        }

        public static List<GameObject> GetSurfaceMazeCubes()
        {
            List<GameObject> surfaceMazeCubes = new List<GameObject>();

            for (int i = 0; i < AllMazeCubes.Count; i++)
            {
                if (AllMazeCubes[i].transform.childCount != 0)
                {
                    surfaceMazeCubes.Add(AllMazeCubes[i]);
                }
            }

            return surfaceMazeCubes;
        }

        /*
         *returs a list of maze items whose values are properly set
         * 
         */
        public static void GetAllMazeItems(out List<List<GameObject>> allMazeItems)
        {
            ReCalculateAllItems();
            allMazeItems = new List<List<GameObject>>();
            for (int itemType = 0; itemType < _typesOfItems.Count; itemType++)
            {
                allMazeItems.Add(new List<GameObject>());
                for (int i = 0; i < AllItems[itemType].Count; i++)
                {
                    //TODO: check if the item is set
                    //TODO: add the item to allMazeItems
                    switch ((ItemCategories)itemType)
                    {
                        case ItemCategories.Path:
                            switch (AllItems[itemType][i].name)
                            {
                                case "Ice":
                                    allMazeItems[itemType].Add((AllItems[itemType][i]));
                                    break;
                            }
                            break;
                        case ItemCategories.Intractable:
                            switch (AllItems[itemType][i].name)
                            {
                                case "Spike":
                                    if (AllItems[itemType][i].GetComponent<Game.Items.Intractable.Spike.Spike>().itemSet)
                                    {
                                        allMazeItems[itemType].Add(AllItems[itemType][i]);
                                    }
                                    break;
                                case "Portal":
                                    if (AllItems[itemType][i].GetComponent<Game.Items.Intractable.Portal.Portal>().itemSet)
                                    {
                                        allMazeItems[itemType].Add(AllItems[itemType][i]);
                                    }
                                    break;
                                case "Gate":
                                    if (AllItems[itemType][i].GetComponent<Game.Items.Intractable.Gate.Gate>().itemSet)
                                    {
                                        allMazeItems[itemType].Add(AllItems[itemType][i]);
                                    }
                                    break;
                                case "Laser":
                                    if (AllItems[itemType][i].GetComponent<Game.Items.Intractable.Laser.Laser>().itemSet)
                                    {
                                        allMazeItems[itemType].Add(AllItems[itemType][i]);
                                    }
                                    break;
                            }
                            break;
                        case ItemCategories.Activator:
                            switch (AllItems[itemType][i].name)
                            {
                                case "Button":
                                    if (AllItems[itemType][i].GetComponent<Game.Items.Activators.Button.Button>().itemSet)
                                    {
                                        allMazeItems[itemType].Add(AllItems[itemType][i]);
                                    }
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
        }
    }

    public class Helper
    {
        public static List<Vector3> Vector3Directions = new List<Vector3>
        {
            Vector3.forward,
            Vector3.back,
            Vector3.up,
            Vector3.down,
            Vector3.right,
            Vector3.left,
        };

        public static int GetUniqueIntractableID()
        {
            int tempId = 1;
            
            List<int> allIds = new List<int>();
            List<List<GameObject>> allItems;
            LevelEditor.GetAllMazeItems(out allItems);
            
            for (int index = 0; index < allItems[(int)ItemCategories.Intractable].Count; index++)
            {
                if (allItems[(int) ItemCategories.Intractable][index].GetComponent<IIntractables>().GetIntractableId() != 0)
                {
                    allIds.Add(allItems[(int) ItemCategories.Intractable][index].GetComponent<IIntractables>().GetIntractableId());
                }
            }
            allIds.Sort();
            foreach (var id in allIds)
            {
                Debug.Log(id);
            }
            foreach (var id in allIds)
            {
                if (!tempId.Equals(id))
                {
                    break;
                }

                tempId++;
            }

            return tempId;
        }
        
    }

    public interface ITem
    {
        /// <summary>
        /// used to initialize the item,
        /// called just after the item is created
        /// </summary>
        void Init();            //creates an item and initialises it
        
        /// <summary>
        /// 
        /// </summary>
        void AddItem();         //adds the item to the item list in the LevelEditor script
        
        /// <summary>
        /// 
        /// </summary>
        void EditItem();        //the user can edit the properties of the item
        
        /// <summary>
        /// used to remove the item
        /// </summary>
        void RemoveItem();      //removes the item from the item list

        /// <summary>
        /// returns true if the item is set with values else false
        /// </summary>
        /// <returns></returns>
        bool CheckValuesSet(); //returns true if the item serializable field values are set
    }
    
    public interface ITemButtonInteraction
    {
        /// <summary>
        /// adds the corresponding button as the link button
        /// for the item
        /// </summary>
        /// <param name="button"></param>
        void AddButtonLink(Game.Items.Activators.Button.Button button);       //creates a button which can interact with the item
        
        /// <summary>
        /// used to edit the values/ properties of the linked button
        /// </summary>
        void EditButtonLink();      //the user can edit the properties of the button and its interaction
        
        /// <summary>
        /// used to remove the button link to the item
        /// </summary>
        /// <param name="buttonId"></param>
        void RemoveButtonLink(int buttonId);    //removes the button with buttonID
    }
}