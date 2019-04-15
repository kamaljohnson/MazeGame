using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace LevelEditor.Items.Interactable.Portal
{
    /*Scene editor script for editing portals
     * 
     */
    [CustomEditor(typeof(Game.Items.Interactable.Portal.Portal))]
    public class PortalEditorScript : Editor, ITem, ItemButtonInteraction
    {
        private Game.Items.Interactable.Portal.Portal _portal;
        
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
            if (Handles.Button(_portal.transform.position + _portal.transform.up, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
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
            _portal = (Game.Items.Interactable.Portal.Portal)target;
            _portal.name = "Portal";
            _portal.levelId = int.Parse(SceneManager.GetActiveScene().name.Split(' ')[1]);
            _portal.mazeId = int.Parse(Selection.activeGameObject.transform.parent.name.Split(' ')[1]);
            _portal.portalName = $"{_portal.levelId.ToString()}:{_portal.mazeId.ToString()}:{_portal.portalId.ToString()}";
            if (_portal.destinationPortalName == "")
            {
                _portal.destinationPortalName = "levelID:mazeID:portalID";
            }

            _portal.itemSet = true;
        }

        public void AddItem()
        {

        }

        public void EditItem()
        {
            
        }

        public void RemoveItem()
        {
            LevelEditor.AllItems[(int) ItemCategories.Interactable].Remove(_portal.gameObject);
            DestroyImmediate(_portal.gameObject);
        }

        public bool CheckValuesSet()
        {
            return true;
        }

        public void AddButtonLink()
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
