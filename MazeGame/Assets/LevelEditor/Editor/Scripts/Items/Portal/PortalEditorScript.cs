using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevelEditor.Items.Portal
{
    public class PortalEditorScript : Editor
    {
        
    }

    public class PortalEditingWindow : EditorWindow, Item, ItemButtonInteraction
    {

        public void OnGUI()      //the sub editor window for the corresponding item
        {
            GUILayout.Label("Portal editing");
        }

        #region Item Interface Functions
        public void Init()
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }
        #endregion

        #region Button Interface Functions
        public void AddButton()
        {
            throw new System.NotImplementedException();
        }

        public void EditButton()
        {
            throw new System.NotImplementedException();
        }

        public void RemoveButton(int buttonID)
        {
            throw new System.NotImplementedException();
        }
        #endregion

    }
}
