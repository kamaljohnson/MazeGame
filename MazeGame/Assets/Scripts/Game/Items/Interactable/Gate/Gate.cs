using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.Items.Interactable.Gate
{
    public class Gate : MonoBehaviour, IInteractables, IItems
    {
        public int interactableId;    //this id is used to link buttons
        
        public bool itemSet;    //is the item set with values

        public Direction gateDireciton;    //the direction of the gate from the node

        public GameObject gatePrefab;

        private List<GameObject> _allGateWalls;
        //rendering status
        //sides
        public bool rrender;
        public bool lrender;
        public bool urender;
        public bool drender;
        
        [Header("State Properties")]
        [Tooltip("o    -    ON\n" +
                 "f    -    OFF\n" +
                 "u    -    UP\n" +
                 "d    -    DOWN\n" +
                 "r    -    RIGHT\n" +
                 "l    -    LEFT\n" +
                 "c    -    CLK-ROT\n" +
                 "a    -    A-CLK-ROT\n" +
                 "#num -    x-times")]
        public string linkedButtonOnState = "";
        public string linkedButtonOffState = "";
        
        public void CalculateGateRenderPath()
        {
            RaycastHit hit;

            rrender = false;
            lrender = false;
            urender = false;
            drender = false;

            switch (gateDireciton)
            {
                case Direction.Up:
                    urender = true;
                    break;
                case Direction.Left:
                    lrender = true;
                    break;
                case Direction.Down:
                    drender = true;
                    break;
                case Direction.Right:
                    rrender = true;
                    break;
            }
            
            //TODO: create a new ex-direction down wall at the edge

        }

        public void RenderGate()
        {
            transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            _allGateWalls = new List<GameObject>();
            
            var offset = 1 / 2f - 1 / 12f;
            var heightOffset = 1 / 12f;
            
            var externalOffset = 1 / 2f + 1 / 12f;
            var downOffset = externalOffset - offset;
            
            var w_size = 4 / 6f;
            var c_size = 1 / 6f;
            var h_size = 1 / 6f;
            
            //rendering walls
            if (rrender)
            {
                GameObject tempobj = Instantiate(gatePrefab, transform.position - transform.forward * 0.5f + transform.right * offset + transform.forward * heightOffset, Quaternion.identity, transform);
                tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                tempobj.name = "r";
                _allGateWalls.Add(tempobj);
                
                tempobj = Instantiate(gatePrefab, transform.position - transform.forward * 0.5f + transform.right * externalOffset + transform.forward * heightOffset, Quaternion.identity, transform);
                tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                tempobj.name = "er";
                _allGateWalls.Add(tempobj);
                
                tempobj = Instantiate(gatePrefab, transform.position - transform.forward * 0.5f + transform.right * externalOffset + transform.forward * heightOffset - transform.forward * downOffset , Quaternion.identity, transform);
                tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                tempobj.name = "er";
                _allGateWalls.Add(tempobj);
            }
            if (lrender)
            {
                GameObject tempobj = Instantiate(gatePrefab, transform.position - transform.forward * 0.5f - transform.right * offset + transform.forward * heightOffset, Quaternion.identity, transform);
                tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                tempobj.name = "l";
                _allGateWalls.Add(tempobj);
                
                tempobj = Instantiate(gatePrefab,
                    transform.position - transform.forward * 0.5f - transform.right * externalOffset +
                    transform.forward * heightOffset, Quaternion.identity, transform);
                tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                tempobj.name = "el";
                _allGateWalls.Add(tempobj);
                
                tempobj = Instantiate(gatePrefab,
                    transform.position - transform.forward * 0.5f - transform.right * externalOffset +
                    transform.forward * heightOffset - transform.forward * downOffset , Quaternion.identity, transform);
                tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                tempobj.name = "el";
                _allGateWalls.Add(tempobj);
            }
            if (urender)
            {
                GameObject tempobj = Instantiate(gatePrefab, transform.position - transform.forward * 0.5f + transform.up * offset + transform.forward * heightOffset, Quaternion.identity, transform);
                tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                tempobj.name = "u";
                _allGateWalls.Add(tempobj);
                
                tempobj = Instantiate(gatePrefab,
                    transform.position - transform.forward * 0.5f + transform.up * externalOffset +
                    transform.forward * heightOffset, Quaternion.identity, transform);
                tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                tempobj.name = "eu";
                _allGateWalls.Add(tempobj);
                
                tempobj = Instantiate(gatePrefab,
                    transform.position - transform.forward * 0.5f + transform.up * externalOffset +
                    transform.forward * heightOffset - transform.forward * downOffset , Quaternion.identity, transform);
                tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                tempobj.name = "eu";
                _allGateWalls.Add(tempobj);
            }
            if (drender)
            {
                GameObject tempobj = Instantiate(gatePrefab, transform.position - transform.forward * 0.5f - transform.up * offset + transform.forward * heightOffset, Quaternion.identity, transform);
                tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                tempobj.name = "d";
                _allGateWalls.Add(tempobj);
                
                tempobj = Instantiate(gatePrefab,
                    transform.position - transform.forward * 0.5f - transform.up * externalOffset +
                    transform.forward * heightOffset, Quaternion.identity, transform);
                tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                tempobj.name = "ed";
                _allGateWalls.Add(tempobj);
                
                tempobj = Instantiate(gatePrefab,
                    transform.position - transform.forward * 0.5f - transform.up * externalOffset +
                    transform.forward * heightOffset - transform.forward * downOffset , Quaternion.identity, transform);
                tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                tempobj.name = "ed";
                _allGateWalls.Add(tempobj);
            }
        }
        
        public bool ActivationStatus()
        {
            return gameObject.activeSelf;
        }

        public void ActivateInteraction()
        {
            
        }

        public void DeActivateInteraction()
        {
            
        }

        public int GetInteractableId()
        {
            return interactableId;
        }

        public void SetInteractableId(int id)
        {
            interactableId = id;
        }

        public ItemCategories GetItemType()
        {
            return ItemCategories.Interactable;
        }

        public void OpenGate()
        {
            
        }

        public void CloseGate()
        {
            CalculateGateRenderPath();
            RenderGate();
        }
        

        public void SetGateValues(Gate gate)
        {
            gateDireciton = gate.gateDireciton;
            
            interactableId = gate.interactableId;
            linkedButtonOnState = gate.linkedButtonOnState;
            linkedButtonOffState = gate.linkedButtonOffState;
        }
    }
    
    [Serializable]
    public class SerializableItem
    {
        public int x;
        public int y;
        public int z;

        public int u;
        public int v;
        public int w;

        public string o;    //Linked Button On State
        public string f;    //Linked Button Off State

        public int i;       //interactable id

        public int d;       //direction of the gate

        public void ConvertToSerializable(Gate gate)
        {
            var transform = gate.transform;
            var position = transform.position;
            x = (int)position.x;
            y = (int)position.y;
            z = (int)position.z;

            var eulerAngles = transform.eulerAngles;
            u = (int) eulerAngles.x;
            v = (int) eulerAngles.y;
            w = (int) eulerAngles.z;

            o = gate.linkedButtonOnState;
            f = gate.linkedButtonOffState;

            i = gate.GetInteractableId();
            d = (int) gate.gateDireciton;
        }

        public Gate GetGate()
        {
            Gate gate = new Gate();

            gate.linkedButtonOnState = o;
            gate.linkedButtonOffState = f;

            gate.interactableId = i;
            gate.gateDireciton = (Direction) d;
            
            return gate;
        }
    }
}
