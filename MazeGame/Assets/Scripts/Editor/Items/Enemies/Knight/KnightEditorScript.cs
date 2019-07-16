using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LevelEditor.Items.Enemies.Knight
{   
    [CustomEditor(typeof(Game.Items.Enemies.Knight.Knight))]
    public class KnightEditorScript : Editor, ITem
    {
        private Game.Items.Enemies.Knight.Knight _knight;
        
        private void OnEnable()
        {
            Init();
        }

        private void OnSceneGUI()
        {
            DrawDeletionHandle();
        }

        public void DrawDeletionHandle()
        {
            Handles.color = new Color(1f, 0f, 0.07f);
            if (Handles.Button(_knight.transform.position + _knight.transform.up * 0.7f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
            {
                if (!EditorUtility.DisplayDialog("Warning!!",
                    "This will delete the item parmanently", "Cancel", "Continue"))
                {
                    RemoveItem();
                }
            }
        }
        
        public void Init()
        {
            _knight = (Game.Items.Enemies.Knight.Knight) target;
            _knight.name = "Knight";
        }

        public void AddItem()
        {
            throw new System.NotImplementedException();
        }

        public void EditItem()
        {
            throw new System.NotImplementedException();
        }

        public void RemoveItem()
        {
            LevelEditor.AllItems[(int) ItemCategories.Intractable].Remove(_knight.gameObject);
            DestroyImmediate(_knight.gameObject);
            LevelEditor.ReCalculateAllItems();        }

        public bool CheckValuesSet()
        {
            throw new System.NotImplementedException();
        }
    }
}
