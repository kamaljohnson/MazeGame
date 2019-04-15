using System.Collections;
using System.Collections.Generic;
using Game.Items;
using UnityEditor;
using UnityEngine;

namespace LevelEditor.Items.Activator.Button
{
    [CustomEditor(typeof(Game.Items.Activators.Button.Button))]
    public class ButtonEditorScript : Editor, ITem
    {
        private Game.Items.Activators.Button.Button _button;

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
            Handles.color = new Color(1f, 0.29f, 0.3f);
            if (Handles.Button(_button.transform.position + _button.transform.up * 0.2f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
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
            _button = (Game.Items.Activators.Button.Button) target;
            _button.name = "Button";
        }

        public void AddItem()
        {
            
        }

        public void EditItem()
        {
            
        }

        public void RemoveItem()
        {
            LevelEditor.AllItems[(int) ItemCategories.Interactable].Remove(_button.gameObject);
            DestroyImmediate(_button.gameObject);
        }

        public bool CheckValuesSet()
        {
            return true;
        }
    }
}
