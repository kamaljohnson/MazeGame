using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Graphs;

namespace LevelEditor.Maze
{
    [CustomEditor(typeof(Maze_e))]
    public class MazeEditorScript : Editor
    {
        public Maze_e maze;

        public static Game.Maze.Node StartNode;
        public static Game.Maze.Node EndNode;

        public void OnEnable()
        {
            maze = (Maze_e)target;
            LevelEditor.CurrentMaze = maze.gameObject;
            LevelEditor.CurrentMaze.GetComponent<Maze.Maze_e>().ID = LevelEditor.CurrentMaze.name.Split(' ')[1];
            if (LevelEditor.Mazes != null)
            {
                Selection.SetActiveObjectWithContext(maze, maze);
                SceneView.lastActiveSceneView.FrameSelected();
                if (maze.transform.parent != LevelEditor.Mazes)
                {
                    LevelEditor.SetMazeParent();
                }
                LevelEditor.ReCalculateAllItems();
                LevelEditor.ReCalculateAllMazeCubes();
            }

            StartNode = null;
            EndNode = null;

        }

        public void OnSceneGUI()
        {
            //itrate through all the children of type mazecube and display the handle button as needed

            for (int i = 0; i < maze.transform.childCount; i++)
            {
                switch (LevelEditor.EditorMode)
                {
                    case Modes.MazeBody:
                        Tools.current = Tool.None;
                        CreateBlockCreationHandle(maze.transform.GetChild(i).gameObject);
                        break;
                    case Modes.MazeLayout:
                        Tools.current = Tool.None;
                        CreateMazeStructureHandle(maze.transform.GetChild(i).gameObject);
                        break;
                    case Modes.Items:
                        Tools.current = Tool.None;
                        CreateItemCreationHandle(maze.transform.GetChild(i).gameObject);
                        break;
                    case Modes.MazePos:
                        Tools.current = Tool.None;
                        CreateSetMazePositionHandle();
                        break;
                    case Modes.MazePivot:
                        Tools.current = Tool.None;
                        CreateSetMazePivotHandle();
                        break;
                }
            }
        }

        private void CreateBlockCreationHandle(GameObject mazeCube)
        {
            RaycastHit hit;
            foreach (var offset in Helper.Vector3Directions)
            {
                if (!Physics.Raycast(mazeCube.transform.position, offset, out hit, 1f))
                {

                    Handles.color = new Color(1f, 0.5f, 0.41f);
                    Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
                    if (Handles.Button(mazeCube.transform.position + (offset * 0.55f), Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
                    {
                        GameObject obj = Instantiate((GameObject)LevelEditor.CurrentMazeCubePrefab, mazeCube.transform.position + offset, mazeCube.transform.localRotation, mazeCube.transform.parent);

                        Selection.SetActiveObjectWithContext(obj, obj);
                        SceneView.lastActiveSceneView.FrameSelected();
                        Selection.SetActiveObjectWithContext(LevelEditor.CurrentMaze, LevelEditor.CurrentMaze);

                        LevelEditor.AllMazeCubes.Add(obj);
                    }
                }
            }
        }

        private void CreateMazeStructureHandle(GameObject mazeCube)
        {
            RaycastHit hit;
            for (int i = 0; i < mazeCube.transform.childCount; i++)
            {
                Game.Maze.Node node = mazeCube.transform.GetChild(i).gameObject.GetComponent<Game.Maze.Node>();
                if (node == null)
                    continue;
                
                Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;

                Handles.color = Color.black;

                if (node.rightPath)
                {
                    Handles.DrawAAPolyLine(Texture2D.whiteTexture, 10, node.transform.position + node.transform.forward * 0.01f,
                        node.transform.position + node.transform.forward * 0.01f + node.transform.right * 0.52f);
                }
                if (node.leftPath)
                {
                    Handles.DrawAAPolyLine(Texture2D.whiteTexture, 10, node.transform.position + node.transform.forward * 0.01f,
                        node.transform.position + node.transform.forward * 0.01f - node.transform.right * 0.52f);
                }
                if (node.upPath)
                {
                    Handles.DrawAAPolyLine(Texture2D.whiteTexture, 10, node.transform.position + node.transform.forward * 0.01f,
                        node.transform.position + node.transform.forward * 0.01f + node.transform.up * 0.52f);
                }
                if (node.downPath)
                {
                    Handles.DrawAAPolyLine(Texture2D.whiteTexture, 10, node.transform.position + node.transform.forward * 0.01f,
                        node.transform.position + node.transform.forward * 0.01f - node.transform.up * 0.52f);
                }

                if (StartNode != node)
                {
                    Handles.color = new Color(0.99f, 0.99f, 1f);
                }
                else
                {
                    Handles.color = new Color(1f, 0.57f, 0.34f);
                }

                if (node.inactive)
                {
                    Handles.color = Color.black;
                }


                if (Handles.Button(node.transform.position, Quaternion.Euler(node.transform.eulerAngles), 0.15f, 0.15f, Handles.SphereCap))
                {
                    if (!LevelEditor.InactiveNodeEditing)
                    {
                        EndNode = node;


                        if (StartNode != null)
                        {
                            StartNode.CalculatePathDirection(EndNode);
                            EndNode.CalculatePathDirection(StartNode);
                        }
                        else
                        {
                            Debug.Log("new path started");
                            //LevelEditor.ReCalculateAllMazeCubes();
                        }

                        if (StartNode == EndNode)
                        {
                            StartNode = null;
                            EndNode = null;
                        }
                        else
                        {
                            EndNode = null;
                            StartNode = node;
                        }

                    }
                    else
                    {
                        if (!node.inactive)
                        {
                            node.Reset();
                            node.inactive = true;
                        }
                        else
                        {
                            node.inactive = false;
                            StartNode = node;
                        }
                    }
                }
            }
        }

        private void CreateItemCreationHandle(GameObject mazeCube)
        {
            RaycastHit hit;
            foreach (var offset in Helper.Vector3Directions)
            {
                if (!Physics.Raycast(mazeCube.transform.position, offset, out hit, 1f))
                {

                    Handles.color = new Color(1f, 0.5f, 0.41f);
                    Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
                    if (Handles.Button(mazeCube.transform.position + (offset * 0.55f), Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
                    {
                        GameObject obj = Instantiate((GameObject)LevelEditor.CurrentItemPrefab, mazeCube.transform.position + offset, mazeCube.transform.localRotation, mazeCube.transform);
                        if (obj.transform.CompareTag("Gate"))
                        {
                            obj.transform.forward = offset;
                        }
                        else
                        {
                            obj.transform.up = offset;
                        }
                        obj.GetComponent<Collider>().enabled = false;
                        LevelEditor.AllItems[(int)LevelEditor.CurrentItemType].Add(obj);
                    }
                }
            }
        }

        private void CreateSetMazePositionHandle()
        {
            EditorGUI.BeginChangeCheck();
            Vector3 newTargetPosition = Handles.PositionHandle(maze.transform.position, maze.transform.rotation);
            if (EditorGUI.EndChangeCheck())
            {
                if (Vector3.Distance(newTargetPosition, maze.transform.position) == 1)
                {
                    maze.transform.position = newTargetPosition;
                    for (int i = 0; i < maze.transform.childCount; i++)
                    {
                        Transform cube = maze.transform.GetChild(i);
                        for (int j = 0; j < cube.childCount; j++)
                        {
                            Transform node = cube.GetChild(j);
                            if (node.GetComponent<Game.Maze.Node>() != null)
                            {
                                node.GetComponent<Game.Maze.Node>().parentCubePos = cube.transform.position;
                            }
                        }
                    }
                }
            }
        }

        private void CreateSetMazePivotHandle()
        {
            EditorGUI.BeginChangeCheck();
            Vector3 newTargetPosition = Handles.PositionHandle(maze.transform.position, maze.transform.rotation);
            if (EditorGUI.EndChangeCheck())
            {
                if (Vector3.Distance(newTargetPosition, maze.transform.position) == 0.5f)
                {
                    for (int i = 0; i < maze.transform.childCount; i++)
                    {
                        maze.transform.GetChild(i).position -= newTargetPosition - maze.transform.position;
                    }
                    maze.transform.position = newTargetPosition;
                }
            }
        }
    }
}