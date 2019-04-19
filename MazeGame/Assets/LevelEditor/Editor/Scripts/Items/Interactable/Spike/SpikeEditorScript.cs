using System.Collections;
using System.Collections.Generic;
using Game.Items.Activators.Button;
using UnityEditor;
using UnityEngine;

namespace LevelEditor.Items.Intractable.Spike
{
    [CustomEditor(typeof(Game.Items.Intractable.Spike.Spike))]
    public class SpikeEditorScript : Editor, ITem, ITemButtonInteraction
    {
        private Game.Items.Intractable.Spike.Spike _spike;

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
            if (Handles.Button(_spike.transform.position + _spike.transform.up * 0.7f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
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
            _spike = (Game.Items.Intractable.Spike.Spike)target;
            _spike.name = "Spike";

            _spike.itemSet = true;
        }

        public void AddItem()
        {
            
        }

        public void EditItem()
        {
            
        }

        public void RemoveItem()
        {
            LevelEditor.AllItems[(int) ItemCategories.Intractable].Remove(_spike.gameObject);
            DestroyImmediate(_spike.gameObject);
            LevelEditor.ReCalculateAllItems();
        }

        public bool CheckValuesSet()
        {
            return false;
        }

        public void AddButtonLink(Button button)
        {
            
        }

        public void EditButtonLink()
        {
            
        }

        public void RemoveButtonLink(int buttonID)
        {
            
        }
    }

}
