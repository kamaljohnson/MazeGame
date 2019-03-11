using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevelEditor.Items.Interactable.Portal
{
    /*Scene editor script for editing portals
     * 
     */
    [CustomEditor(typeof(Game.Items.Interactable.Portal.Portal))]
    public class PortalEditorScript : Editor, ITem, ItemButtonInteraction
    {
        public bool ItemSet;    //is the item set with values

        public void OnEnable()
        {
            Game.Items.Interactable.Portal.Portal portal = (Game.Items.Interactable.Portal.Portal)target;
        }

        public void OnSceneGUI()
        {
            Debug.Log("portal editing activated");
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
