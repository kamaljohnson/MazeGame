using System.Collections;
using System.Collections.Generic;
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
        public bool ItemSet;    //is the item set with values
        private Game.Items.Interactable.Portal.Portal _portal;
        
        public void OnEnable()
        {
            _portal = (Game.Items.Interactable.Portal.Portal)target;
            _portal.name = "Portal";
            _portal.LevelId = int.Parse(SceneManager.GetActiveScene().name.Split(' ')[1]);
            _portal.MazeId = int.Parse(Selection.activeGameObject.transform.parent.name.Split(' ')[1]);
            _portal.PortalName = $"{_portal.LevelId.ToString()}:{_portal.MazeId.ToString()}:{_portal.PortalId.ToString()}";
            if (_portal.DestinationPortalName == "")
            {
                _portal.DestinationPortalName = "levelID:mazeID:portalID";
            }
//            Selection.SetActiveObjectWithContext(LevelEditor.CurrentMaze, LevelEditor.CurrentMaze);
        }

        public void OnSceneGUI()
        {
            
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

        }

        public bool CheckValuesSet()
        {
            return ItemSet;
        }

        public void AddButton()
        {

        }

        public void EditButton()
        {

        }

        public void RemoveButton(int buttonID)
        {

        }

    }
}
