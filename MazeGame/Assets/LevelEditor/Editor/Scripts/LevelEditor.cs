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
        public static List<Object> TypesOfMazeCubes = new List<Object>();
        public static List<Object> TypesOfMazeWalls = new List<Object>();
        public static List<List<Object>> TypesOfItems = new List<List<Object>>();
        private int TotalNumberOFMazeCubeTypes;
        private int TotalNumbetOfMazeWallTypes;
        public static Object currentMazeCubePrefab;
        public static Object currentMazeWallPrefab;
        public static Object currentItemPrefab;

        //actual list of gameobject in the scene
        public static List<GameObject> AllMazeCubes = new List<GameObject>();
        public static List<GameObject> AllMazeWalls = new List<GameObject>();
        public static List<List<GameObject>> AllItems = new List<List<GameObject>>();

        #region local variables
        private Vector2 prefabScrollViewValue = Vector2.zero;
        private ItemCategories currentItemCatgory;
        #endregion

        public static Modes editorMode;
        public static bool inactiveNodesEditing = false;

        [MenuItem("Window/LevelEditor")]
        public static void ShowWindow()
        {
            LevelEditor window = GetWindow<LevelEditor>();

            window.title = "Level Editor";
        }

        void OnEnable()
        {
            TotalNumberOFMazeCubeTypes = 0;

            MazeCubesFilePath = "Assets/LevelEditor/Prefabs/MazeCubes/MazeCube 1";
            MazeWallsFilePath = "Assets/LevelEditor/Prefabs/MazeWalls/MazeWall 1";
            ItemFilePath = "Assets/LevelEditor/Prefabs/Items/";
            AddMazePrefabs();
            AddItemPrefabs();
        }

        void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            GUILayout.Label("LEVEL EDITOR", style);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            if (editorMode == Modes.ITEMS)
            {
                prefabScrollViewValue = GUILayout.BeginScrollView(prefabScrollViewValue, GUI.skin.scrollView);
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
                        int _horizontalStackingLimit = 4;
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
                if(GUILayout.Button("Create Item", GUILayout.Height(30)))
                {

                }
                if (GUILayout.Button("Refresh", GUILayout.Height(30)))
                {
                    AddItemPrefabs();
                }

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                GUILayout.EndVertical();
                //display all the items in the scene
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("ADDED ITEMS", _style);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.BeginVertical();
                for (int _typeIndex = 0; _typeIndex < AllItems.Count; _typeIndex++)
                {
                    GUILayout.BeginVertical();
                    GUILayout.Label(Enum.GetName(typeof(ItemCategories), (ItemCategories)_typeIndex));
                    for (int _itemIndex = 0; _itemIndex < AllItems[_typeIndex].Count;)
                    {
                        DrawCustomItemButtons(_typeIndex, _itemIndex);

                        _itemIndex++;
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Maze Cubes");
                GUILayout.Label("Maze Walls");
                GUILayout.EndHorizontal();

                prefabScrollViewValue = GUILayout.BeginScrollView(prefabScrollViewValue, GUI.skin.scrollView);
                if (currentMazeCubePrefab == null && TotalNumberOFMazeCubeTypes > 0)
                {
                    currentMazeCubePrefab = TypesOfMazeCubes[0];
                }
                int largestCollection = Mathf.Max(TotalNumberOFMazeCubeTypes, TotalNumbetOfMazeWallTypes);
                for (int i = 0; i < largestCollection; i++)
                {
                    GUILayout.BeginHorizontal();
                    if (i < TotalNumberOFMazeCubeTypes)
                    {
                        DrawCustomMazeButtons(i);
                    }
                    else
                    {
                        GUILayout.FlexibleSpace();
                    }

                    if (i < TotalNumbetOfMazeWallTypes)
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
            int _size = 80;
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
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            if (CurrentMaze != null)
            {
                editorMode = (Modes)EditorGUILayout.EnumPopup("", editorMode);
                switch (editorMode)
                {
                    case Modes.MAZE_BODY:
                        ReCalculateAllMazeCubes();
                        ReCalculateNodes();
                        break;
                    case Modes.MAZE_LAYOUT:
                        ReCalculateAllMazeCubes();
                        ReCalculateNodes();

                        inactiveNodesEditing = GUILayout.Toggle(inactiveNodesEditing, "set inactive nodes");

                        if (GUILayout.Button("reset paths"))
                        {
                            if (!EditorUtility.DisplayDialog("Warning!!", "Resetting the maze layout will delete the current progress completely", "Cancel", "Reset"))
                            {
                                ResetPaths();
                                ReCalculateNodes();
                            }

                        }
                        break;
                    case Modes.ITEMS:
                        break;
                    case Modes.MAZE_POS:

                        break;
                    default:
                        break;
                }

            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save", GUILayout.Height(30)))
            {
                Save.LevelSaveManager sm = ScriptableObject.CreateInstance<Save.LevelSaveManager>();
                sm.Save();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Render", GUILayout.Height(30)))
            {
                Maze.MazeEditorScript.endNode = null;
                Maze.MazeEditorScript.startNode = null;
                for (int k = 0; k < Mazes.childCount; k++)
                {
                    CurrentMaze = Mazes.GetChild(k).gameObject;

                    RenderPaths();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
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
                        node.GetComponent<Game.Maze.Node>().parentCube_pos = AllMazeCubes[i].transform.position;

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
                    AllMazeCubes[i].transform.GetChild(j).GetComponent<Game.Maze.Node>().ReCalculateNeighbourInterations();
                }

                if (AllMazeCubes[i].transform.childCount == 0)
                {
                    //DestroyImmediate(AllMazeCubes[i].gameObject);
                    //AllMazeCubes.RemoveAt(i);
                }
            }
        }

        public void DrawCustomMazeButtons(int index)
        {
            bool selected = currentMazeCubePrefab == TypesOfMazeCubes[index];

            Texture2D previewImage = AssetPreview.GetAssetPreview((GameObject)TypesOfMazeCubes[index]);

            GUIContent buttonContent = new GUIContent(previewImage);

            bool isToggleDown = GUILayout.Toggle(selected, buttonContent, GUI.skin.button);
            if (isToggleDown)
            {
                currentMazeCubePrefab = TypesOfMazeCubes[index];
            }
        }

        public void DrawCustomMazeWallButtons(int index)
        {
            bool selected = currentMazeWallPrefab == TypesOfMazeWalls[index];

            Texture2D previewImage = AssetPreview.GetAssetPreview((GameObject)TypesOfMazeWalls[index]);

            GUIContent buttonContent = new GUIContent(previewImage);

            bool isToggleDown = GUILayout.Toggle(selected, buttonContent, GUI.skin.button);
            if (isToggleDown)
            {
                currentMazeWallPrefab = TypesOfMazeWalls[index];
            }
        }

        public void DrawCustomItemButtons(int typeIndex, int itemIndex)
        {
            GUILayout.BeginVertical();
            bool selected = currentItemPrefab == TypesOfItems[typeIndex][itemIndex];

            Texture2D previewImage = AssetPreview.GetAssetPreview((GameObject)TypesOfItems[typeIndex][itemIndex]);

            GUIContent buttonContent = new GUIContent(previewImage);

            bool isToggleDown = GUILayout.Toggle(selected, buttonContent, GUI.skin.button, GUILayout.Height(75), GUILayout.Width(75));
            if (isToggleDown)
            {
                currentItemPrefab = TypesOfItems[typeIndex][itemIndex];
            }
            GUILayout.Label(TypesOfItems[typeIndex][itemIndex].name);
            GUILayout.EndVertical();
        }

        public void AddMazePrefabs()
        {
            TypesOfMazeCubes = new List<Object>();
            TypesOfMazeWalls = new List<Object>();

            TotalNumberOFMazeCubeTypes = 0;
            TotalNumbetOfMazeWallTypes = 0;

            int i = Int32.Parse(MazeCubesFilePath.Split(' ')[1]);

            int n = 30;
            do
            {
                if (AssetDatabase.LoadAssetAtPath(MazeCubesFilePath.Split(' ')[0] + " " + i.ToString() + ".prefab", typeof(Object)))
                {
                    TypesOfMazeCubes.Add(AssetDatabase.LoadAssetAtPath(MazeCubesFilePath.Split(' ')[0] + " " + i.ToString() + ".prefab", typeof(Object)));
                    TotalNumberOFMazeCubeTypes++;
                }
                if (AssetDatabase.LoadAssetAtPath(MazeWallsFilePath.Split(' ')[0] + " " + i.ToString() + ".prefab", typeof(Object)))
                {
                    TypesOfMazeWalls.Add(AssetDatabase.LoadAssetAtPath(MazeWallsFilePath.Split(' ')[0] + " " + i.ToString() + ".prefab", typeof(Object)));
                    TotalNumbetOfMazeWallTypes++;
                }
                i++;
                n--;
            } while (n > 0);

            currentMazeCubePrefab = TypesOfMazeCubes[0];
            currentMazeWallPrefab = TypesOfMazeWalls[0];
        }

        public void AddItemPrefabs()
        {
            TypesOfItems = new List<List<Object>> {
                new List<Object>(), //Path items
                new List<Object>(), //Interactable items
                new List<Object>(), //Collectable items
                new List<Object>(), //Enemie items
            };

            for (int _itemIndex = 0; _itemIndex < Enum.GetNames(typeof(ItemCategories)).Length; _itemIndex++)
            {
                int n = 0;
                do
                {
                    Object _itemObject = AssetDatabase.LoadAssetAtPath(ItemFilePath + Enum.GetName(typeof(ItemCategories), (ItemCategories)_itemIndex) + "/" + n.ToString() + ".prefab", typeof(Object));
                    if (_itemObject != null)
                    {
                        TypesOfItems[_itemIndex].Add(_itemObject);
                        Debug.Log(ItemFilePath + Enum.GetName(typeof(ItemCategories), (ItemCategories)_itemIndex) + "/" + n.ToString() + ".prefab");
                    }
                    else
                    {
                        break;
                    }
                    n++;
                } while (n < 30);
            }
            currentItemPrefab = TypesOfItems[0][0];

        }

        public static void SetMazeHolder()
        {
            Mazes = Selection.activeTransform;
            if (Mazes != null)
            {
                Selection.SetActiveObjectWithContext(Mazes, Mazes);
                SceneView.lastActiveSceneView.FrameSelected();
            }
            else
            {
                Debug.LogError("select an empty gameobject before initializing maze holder");
            }
        }


        public static void SetMazeParent()
        {
            CurrentMaze = Selection.activeGameObject;

            if (CurrentMaze != null)
            {
                CurrentMaze.transform.parent = Mazes;
                Selection.SetActiveObjectWithContext(CurrentMaze, CurrentMaze);
                SceneView.lastActiveSceneView.FrameSelected();

                if (currentMazeCubePrefab == null)
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
                    GameObject obj = Instantiate((GameObject)currentMazeCubePrefab, CurrentMaze.transform);
                    AllMazeCubes.Add(obj);
                }
            }
            else
            {
                Debug.LogError("select an empty gameobject before creating the maze cube");
            }
        }

        public static void ReCalculateAllMazeCubes()
        {
            AllMazeCubes = new List<GameObject>();
            List<GameObject> tempList = new List<GameObject>();

            for (int i = 0; i < CurrentMaze.transform.childCount; i++)
            {
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

                    node.CalculateRenderNodePath();
                }
            }
            for (int i = 0; i < AllMazeCubes.Count; i++)
            {
                for (int j = 0; j < AllMazeCubes[i].transform.childCount; j++)
                {
                    Game.Maze.Node node = AllMazeCubes[i].transform.GetChild(j).GetComponent<Game.Maze.Node>();

                    float offset = 1 / 2f - 1 / 12f;
                    float height_offset = 1 / 12f;

                    float external_offset = 1 / 2f + 1 / 12f;
                    float internal_offset = 1 / 2f - 1 / 12f;

                    float w_size = 4 / 6f;
                    float c_size = 1 / 6f;
                    float h_size = 1 / 6f;

                    //rendering walls
                    if (node.r_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab, node.transform.position + node.transform.right * offset + node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                        tempobj.name = "r";
                        AllMazeWalls.Add(tempobj);
                    }
                    if (node.l_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab, node.transform.position - node.transform.right * offset + node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                        tempobj.name = "l";
                        AllMazeWalls.Add(tempobj);
                    }
                    if (node.u_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab, node.transform.position + node.transform.up * offset + node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                        tempobj.name = "u";
                        AllMazeWalls.Add(tempobj);
                    }
                    if (node.d_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab, node.transform.position - node.transform.up * offset + node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                        tempobj.name = "d";
                        AllMazeWalls.Add(tempobj);
                    }

                    //rendering corner
                    if (node.ru_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab,
                            node.transform.position + (node.transform.right + node.transform.up) * offset +
                            node.transform.forward * height_offset, node.transform.rotation, node.transform);
                        tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                        tempobj.name = "ru";
                        AllMazeWalls.Add(tempobj);
                    }
                    if (node.rd_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab,
                            node.transform.position + (node.transform.right - node.transform.up) * offset +
                            node.transform.forward * height_offset, node.transform.rotation, node.transform);
                        tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                        tempobj.name = "rd";
                        AllMazeWalls.Add(tempobj);
                    }
                    if (node.lu_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab,
                            node.transform.position + (-node.transform.right + node.transform.up) * offset +
                            node.transform.forward * height_offset, node.transform.rotation, node.transform);
                        tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                        tempobj.name = "lu";
                        AllMazeWalls.Add(tempobj);
                    }
                    if (node.ld_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab,
                            node.transform.position + (-node.transform.right - node.transform.up) * offset +
                            node.transform.forward * height_offset, node.transform.rotation, node.transform);
                        tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                        tempobj.name = "ld";
                        AllMazeWalls.Add(tempobj);
                    }

                    //rendering external edges
                    if (node.er_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab, node.transform.position + node.transform.right * external_offset + node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                        tempobj.name = "er";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.el_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab,
                            node.transform.position - node.transform.right * external_offset +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                        tempobj.name = "el";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.eu_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab,
                            node.transform.position + node.transform.up * external_offset +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                        tempobj.name = "eu";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.ed_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab,
                            node.transform.position - node.transform.up * external_offset +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                        tempobj.name = "ed";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.ir_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab,
                            node.transform.position + node.transform.right * internal_offset +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                        tempobj.name = "er";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.il_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab,
                            node.transform.position - node.transform.right * internal_offset +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                        tempobj.name = "el";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.iu_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab,
                            node.transform.position + node.transform.up * internal_offset +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                        tempobj.name = "eu";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.id_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab,
                            node.transform.position - node.transform.up * internal_offset +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                        tempobj.name = "ed";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.eru_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab,
                            node.transform.position +
                            (node.transform.right * external_offset + node.transform.up * offset) +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                        tempobj.name = "eru";
                        AllMazeWalls.Add(tempobj);

                    }

                    if (node.erd_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab,
                            node.transform.position +
                            (node.transform.right * external_offset - node.transform.up * offset) +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                        tempobj.name = "erd";
                        AllMazeWalls.Add(tempobj);

                    }

                    if (node.elu_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab,
                            node.transform.position +
                            (-node.transform.right * external_offset + node.transform.up * offset) +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                        tempobj.name = "elu";
                        AllMazeWalls.Add(tempobj);

                    }

                    if (node.eld_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab,
                            node.transform.position +
                            (-node.transform.right * external_offset - node.transform.up * offset) +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                        tempobj.name = "eld";
                        AllMazeWalls.Add(tempobj);

                    }

                    if (node.eur_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab,
                            node.transform.position +
                            (node.transform.right * offset + node.transform.up * external_offset) +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 90);
                        tempobj.name = "eur";
                        AllMazeWalls.Add(tempobj);

                    }

                    if (node.eul_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab,
                            node.transform.position +
                            (-node.transform.right * offset + node.transform.up * external_offset) +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 90);
                        tempobj.name = "eul";
                        AllMazeWalls.Add(tempobj);

                    }

                    if (node.edr_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab,
                            node.transform.position +
                            (node.transform.right * offset - node.transform.up * external_offset) +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 90);
                        tempobj.name = "edr";
                        AllMazeWalls.Add(tempobj);

                    }

                    if (node.edl_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab,
                            node.transform.position +
                            (-node.transform.right * offset - node.transform.up * external_offset) +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 90);
                        tempobj.name = "edl";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.eeru_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab,
                            node.transform.position +
                            (node.transform.right + node.transform.up) * external_offset +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localScale = new Vector3(h_size, h_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                        tempobj.name = "eeru";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.eerd_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab,
                            node.transform.position +
                            (node.transform.right - node.transform.up) * external_offset +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localScale = new Vector3(h_size, h_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                        tempobj.name = "eerd";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.eelu_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab,
                            node.transform.position +
                            (-node.transform.right + node.transform.up) * external_offset +
                            node.transform.forward * height_offset, Quaternion.identity, node.transform);
                        tempobj.transform.localScale = new Vector3(h_size, h_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                        tempobj.name = "eelu";
                        AllMazeWalls.Add(tempobj);
                    }

                    if (node.eeld_render)
                    {
                        GameObject tempobj = (GameObject)Instantiate(currentMazeWallPrefab,
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

    public interface Item
    {
        void Init();    //creates an item and initialises it
        void AddItem();     //adds the item to the item list
        void EditItem();    //the user can edit the properties of the item
        void RemoveItem();  //removes the item from the item list
    }
    public interface ItemButtonInteraction
    {
        void AddButton();       //creates a button which can interact with the item
        void EditButton();      //the user can edit the properties of the button and its interaction
        void RemoveButton(int buttonID);    //removes the button with buttonID
    }
}