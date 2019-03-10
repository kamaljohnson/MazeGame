using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevelEditor.Items.Portal
{
    [CustomEditor(typeof(Game.Items.Portal.Portal))]
    public class PortalEditorScript : Editor, Item, ItemButtonInteraction
    {
        public void OnEnable()
        {
            Game.Items.Portal.Portal portal = (Game.Items.Portal.Portal)target;
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
