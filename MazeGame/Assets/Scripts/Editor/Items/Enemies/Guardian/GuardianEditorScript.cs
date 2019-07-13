using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LevelEditor.Items.Enemies.Guardian
{
    [CustomEditor(typeof(Game.Items.Enemies.Guardian.Guardian))]
    public class GuardianEditorScript : Editor, ITem
    {
        private Game.Items.Enemies.Guardian.Guardian _guardian;
        private bool _showPathPositions;
        
        private void OnEnable()
        {
            Init();
        }

        private void OnSceneGUI()
        {
            DrawPathSelectionHandle();

            if (_showPathPositions)
            {
                DrawAllLocationHandles();
            }
            else
            {
                DrawDeletionHandle();
            }
        }

        private void DrawDeletionHandle()
        {
            Handles.color = new Color(1f, 0f, 0.07f);
            if (Handles.Button(_guardian.transform.position + _guardian.transform.up * 0.7f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
            {
                if (!EditorUtility.DisplayDialog("Warning!!",
                    "This will delete the item permanently", "Cancel", "Continue"))
                {
                    RemoveItem();
                }
            }
        }

        private void DrawPathSelectionHandle()
        {
            Handles.color = new Color(0.37f, 0.96f, 1f);
            if (Handles.Button(_guardian.transform.position + _guardian.transform.up * 0.5f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
            {
                _showPathPositions = !_showPathPositions;
            }
        }
        
        private void DrawAllLocationHandles()
        {
            for (int i = 0; i < LevelEditor.CurrentMaze.transform.childCount; i++)
            {
                GameObject mazeCube = LevelEditor.CurrentMaze.transform.GetChild(i).gameObject;
                foreach (var offset in Helper.Vector3Directions)
                {
                    var positionFlag = true;
                    if (!Physics.Raycast(mazeCube.transform.position, offset, out _, 1f))
                    {
                        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
                        foreach (var location in _guardian.locations)
                        {
                            if (location == mazeCube.transform.position + offset * 0.5f)
                            {
                                Handles.color = new Color(0.78f, 1f, 0.09f);
                                if (Handles.Button(mazeCube.transform.position + offset * 0.55f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
                                {
                                    
                                }
                                positionFlag = false;
                                break;
                            }
                        }

                        if (positionFlag)
                        {
                            Handles.color = new Color(0.54f, 0.25f, 1f);
                            if (Handles.Button(mazeCube.transform.position + offset * 0.55f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
                            {
                                if (_guardian.locations.Count == 0)
                                {
                                    _guardian.locations.Add(_guardian.transform.position + offset * 0.5f);
                                }
                                _guardian.locations.Add(mazeCube.transform.position + offset * 0.5f);
                            }
                        }
                    }
                }
            }
        }
        
        public void Init()
        {
            _guardian = (Game.Items.Enemies.Guardian.Guardian)target;
            _guardian.name = "Guardian";
        }

        public void AddItem()
        {
            
        }

        public void EditItem()
        {
            
        }

        public void RemoveItem()
        {
            LevelEditor.AllItems[(int) ItemCategories.Enemy].Remove(_guardian.gameObject);
            DestroyImmediate(_guardian.gameObject);
            LevelEditor.ReCalculateAllItems();
        }

        public bool CheckValuesSet()
        {
            return false;
        }
    }
}
