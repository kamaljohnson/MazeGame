﻿using System.Collections;
using System.Collections.Generic;
using Game.Items.Activators.Button;
using UnityEditor;
using UnityEngine;

namespace LevelEditor.Items.Interactable.Gate
{
    [CustomEditor(typeof(Game.Items.Interactable.Gate.Gate))]
    public class GateEditorScript : Editor, ITem, ITemButtonInteraction
    {
        private Game.Items.Interactable.Gate.Gate _gate;
        private bool _buttonMappingMode;
        
        private void OnEnable()
        {
            Init();
        }

        private void OnSceneGUI()
        {
            DrawAddButtonLinkHandle();
            DrawDeletionHandle();
            DrawOrintationButtonHandles();
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
            if (Handles.Button(_gate.transform.position + _gate.transform.up, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
            {
                _buttonMappingMode = !_buttonMappingMode;
            }
        }
        
        public void DrawDeletionHandle()
        {
            Handles.color = new Color(1f, 0f, 0.07f);
            if (Handles.Button(_gate.transform.position + _gate.transform.up * 0.7f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
            {
                if (!EditorUtility.DisplayDialog("Warning!!",
                    "This will delete the item permenently", "Cancel", "Continue"))
                {
                    RemoveItem();
                }
            }
        }
        
        public void DrawAllUnUsedButtonHandles()
        {
            foreach (var activator in LevelEditor.AllItems[(int)ItemCategories.Activator])
            {
                var button = activator.GetComponent<Button>();
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

        public void DrawOrintationButtonHandles()
        {
            
        }
        
        public void Init()
        {
            _gate = (Game.Items.Interactable.Gate.Gate) target;
            _gate.name = "Gate";
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
            LevelEditor.AllItems[(int) ItemCategories.Interactable].Remove(_gate.gameObject);
            DestroyImmediate(_gate.gameObject);
            LevelEditor.ReCalculateAllItems();
        }

        public bool CheckValuesSet()
        {
            throw new System.NotImplementedException();
        }

        public void AddButtonLink(Button button)
        {
            button.itemSet = true;
            button.interactionItem = _gate;
        }

        public void EditButtonLink()
        {
            throw new System.NotImplementedException();
        }

        public void RemoveButtonLink(int buttonId)
        {
            throw new System.NotImplementedException();
        }
    }
}
