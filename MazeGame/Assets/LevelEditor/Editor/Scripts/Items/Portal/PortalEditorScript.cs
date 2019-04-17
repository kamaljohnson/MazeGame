using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Game.Items;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace LevelEditor.Items.Interactable.Portal
{
    /*Scene editor script for editing portals
     * 
     */
    [CustomEditor(typeof(Game.Items.Interactable.Portal.Portal))]
    public class PortalEditorScript : Editor, ITem, ITemButtonInteraction
    {
        private Game.Items.Interactable.Portal.Portal _portal;
        private bool _buttonMappingMode;
        
        private void OnEnable()
        {
            Init();
        }

        private void OnSceneGUI()
        {
            DrawAddButtonLinkHandle();
            DrawDeletionHandle();
            if (_buttonMappingMode)
            {
                DrawAllUnUsedButtonHandles();
            }
        }

        public void DrawAddButtonLinkHandle()
        {
            if (!_buttonMappingMode)
            {
                Handles.color = new Color(0.14f, 0.93f, 1f);
            }
            else
            {
                Handles.color = new Color(1f, 0.73f, 0.18f);                
            }
            if (Handles.Button(_portal.transform.position + _portal.transform.up, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
            {
                _buttonMappingMode = !_buttonMappingMode;
            }
        }

        public void DrawAllUnUsedButtonHandles()
        {
            foreach (var activator in LevelEditor.AllItems[(int)ItemCategories.Activator])
            {
                var button = activator.GetComponent<Game.Items.Activators.Button.Button>();
                Handles.color = new Color(0.25f, 1f, 0.67f);                
                if (!button.itemSet)
                {
                    if (Handles.Button(activator.transform.position + activator.transform.up * 0.2f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
                    {
                        AddButtonLink(button);
                    }
                }
            }
        }
        
        public void DrawDeletionHandle()
        {
            Handles.color = new Color(1f, 0f, 0.07f);
            if (Handles.Button(_portal.transform.position + _portal.transform.up * 0.7f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
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
            LevelEditor.ReCalculateAllItems();
        }

        public bool CheckValuesSet()
        {
            return true;
        }

        public void AddButtonLink(Game.Items.Activators.Button.Button button)
        {
            button.itemSet = true;
            button.interactionItem = _portal;

            List<int> allIds = new List<int>();
            int tempId = 0;
            
            List<List<GameObject>> allItems;
            LevelEditor.GetAllMazeItems(out allItems);
            
            for (int index = 0; index < allItems[(int)ItemCategories.Interactable].Count; index++)
            {
                if (allItems[(int) ItemCategories.Interactable][index].GetComponent<IInteractables>().GetInteractableId() != 0)
                {
                    allIds.Add(allItems[(int) ItemCategories.Interactable][index].GetComponent<IInteractables>().GetInteractableId());
                }
            }
            allIds.Sort();
            foreach (var id in allIds)
            {
                if (!tempId.Equals(id))
                {
                    tempId = id;
                }

                tempId++;
            }

            if (tempId == 0)
            {
                tempId = 1;
            }

            _portal.interactableId = tempId;
            button.interactionItemId = tempId;
        }

        public void EditButtonLink()
        {

        }

        public void RemoveButtonLink(int buttonId)
        {

        }

    }
}
