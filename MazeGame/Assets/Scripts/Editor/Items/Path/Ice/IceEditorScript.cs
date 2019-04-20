using System;
using UnityEditor;
using UnityEngine;

namespace LevelEditor.Items.Path.Ice
{
    [CustomEditor(typeof(Game.Items.Path.Ice.Ice))]
    public class IceEditorScript : Editor, ITem
    {
        private Game.Items.Path.Ice.Ice _ice;
        
        private void OnEnable()
        {
            _ice = (Game.Items.Path.Ice.Ice) target;
        }

        private void OnSceneGUI()
        {
            DrawDeletionHandle();
        }

        public void DrawDeletionHandle()
        {
            Handles.color = new Color(1f, 0f, 0.07f);
            if (Handles.Button(_ice.transform.position + _ice.transform.up * 0.7f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
            {
                if (!EditorUtility.DisplayDialog("Warning!!",
                    "This will delete the item permenently", "Cancel", "Continue"))
                {
                    RemoveItem();
                }
            }
        }
        
        public void Init()
        {
            
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
            LevelEditor.AllItems[(int) ItemCategories.Intractable].Remove(_ice.gameObject);
            DestroyImmediate(_ice.gameObject);
            LevelEditor.ReCalculateAllItems();        }

        public bool CheckValuesSet()
        {
            throw new System.NotImplementedException();
        }
    }

}
