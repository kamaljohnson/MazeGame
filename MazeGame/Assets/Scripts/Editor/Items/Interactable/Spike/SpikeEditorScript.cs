using System.Collections;
using System.Collections.Generic;
using Game.Items.Activators.Button;
using UnityEditor;
using UnityEngine;

namespace LevelEditor.Items.Intractable.Spike
{
    [CustomEditor(typeof(Game.Items.Intractable.Spike.Spike))]
    public class SpikeEditorScript : Editor, ITem, ITemButtonInteraction
    {
        private Game.Items.Intractable.Spike.Spike _spike;
        private static int _currentGroupId;
        private static int _currentSpikeId;

        private void OnEnable()
        {
            Init();
        }

        private void OnSceneGUI()
        {
            DrawDeletionHandle();
            DrawGroupingHandles();
        }

        private void DrawDeletionHandle()
        {
            Handles.color = new Color(1f, 0f, 0.07f);
            if (Handles.Button(_spike.transform.position + _spike.transform.up * 0.7f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
            {
                if (!EditorUtility.DisplayDialog("Warning!!",
                    "This will delete the item permanently", "Cancel", "Continue"))
                {
                    RemoveItem();
                }
            }
        }

        private void DrawGroupingHandles()
        {
            foreach (var intractableSpike in LevelEditor.AllItems[(int)ItemCategories.Intractable])
            {
                var spike = intractableSpike.GetComponent<Game.Items.Intractable.Spike.Spike>();
                if (spike == null)
                {
                    continue;
                }
                Handles.color = new Color(0.25f, 1f, 0.67f);                
                if (!spike.itemSet)
                {
                    if (Handles.Button(intractableSpike.transform.position + intractableSpike.transform.up * 0.2f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
                    {
                        spike.itemSet = true;
                        spike.groupId = _currentGroupId;
                        spike.spikeId = _currentSpikeId;
                        spike.type = _spike.type;
                        _currentSpikeId++;
                    }
                }
            }
        }

        public void Init()
        {
            _spike = (Game.Items.Intractable.Spike.Spike)target;
            _spike.name = "Spike";
            _currentGroupId++;
            _currentSpikeId = 0;
        }

        public void AddItem()
        {
            
        }

        public void EditItem()
        {
            
        }

        public void RemoveItem()
        {
            LevelEditor.AllItems[(int) ItemCategories.Intractable].Remove(_spike.gameObject);
            DestroyImmediate(_spike.gameObject);
            LevelEditor.ReCalculateAllItems();
        }

        public bool CheckValuesSet()
        {
            return false;
        }

        public void AddButtonLink(Button button)
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
