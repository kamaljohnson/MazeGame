﻿using System;
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
        public Maze_e Maze;

        public static Game.Maze.Node StartNode;
        public static Game.Maze.Node EndNode;

        private bool _pathCreationStarted = false;

        public void OnEnable()
        {
            Maze = (Maze_e)target;
            LevelEditor.CurrentMaze = Maze.gameObject;
            LevelEditor.CurrentMaze.GetComponent<Maze.Maze_e>().ID = LevelEditor.CurrentMaze.name.Split(' ')[1];
            if (LevelEditor.Mazes != null)
            {
                Selection.SetActiveObjectWithContext(Maze, Maze);
                SceneView.lastActiveSceneView.FrameSelected();
                if (Maze.transform.parent != LevelEditor.Mazes)
                {
                    LevelEditor.SetMazeParent();
                }
                
                LevelEditor.ReCalculateAllMazeCubes();
                LevelEditor.ReCalculateNodes();
            }

            StartNode = null;
            EndNode = null;

        }

        public void OnSceneGUI()
        {
            //itrate through all the children of type mazecube and display the handle button as needed

            for (int i = 0; i < Maze.transform.childCount; i++)
            {
                switch (LevelEditor.EditorMode)
                {
                    case Modes.MAZE_BODY:
                        Tools.current = Tool.None;
                        CreateBlockCreationHandle(Maze.transform.GetChild(i).gameObject);
                        break;
                    case Modes.MAZE_LAYOUT:
                        Tools.current = Tool.None;
                        CreateMazeStructureHandle(Maze.transform.GetChild(i).gameObject);
                        break;
                    case Modes.ITEMS:
                        Tools.current = Tool.None;
                        CreateItemCreationHandle(Maze.transform.GetChild(i).gameObject);
                        break;
                    case Modes.MAZE_POS:
                        Tools.current = Tool.None;
                        CreateSetMazePositionHandle();
                        break;
                    case Modes.MAZE_PIVOT:
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
                    break;
                
                Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;

                Handles.color = Color.black;

                if (node.RightPath)
                {
                    Handles.DrawAAPolyLine(Texture2D.whiteTexture, 10, node.transform.position + node.transform.forward * 0.01f,
                        node.transform.position + node.transform.forward * 0.01f + node.transform.right * 0.52f);
                }
                if (node.LeftPath)
                {
                    Handles.DrawAAPolyLine(Texture2D.whiteTexture, 10, node.transform.position + node.transform.forward * 0.01f,
                        node.transform.position + node.transform.forward * 0.01f - node.transform.right * 0.52f);
                }
                if (node.UpPath)
                {
                    Handles.DrawAAPolyLine(Texture2D.whiteTexture, 10, node.transform.position + node.transform.forward * 0.01f,
                        node.transform.position + node.transform.forward * 0.01f + node.transform.up * 0.52f);
                }
                if (node.DownPath)
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
                        obj.transform.up = offset;
                        obj.GetComponent<Collider>().enabled = false;
                        LevelEditor.AllItems[(int)LevelEditor.CurrentItemType].Add(obj);
                    }
                }
            }
        }

        private void CreateSetMazePositionHandle()
        {
            EditorGUI.BeginChangeCheck();
            Vector3 newTargetPosition = Handles.PositionHandle(Maze.transform.position, Maze.transform.rotation);
            if (EditorGUI.EndChangeCheck())
            {
                if (Vector3.Distance(newTargetPosition, Maze.transform.position) == 1)
                {
                    Maze.transform.position = newTargetPosition;
                    for (int i = 0; i < Maze.transform.childCount; i++)
                    {
                        Transform cube = Maze.transform.GetChild(i);
                        for (int j = 0; j < cube.childCount; j++)
                        {
                            Transform node = cube.GetChild(j);
                            if (node.GetComponent<Game.Maze.Node>() != null)
                            {
                                node.GetComponent<Game.Maze.Node>().ParentCubePos = cube.transform.position;
                            }
                        }
                    }
                }
            }
        }

        private void CreateSetMazePivotHandle()
        {
            EditorGUI.BeginChangeCheck();
            Vector3 newTargetPosition = Handles.PositionHandle(Maze.transform.position, Maze.transform.rotation);
            if (EditorGUI.EndChangeCheck())
            {
                if (Vector3.Distance(newTargetPosition, Maze.transform.position) == 0.5f)
                {
                    for (int i = 0; i < Maze.transform.childCount; i++)
                    {
                        Maze.transform.GetChild(i).position -= newTargetPosition - Maze.transform.position;
                    }
                    Maze.transform.position = newTargetPosition;
                }
            }
        }
    }
}