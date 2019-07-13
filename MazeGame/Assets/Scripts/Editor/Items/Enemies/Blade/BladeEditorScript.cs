using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LevelEditor.Items.Enemies.Blade
{
    [CustomEditor(typeof(Game.Items.Enemies.Blade.Blade))]
    public class BladeEditorScript : Editor, ITem
    {
         private Game.Items.Enemies.Blade.Blade _blade;
        private bool _showPathPositions;
        private List<Vector3> _locations = new List<Vector3>();
        
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
                DrawOrientationHandles();
            }

            if (_blade.locations.Count == 0)
            {
                _locations = new List<Vector3>();
            }
        }

        private void DrawDeletionHandle()
        {
            Handles.color = new Color(1f, 0f, 0.07f);
            if (Handles.Button(_blade.transform.position + _blade.transform.up * 0.7f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
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
            if (Handles.Button(_blade.transform.position + _blade.transform.up * 0.5f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
            {
                _showPathPositions = !_showPathPositions;
            }
        }

        private void DrawOrientationHandles()
        {
            Handles.color = new Color(0.92f, 0.56f, 1f);
            if (Handles.Button(_blade.transform.position + _blade.transform.up * 0.3f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
            {
                _blade.transform.localEulerAngles = _blade.transform.localEulerAngles == Vector3.zero ? new Vector3(0, 90, 0) : Vector3.zero;
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
                        foreach (var location in _locations)
                        {
                            if (location == mazeCube.transform.localPosition + offset * 0.5f)
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
                                if (_blade.locations.Count == 0)
                                {
                                    _blade.locations.Add(_blade.transform.parent.localPosition + offset);
                                }
                                _blade.locations.Add(mazeCube.transform.localPosition + offset);
                                _locations.Add(mazeCube.transform.localPosition + offset * 0.5f);
                            }
                        }
                    }
                }
            }
        }
        
        public void Init()
        {
            _blade = (Game.Items.Enemies.Blade.Blade)target;
            _blade.name = "Blade";
        }

        public void AddItem()
        {
            
        }

        public void EditItem()
        {
            
        }

        public void RemoveItem()
        {
            LevelEditor.AllItems[(int) ItemCategories.Enemy].Remove(_blade.gameObject);
            DestroyImmediate(_blade.gameObject);
            LevelEditor.ReCalculateAllItems();
        }

        public bool CheckValuesSet()
        {
            return false;
        }
    }
}
