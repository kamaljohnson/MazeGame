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
        public Maze_e maze = new Maze_e();

        public static Game.Maze.Node startNode;
        public static Game.Maze.Node endNode;

        private bool pathCreationStarted = false;

        public void OnEnable()
        {
            maze = (Maze_e)target;
            LevelEditor.SetMazeParent();
            LevelEditor.ReCalculateNodes();
            LevelEditor.RenderPaths();

            startNode = null;
            endNode = null;

        }

        public void OnSceneGUI()
        {
            //itrate through all the children of type mazecube and display the handle button as needed

            for (int i = 0; i < maze.transform.childCount; i++)
            {
                switch (LevelEditor.editorMode)
                {
                    case LevelEditor.Modes.MAZE_BODY:
                        Tools.current = Tool.None;
                        CreateBlockCreationHandle(maze.transform.GetChild(i).gameObject);
                        break;
                    case LevelEditor.Modes.MAZE_LAYOUT:
                        Tools.current = Tool.None;
                        CreateMazeStructureHandle(maze.transform.GetChild(i).gameObject);
                        break;
                    case LevelEditor.Modes.ITEMS:
                        Tools.current = Tool.None;
                        break;
                    case LevelEditor.Modes.MAZE_POS:
                        Tools.current = Tool.None;
                        CreateSetMazePositionHandle();
                        break;
                    case LevelEditor.Modes.MAZE_PIVOT:
                        Tools.current = Tool.None;
                        CreateSetMazePivotHandle();
                        break;
                }
            }
        }

        public void CreateBlockCreationHandle(GameObject mazeCube)
        {
            RaycastHit hit;
            foreach (var offset in LevelEditor.Vector3Directions)
            {
                if (!Physics.Raycast(mazeCube.transform.position, offset, out hit, 1f))
                {

                    Handles.color = new Color(1f, 0.5f, 0.41f);
                    Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
                    if (Handles.Button(mazeCube.transform.position + (offset * 0.55f), Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
                    {
                        GameObject obj = Instantiate((GameObject)LevelEditor.currentMazeCubePrefab, mazeCube.transform.position + offset, mazeCube.transform.localRotation, mazeCube.transform.parent);

                        Selection.SetActiveObjectWithContext(obj, obj);
                        SceneView.lastActiveSceneView.FrameSelected();
                        Selection.SetActiveObjectWithContext(LevelEditor.CurrentMaze, LevelEditor.CurrentMaze);

                        LevelEditor.AllMazeCubes.Add(obj);
                    }
                }
            }
        }

        public void CreateMazeStructureHandle(GameObject mazeCube)
        {
            RaycastHit hit;
            for (int i = 0; i < mazeCube.transform.childCount; i++)
            {
                Game.Maze.Node node = mazeCube.transform.GetChild(i).gameObject.GetComponent<Game.Maze.Node>();

                Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;


                Handles.color = Color.black;

                if (node.right_path)
                {
                    Handles.DrawAAPolyLine(Texture2D.whiteTexture, 10, node.transform.position + node.transform.forward * 0.01f,
                        node.transform.position + node.transform.forward * 0.01f + node.transform.right * 0.52f);
                }
                if (node.left_path)
                {
                    Handles.DrawAAPolyLine(Texture2D.whiteTexture, 10, node.transform.position + node.transform.forward * 0.01f,
                        node.transform.position + node.transform.forward * 0.01f - node.transform.right * 0.52f);
                }
                if (node.up_path)
                {
                    Handles.DrawAAPolyLine(Texture2D.whiteTexture, 10, node.transform.position + node.transform.forward * 0.01f,
                        node.transform.position + node.transform.forward * 0.01f + node.transform.up * 0.52f);
                }
                if (node.down_path)
                {
                    Handles.DrawAAPolyLine(Texture2D.whiteTexture, 10, node.transform.position + node.transform.forward * 0.01f,
                        node.transform.position + node.transform.forward * 0.01f - node.transform.up * 0.52f);
                }

                if (startNode != node)
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
                    if (!LevelEditor.inactiveNodesEditing)
                    {
                        endNode = node;


                        if (startNode != null)
                        {
                            startNode.CalculatePathDirection(endNode);
                            endNode.CalculatePathDirection(startNode);
                        }
                        else
                        {
                            Debug.Log("new path started");
                            LevelEditor.ReCalculateAllMazeCubes();
                        }

                        if (startNode == endNode)
                        {
                            startNode = null;
                            endNode = null;
                        }
                        else
                        {
                            endNode = null;
                            startNode = node;
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
                            startNode = node;
                        }
                    }
                }
            }
        }

        //TODO: add snaping feature
        public void CreateSetMazePositionHandle()
        {
            EditorGUI.BeginChangeCheck();
            Vector3 newTargetPosition = Handles.PositionHandle(maze.transform.position, maze.transform.rotation);
            if (EditorGUI.EndChangeCheck())
            {
                if (Vector3.Distance(newTargetPosition, maze.transform.position) == 1)
                {
                    maze.transform.position = newTargetPosition;
                }
            }
        }

        public void CreateSetMazePivotHandle()
        {
            EditorGUI.BeginChangeCheck();
            Vector3 newTargetPosition = Handles.PositionHandle(maze.transform.position, maze.transform.rotation);
            if (EditorGUI.EndChangeCheck())
            {
                if (Vector3.Distance(newTargetPosition, maze.transform.position) == 1)
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