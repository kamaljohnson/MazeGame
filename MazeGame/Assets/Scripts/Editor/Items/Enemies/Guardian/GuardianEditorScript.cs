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
        
        private void OnEnable()
        {
            Init();
        }

        private void OnSceneGUI()
        {
            DrawDeletionHandle();
            DrawPathSelectionHandle();
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
                DrawAllLocationHandles();
            }
        }
        
        private void DrawAllLocationHandles()
        {
            for (int i = 0; i < LevelEditor.CurrentMaze.transform.childCount; i++)
            {
                GameObject mazeCube = LevelEditor.CurrentMaze.transform.GetChild(i).gameObject;
                RaycastHit hit;
                foreach (var offset in Helper.Vector3Directions)
                {
                    if (!Physics.Raycast(mazeCube.transform.position, offset, out hit, 1f))
                    {
    
                        Handles.color = new Color(0.54f, 0.25f, 1f);
                        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
                        if (Handles.Button(mazeCube.transform.position + (offset * 0.55f), Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
                        {
                            
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
