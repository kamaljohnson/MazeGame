using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace LevelEditor
{
    public enum Modes
    {
        MAZE_BODY,      //maze body editing mode
        MAZE_LAYOUT,    //maze structure editing mode
        ITEMS,          //add and delete items on the maze
        MAZE_POS,       //can set the position of the maze
        MAZE_PIVOT,     //set the pivot of the maze
    }

    public enum ItemCategories
    {
        Path,                   // [ICE, FIRE]
        Interactable,           // [PORTAL, LASER, SPIKES, GATES, BRIDGE]
        Collectable,            // [COIN, DIAMOND, COLLECTION POINT]
        Enemie,                 // [GUARDIAN, KNIGHT, HAMMER]
        Decoratable,           // [PLANT_01, FOUNTAIN, ...]
    }

    public class LevelEditor : EditorWindow
    {

        public static Transform Mazes;
        public static GameObject CurrentMaze;

        //path of the prefabs
        public string MazeCubesFilePath;
        public string MazeWallsFilePath;
        public string ItemFilePath;

        //reference of corresponding prefabs
        private static List<Object> _typesOfMazeCubes;
        private static List<Object> TypesOfMazeWalls;
        private static List<List<Object>> TypesOfItems;
        private int _totalNumberOfMazeCubeTypes;
        private int _totalNumbetOfMazeWallTypes;
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
            MazeCubesFilePath = "Assets/LevelEditor/Prefabs/MazeCubes/MazeCube 1";
            MazeWallsFilePath = "Assets/LevelEditor/Prefabs/MazeWalls/MazeWall 1";
            ItemFilePath = "Assets/LevelEditor/Prefabs/Items/";
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


            if (EditorMode == Modes.ITEMS)
            {
                _prefabScrollViewValue = GUILayout.BeginScrollView(_prefabScrollViewValue, GUI.skin.scrollView);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUIStyle _style = new GUIStyle();
                _style.fontSize = 15;
                GUILayout.Label("ITEMS", _style);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                //display all the items available
                GUILayout.BeginVertical();
                for (int _typeIndex = 0; _typeIndex < TypesOfItems.Count; _typeIndex++)
                {
                    GUILayout.BeginVertical();
                    GUILayout.Label(Enum.GetName(typeof(ItemCategories), (ItemCategories)_typeIndex));
                    for (int _itemIndex = 0; _itemIndex < TypesOfItems[_typeIndex].Count;)
                    {
                        int _horizontalStackingLimit = 6;
                        GUILayout.BeginHorizontal();
                        for (int k = 0; k < _horizontalStackingLimit; k++)
                        {
                            DrawCustomItemButtons(_typeIndex, _itemIndex);
                            _itemIndex++;
                            if (_itemIndex == TypesOfItems[_typeIndex].Count && k < _horizontalStackingLimit)
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
                        for (int itemIndex = 0; itemIndex < AllItems[(int)_currentItemCatgoryToggled].Count; itemIndex++)
                        {
                            GUILayout.BeginHorizontal();
                            Texture2D previewImage = AssetPreview.GetAssetPreview((GameObject)TypesOfItems[(int)_currentItemCatgoryToggled][(int)_currentItemCatgoryToggled]);
                            GUIContent buttonContent = new GUIContent(previewImage);
                            GUILayout.Toggle(false, buttonContent, GUI.skin.box, GUILayout.Height(20), GUILayout.Width(20));
                            if (GUILayout.Button("Edit", GUILayout.Height(20)))
                            {
                                
                            }
                            if (GUILayout.Button("Remove", GUILayout.Height(20)))
                            {
                                AllItems[itemType][itemIndex].GetComponent<ITem>().RemoveItem();
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
                int largestCollection = Mathf.Max(_totalNumberOfMazeCubeTypes, _totalNumbetOfMazeWallTypes);
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

                    if (i < _totalNumbetOfMazeWallTypes)
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
                    Modes _tempMode = (Modes) EditorGUILayout.EnumPopup("", EditorMode);
                    if (_tempMode != EditorMode)
                    {
                        EditorMode = _tempMode;
                        ReCalculateAllMazeCubes();
                        ReCalculateNodes();
                    }

                    switch (EditorMode)
                    {
                        case Modes.MAZE_BODY:
                            break;
                        case Modes.MAZE_LAYOUT:

                            InactiveNodeEditing = GUILayout.Toggle(InactiveNodeEditing, "set inactive nodes");

                            if (GUILayout.Button("reset paths"))
                            {
                                if (!EditorUtility.DisplayDialog("Warning!!",
                                    "Resetting the maze layout will delete the current progress completely", "Cancel",
                                    "Reset"))
                                {
                                    ResetPaths();
                                    ReCalculateNodes();
                                }
                            }
                            break;
                        case Modes.ITEMS:
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
            var selected = CurrentMazeWallPrefab == TypesOfMazeWalls[index];
            var previewImage = AssetPreview.GetAssetPreview((GameObject)TypesOfMazeWalls[index]);
            var buttonContent = new GUIContent(previewImage);
            var isToggleDown = GUILayout.Toggle(selected, buttonContent, GUI.skin.button, GUILayout.Height(125), GUILayout.Width(125));
            
            if (isToggleDown)
            {
                CurrentMazeWallPrefab = TypesOfMazeWalls[index];
            }
        }

        public void DrawCustomItemButtons(int typeIndex, int itemIndex)
        {
            var selected = CurrentItemPrefab == TypesOfItems[typeIndex][itemIndex];
            var previewImage = AssetPreview.GetAssetPreview((GameObject)TypesOfItems[typeIndex][itemIndex]);
            var buttonContent = new GUIContent(previewImage);

            var isToggleDown = GUILayout.Toggle(selected, buttonContent, GUI.skin.button, GUILayout.Height(35), GUILayout.Width(35));
            if (isToggleDown)
            {
                CurrentItemPrefab = TypesOfItems[typeIndex][itemIndex];
                CurrentItemType = (ItemCategories)typeIndex;
            }
        }

        public void AddMazePrefabs()
        {
            _typesOfMazeCubes = new List<Object>();
            TypesOfMazeWalls = new List<Object>();

            _totalNumberOfMazeCubeTypes = 0;
            _totalNumbetOfMazeWallTypes = 0;

            var i = int.Parse(MazeCubesFilePath.Split(' ')[1]);

            var n = 30;
            do
            {
                if (AssetDatabase.LoadAssetAtPath(MazeCubesFilePath.Split(' ')[0] + " " + i.ToString() + ".prefab", typeof(Object)))
                {
                    _typesOfMazeCubes.Add(AssetDatabase.LoadAssetAtPath(MazeCubesFilePath.Split(' ')[0] + " " + i.ToString() + ".prefab", typeof(Object)));
                    _totalNumberOfMazeCubeTypes++;
                }
                if (AssetDatabase.LoadAssetAtPath(MazeWallsFilePath.Split(' ')[0] + " " + i.ToString() + ".prefab", typeof(Object)))
                {
                    TypesOfMazeWalls.Add(AssetDatabase.LoadAssetAtPath(MazeWallsFilePath.Split(' ')[0] + " " + i.ToString() + ".prefab", typeof(Object)));
                    _totalNumbetOfMazeWallTypes++;
                }
                i++;
                n--;
            } while (n > 0);

            CurrentMazeCubePrefab = _typesOfMazeCubes[0];
            CurrentMazeWallPrefab = TypesOfMazeWalls[0];
        }

        public void AddItemPrefabs()
        {
            TypesOfItems = new List<List<Object>> {
                new List<Object>(), //Path items
                new List<Object>(), //Interactable items
                new List<Object>(), //Collectable items
                new List<Object>(), //Enemie items
                new List<Object>(), //Decoratable items
            };
            AllItems = new List<List<GameObject>>
            {
                new List<GameObject>(), //Path items
                new List<GameObject>(), //Interactable items
                new List<GameObject>(), //Collectable items
                new List<GameObject>(), //Enemie items
                new List<GameObject>(), //Decoratable items
            };

            for (var itemIndex = 0; itemIndex < Enum.GetNames(typeof(ItemCategories)).Length; itemIndex++)
            {
                int n = 0;
                do
                {
                    Object itemObject = AssetDatabase.LoadAssetAtPath(ItemFilePath + Enum.GetName(typeof(ItemCategories), (ItemCategories)itemIndex) + "/" + n.ToString() + ".prefab", typeof(Object));
                    if (itemObject != null)
                    {
                        TypesOfItems[itemIndex].Add(itemObject);
                    }
                    else
                    {
                        break;
                    }
                    n++;
                } while (n < 30);
            }
            CurrentItemPrefab = TypesOfItems[0][0];

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
                GameObject tempMazesGameobject = new GameObject();
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
            AllMazeCubes = new List<GameObject>();
            List<GameObject> tempList = new List<GameObject>();

            for (int i = 0; i < CurrentMaze.transform.childCount; i++)
            {
                CurrentMaze.transform.GetChild(i).gameObject.name = CurrentMaze.name + " cube";
                for (int j = 0; j < CurrentMaze.transform.GetChild(i).childCount; j++)
                {
                    for (int k = 0; k < CurrentMaze.transform.GetChild(i).GetChild(j).childCount; k++)
                    {
                        tempList.Add(CurrentMaze.transform.GetChild(i).GetChild(j).GetChild(k).gameObject);
                    }
                }

                AllMazeCubes.Add(CurrentMaze.transform.GetChild(i).gameObject);
            }

            foreach (var o in tempList)
            {
                DestroyImmediate(o);
            }
        }

        public static void ReCalculateNodes()
        {
            //create sub nodes for the new maze cube
            for (int i = 0; i < AllMazeCubes.Count; i++)
            {
                foreach (var direction in Helper.Vector3Directions)
                {
                    bool flag = false;
                    for (int j = 0; j < AllMazeCubes[i].transform.childCount; j++)
                    {
                        if (Vector3.Distance(AllMazeCubes[i].transform.GetChild(j).transform.forward, direction) < 0.01f)
                        {
                            flag = true;
                            break;
                        }
                    }

                    if (!flag)
                    {
                        GameObject node = new GameObject();

                        node.AddComponent<Game.Maze.Node>();
                        node.GetComponent<Game.Maze.Node>().ParentCubePos = AllMazeCubes[i].transform.position;

                        node.transform.parent = AllMazeCubes[i].transform;
                        node.transform.position = AllMazeCubes[i].transform.position + direction * 0.5f;
                        node.transform.forward = direction;
                    }
                }

                RaycastHit hit;
                foreach (var node_offset in Helper.Vector3Directions)
                {
                    if (Physics.Raycast(AllMazeCubes[i].transform.position, node_offset, out hit, 1f)) //checks if there is any other maze cube in the direction
                    {
                        for (int j = 0; j < AllMazeCubes[i].transform.childCount; j++)
                        {
                            if (Vector3.Distance(AllMazeCubes[i].transform.GetChild(j).transform.forward, node_offset) < 0.01f)
                            {
                                DestroyImmediate(AllMazeCubes[i].transform.GetChild(j).gameObject);
                                break;
                            }
                        }
                    }
                }


            }

            for (int i = 0; i < AllMazeCubes.Count; i++)
            {
                for (int j = 0; j < AllMazeCubes[i].transform.childCount; j++)
                {
                    if (AllMazeCubes[i].transform.GetChild(j).GetComponent<Game.Maze.Node>() != null)
                    {
                        AllMazeCubes[i].transform.GetChild(j).GetComponent<Game.Maze.Node>().ReCalculateNeighbourInterations();
                    }
                }
            }
        }

        public void ResetPaths()
        {
            Debug.Log("the path is reset");
            for (int i = 0; i < AllMazeCubes.Count; i++)
            {
                for (int j = 0; j < AllMazeCubes[i].transform.childCount; j++)
                {
                    AllMazeCubes[i].transform.GetChild(j).GetComponent<Game.Maze.Node>().Reset();
                }
            }
        }

        public static void RenderPaths()
        {
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
                        break;
                    node.CalculateRenderNodePath();
                }
            }
            for (int i = 0; i < AllMazeCubes.Count; i++)
            {
                for (int j = 0; j < AllMazeCubes[i].transform.childCount; j++)
                {
                    Game.Maze.Node node = AllMazeCubes[i].transform.GetChild(j).GetComponent<Game.Maze.Node>();
                    if (node == null)
                        break;
                    
                    float offset = 1 / 2f - 1 / 12f;
                    float height_offset = 1 / 12f;

                    float external_offset = 1 / 2f + 1 / 12f;
                    float internal_offset = 1 / 2f - 1 / 12f;

                    float w_size = 4 / 6f;
                    float c_size = 1 / 6f;
                    float h_size = 1 / 6f;

                    //rendering walls
                    if (node.Rrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab, node.transform.position + node.transform.right * offset + node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                        tempobj.name = "r";
                        AllMazeWalls.Add(tempobj);
                    }
                    if (node.Lrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab, node.transform.position - node.transform.right * offset + node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                        tempobj.name = "l";
                        AllMazeWalls.Add(tempobj);
                    }
                    if (node.Urender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab, node.transform.position + node.transform.up * offset + node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                        tempobj.name = "u";
                        AllMazeWalls.Add(tempobj);
                    }
                    if (node.Drender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab, node.transform.position - node.transform.up * offset + node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                        tempobj.name = "d";
                        AllMazeWalls.Add(tempobj);
                    }

                    //rendering corner
                    if (node.RUrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position + (node.transform.right + node.transform.up) * offset +
                            node.transform.forward * height_offset, node.transform.rotation, node.transform);
                        tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                        tempobj.name = "ru";
                        AllMazeWalls.Add(tempobj);
                    }
                    if (node.RDrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position + (node.transform.right - node.transform.up) * offset +
                            node.transform.forward * height_offset, node.transform.rotation, node.transform);
                        tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                        tempobj.name = "rd";
                        AllMazeWalls.Add(tempobj);
                    }
                    if (node.LUrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position + (-node.transform.right + node.transform.up) * offset +
                            node.transform.forward * height_offset, node.transform.rotation, node.transform);
                        tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                        tempobj.name = "lu";
                        AllMazeWalls.Add(tempobj);
                    }
                    if (node.LDrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position + (-node.transform.right - node.transform.up) * offset +
                            node.transform.forward * height_offset, node.transform.rotation, node.transform);
                        tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                        tempobj.name = "ld";
                        AllMazeWalls.Add(tempobj);
                    }

                    //rendering external edges
                    if (node.ERrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab, node.transform.position + node.transform.right * external_offset + node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                        tempobj.name = "er";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.ELrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position - node.transform.right * external_offset +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                        tempobj.name = "el";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.EUrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position + node.transform.up * external_offset +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                        tempobj.name = "eu";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.EDrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position - node.transform.up * external_offset +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                        tempobj.name = "ed";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.IRrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position + node.transform.right * internal_offset +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                        tempobj.name = "er";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.ILrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position - node.transform.right * internal_offset +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                        tempobj.name = "el";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.IUrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position + node.transform.up * internal_offset +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                        tempobj.name = "eu";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.IDrender)
                    {
                        GameObject tempobj = (GameObject)Instantiate(CurrentMazeWallPrefab,
                            node.transform.position - node.transform.up * internal_offset +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                        tempobj.name = "ed";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.ERUrender)
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

                    if (node.ERDrender)
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

                    if (node.ELUrender)
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

                    if (node.ELDrender)
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

                    if (node.EURrender)
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

                    if (node.EULrender)
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

                    if (node.EDRrender)
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

                    if (node.EDLrender)
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

                    if (node.EERUrender)
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

                    if (node.EERDrender)
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

                    if (node.EELUrender)
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

                    if (node.EELDrender)
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

        public static List<List<GameObject>> GetMazeItems()
        {
            ReCalculateAllItems();

            List<List<GameObject>> allMazeItems = new List<List<GameObject>>();
            for (int itemType = 0; itemType < TypesOfItems.Count; itemType++)
            {
                for (int i = 0; i < AllItems[itemType].Count; i++)
                {
                    //TODO: check if the item is set
                    //TODO: add the item to allMazeItems
                }                    
            }
            return allMazeItems;
        }
    }

    public class Helper
    {
        public static List<Vector3> Vector3Directions = new List<Vector3>()
        {
            Vector3.forward,
            Vector3.back,
            Vector3.up,
            Vector3.down,
            Vector3.right,
            Vector3.left,
        };
    }

    public interface ITem
    {
        void Init();        //creates an item and initialises it
        void AddItem();     //adds the item to the item list in the LevelEditor script
        void EditItem();    //the user can edit the properties of the item
        void RemoveItem();  //removes the item from the item list

        bool CheckValuesSet(); //returns true if the item serializable field values are set
    }
    
    public interface ItemButtonInteraction
    {
        void AddButton();       //creates a button which can interact with the item
        void EditButton();      //the user can edit the properties of the button and its interaction
        void RemoveButton(int buttonID);    //removes the button with buttonID
    }
}