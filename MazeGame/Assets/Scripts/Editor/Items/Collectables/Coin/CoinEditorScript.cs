using UnityEditor;
using UnityEngine;

namespace LevelEditor.Items.Collectables.Coin
{
    [CustomEditor(typeof(Game.Items.Collectables.Coin.Coin))]
    public class CoinEditorScript : Editor, ITem
    {
        private Game.Items.Collectables.Coin.Coin _coin;

        private void OnEnable()
        {
            _coin = (Game.Items.Collectables.Coin.Coin) target;
            _coin.name = "Coin";
        }

        private void OnSceneGUI()
        {
            DrawDeletionHandle();
        }

        public void DrawDeletionHandle()
        {
            Handles.color = new Color(1f, 0f, 0.07f);
            if (Handles.Button(_coin.transform.position + _coin.transform.up * 0.7f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
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
            LevelEditor.AllItems[(int) ItemCategories.Intractable].Remove(_coin.gameObject);
            DestroyImmediate(_coin.gameObject);
            LevelEditor.ReCalculateAllItems();
        }

        public bool CheckValuesSet()
        {
            return true;
        }
    }
}
