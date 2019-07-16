using UnityEditor;
using UnityEngine;

namespace LevelEditor.Items.Path.Fire
{
    
    [CustomEditor(typeof(Game.Items.Path.Fire.Fire))]
    public class FireEditorScript : Editor, ITem
    {
        private Game.Items.Path.Fire.Fire _fire;
            
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
            if (Handles.Button(_fire.transform.position + _fire.transform.up * 0.7f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
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
            _fire = (Game.Items.Path.Fire.Fire) target;
            _fire.name = "Fire";
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
            LevelEditor.AllItems[(int) ItemCategories.Intractable].Remove(_fire.gameObject);
            DestroyImmediate(_fire.gameObject);
            LevelEditor.ReCalculateAllItems();
                
        }

        public bool CheckValuesSet()
        {
            throw new System.NotImplementedException();

        }
    }
}
