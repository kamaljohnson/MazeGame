using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LevelEditor.Items.Enemies.Hammer
{
    [CustomEditor(typeof(Game.Items.Enemies.Hammer.Hammer))]
    public class HammerEditorScript : Editor, ITem
    {        
        private Game.Items.Enemies.Hammer.Hammer _hammer;
   
        private void OnEnable()
        {
            Init();
        }
        
        private void OnSceneGUI()
        {
            DrawDeletionHandle();
        }

        private void DrawDeletionHandle()
        {
            Handles.color = new Color(1f, 0f, 0.07f);
            if (Handles.Button(_hammer.transform.position + _hammer.transform.up * 0.7f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
            {
                if (!EditorUtility.DisplayDialog("Warning!!",
                    "This will delete the item permanently", "Cancel", "Continue"))
                {
                    RemoveItem();
                }
            }
        }
        
        public void Init()
        {
            _hammer = (Game.Items.Enemies.Hammer.Hammer)target;
            _hammer.name = "Hammer";
            _hammer.itemSet = true;
        }

        public void AddItem()
        {
            
        }

        public void EditItem()
        {
            throw new System.NotImplementedException();
        }

        public void RemoveItem()
        {
            LevelEditor.AllItems[(int) ItemCategories.Enemy].Remove(_hammer.gameObject);
            DestroyImmediate(_hammer.gameObject);
            LevelEditor.ReCalculateAllItems();        }

        public bool CheckValuesSet()
        {
            return false;
        }
    }
}
