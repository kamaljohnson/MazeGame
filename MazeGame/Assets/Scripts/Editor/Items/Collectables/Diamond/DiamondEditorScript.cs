using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LevelEditor.Items.Collectables.Diamond
{
    [CustomEditor(typeof(Game.Items.Collectables.Diamond.Diamond))]
    public class CoinEditorScript : Editor, ITem
    {
        private Game.Items.Collectables.Diamond.Diamond diamond;

        private void OnEnable()
        {
            diamond = (Game.Items.Collectables.Diamond.Diamond) target;
            diamond.name = "Diamond";
        }

        private void OnSceneGUI()
        {
            DrawDeletionHandle();
        }

        public void DrawDeletionHandle()
        {
            Handles.color = new Color(1f, 0f, 0.07f);
            if (Handles.Button(diamond.transform.position + diamond.transform.up * 0.7f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
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
        }

        public void EditItem()
        {
        }

        public void RemoveItem()
        {
            LevelEditor.AllItems[(int) ItemCategories.Intractable].Remove(diamond.gameObject);
            DestroyImmediate(diamond.gameObject);
            LevelEditor.ReCalculateAllItems();
        }

        public bool CheckValuesSet()
        {
            return true;
        }
    }
}
