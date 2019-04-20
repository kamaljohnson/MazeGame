using System.Collections;
using System.Collections.Generic;
using Game;
using Game.Items;
using Game.Items.Activators.Button;
using UnityEditor;
using UnityEngine;

namespace LevelEditor.Items.Intractable.Gate
{
    [CustomEditor(typeof(Game.Items.Intractable.Gate.Gate))]
    public class GateEditorScript : Editor, ITem, ITemButtonInteraction
    {
        private Game.Items.Intractable.Gate.Gate _gate;
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
            if (Handles.Button(_gate.transform.position + _gate.transform.forward, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
            {
                _buttonMappingMode = !_buttonMappingMode;
            }
        }
        
        public void DrawDeletionHandle()
        {
            Handles.color = new Color(1f, 0f, 0.07f);
            if (Handles.Button(_gate.transform.position + _gate.transform.forward * 0.6f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
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
            Handles.color = new Color(0.55f, 0.64f, 1f);
            
            if (Handles.Button(_gate.transform.position + _gate.transform.forward * 0.6f - _gate.transform.up * 0.3f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
            {
                _gate.gateDirection = Direction.Down;

                Debug.Log("up");
                _gate.transform.GetChild(0).localPosition = new Vector3(0, 0, -0.5f +1/12f);
                _gate.transform.GetChild(0).localPosition += new Vector3(0,1, 0) * -(0.5f + 1/12f);                    
                _gate.transform.GetChild(0).localEulerAngles = new Vector3(270, 90, 90);                    
                    
                _gate.transform.GetChild(1).localPosition = new Vector3(0, 0, -0.5f +1/12f);
                _gate.transform.GetChild(1).localPosition += new Vector3(0,1, 0) * -(0.5f + 1/12f);                    
                _gate.transform.GetChild(1).localEulerAngles = new Vector3(270, 90, 90); 
            }

            if (Handles.Button(_gate.transform.position + _gate.transform.forward * 0.6f + _gate.transform.up * 0.3f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
            {
                _gate.gateDirection = Direction.Up;       
                _gate.transform.GetChild(0).localPosition = new Vector3(0, 0, -0.5f +1/12f);                
                _gate.transform.GetChild(0).localPosition += new Vector3(0,1, 0) * (0.5f + 1/12f);     
                _gate.transform.GetChild(0).localEulerAngles = new Vector3(90, 90, 90);        
                    
                _gate.transform.GetChild(1).localPosition = new Vector3(0, 0, -0.5f +1/12f);                
                _gate.transform.GetChild(1).localPosition += new Vector3(0,1, 0) * (0.5f + 1/12f);     
                _gate.transform.GetChild(1).localEulerAngles = new Vector3(90, 90, 90);   
                Debug.Log("down");
            }

            if (Handles.Button(_gate.transform.position + _gate.transform.forward * 0.6f - _gate.transform.right * 0.3f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
            {
                _gate.gateDirection = Direction.Left;      
                _gate.transform.GetChild(0).localPosition = new Vector3(0, 0, -0.5f +1/12f);
                _gate.transform.GetChild(0).localPosition += new Vector3(1,0, 0) * -(0.5f + 1/12f);                    
                _gate.transform.GetChild(0).localEulerAngles = new Vector3(0, 90, 90);                    
                    
                _gate.transform.GetChild(1).localPosition = new Vector3(0, 0, -0.5f +1/12f);
                _gate.transform.GetChild(1).localPosition += new Vector3(1,0, 0) * -(0.5f + 1/12f);                    
                _gate.transform.GetChild(1).localEulerAngles = new Vector3(0, 90, 90);    
  
                Debug.Log("right");
            }

            if (Handles.Button(_gate.transform.position + _gate.transform.forward * 0.6f + _gate.transform.right * 0.3f, Quaternion.identity, 0.15f, 0.15f, Handles.CubeCap))
            {
                _gate.gateDirection = Direction.Right;
                _gate.transform.GetChild(0).localPosition = new Vector3(0, 0, -0.5f +1/12f);
                _gate.transform.GetChild(0).localPosition += new Vector3(1,0, 0) * (0.5f + 1/12f);                    
                _gate.transform.GetChild(0).localEulerAngles = new Vector3(180, 90, 90);                    
                    
                _gate.transform.GetChild(1).localPosition = new Vector3(0, 0, -0.5f +1/12f);
                _gate.transform.GetChild(1).localPosition += new Vector3(1,0, 0) * (0.5f + 1/12f);                    
                _gate.transform.GetChild(1).localEulerAngles = new Vector3(180, 90, 90);    
                Debug.Log("left");
            }
        }
        
        public void Init()
        {
            _gate = (Game.Items.Intractable.Gate.Gate) target;
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
            LevelEditor.AllItems[(int) ItemCategories.Intractable].Remove(_gate.gameObject);
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

            button.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = _gate.gateColors[_gate.colorId];

            //color coding the gate-button pair
            for (int i = 0; i < 2; i++)
            {
                for (var j = 0; j < _gate.transform.GetChild(i).childCount; j++)
                {
                    _gate.transform.GetChild(i).GetChild(j).GetComponent<MeshRenderer>().material.color = _gate.gateColors[_gate.colorId];
                }
            }
//            _gate.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = _gate.gateColors[_gate.colorId];
//            _gate.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.color = _gate.gateColors[_gate.colorId];
//            _gate.transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material.color = _gate.gateColors[_gate.colorId];
            
            List<int> allIds = new List<int>();
            int tempId = 0;
            
            List<List<GameObject>> allItems;
            LevelEditor.GetAllMazeItems(out allItems);
            
            for (int index = 0; index < allItems[(int)ItemCategories.Intractable].Count; index++)
            {
                if (allItems[(int) ItemCategories.Intractable][index].GetComponent<IIntractables>().GetIntractableId() != 0)
                {
                    allIds.Add(allItems[(int) ItemCategories.Intractable][index].GetComponent<IIntractables>().GetIntractableId());
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

            _gate.intractableId = tempId;
            button.interactionItemId = tempId;
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
