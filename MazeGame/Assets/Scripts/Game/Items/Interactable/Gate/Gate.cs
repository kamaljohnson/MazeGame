using System;
using System.Collections;
using System.Collections.Generic;
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
        //corners
        public bool rUrender;
        public bool rDrender;
        public bool lUrender;
        public bool lDrender;

        public bool eRrender;
        public bool eLrender;
        public bool eUrender;
        public bool eDrender;

        public bool erUrender;
        public bool erDrender;
        public bool elUrender;
        public bool elDrender;

        public bool euRrender;
        public bool euLrender;
        public bool edRrender;
        public bool edLrender;

        public bool eerUrender;
        public bool eerDrender;
        public bool eelUrender;
        public bool eelDrender;
        
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

            rUrender = false;
            rDrender = false;
            lUrender = false;
            lDrender = false;

            eRrender = false;
            eLrender = false;
            eUrender = false;
            eDrender = false;

            erUrender = false;
            erDrender = false;
            elUrender = false;
            elDrender = false;

            euRrender = false;
            euLrender = false;
            edRrender = false;
            edLrender = false;

            eerUrender = false;
            eerDrender = false;
            eelUrender = false;
            eelDrender = false;

            switch (gateDireciton)
            {
                case Direction.Up:
                    urender = true;
                    rrender = true;
                    lrender = true;
                    break;
                case Direction.Left:
                    lrender = true;
                    urender = true;
                    drender = true;
                    break;
                case Direction.Down:
                    drender = true;
                    rrender = true;
                    lrender = true;
                    break;
                case Direction.Right:
                    rrender = true;
                    urender = true;
                    drender = true;
                    break;
            }

            Debug.DrawRay(transform.position - transform.forward, transform.right * 0.6f, Color.red, 100);
            if (!Physics.Raycast( transform.position - transform.forward, transform.right * 0.6f, out hit, 0.5f))
            {
                erUrender = true;
                erDrender = true;
                if (rrender)
                {
                    eRrender = true;
                }
            }
            Debug.DrawRay(transform.position - transform.forward, -transform.right * 0.6f, Color.red, 100);
            if (!Physics.Raycast( transform.position - transform.forward, -transform.right * 0.6f, out hit, 0.5f))
            {
                elUrender = true;
                elDrender = true;
                if (lrender)
                {
                    eLrender = true;
                }
            }
            Debug.DrawRay(transform.position - transform.forward, transform.up * 0.6f, Color.red, 100);
            if (!Physics.Raycast( transform.position - transform.forward, transform.up * 0.6f, out hit, 0.5f))
            {
                euRrender = true;
                euLrender = true;
                if (urender)
                {
                    eUrender = true;
                }
            }
            Debug.DrawRay(transform.position - transform.forward, -transform.up * 0.6f, Color.red, 100);
            if (!Physics.Raycast( transform.position - transform.forward, - transform.up * 0.6f, out hit, 0.5f))
            {
                edRrender = true;
                edLrender = true;
                if (drender)
                {
                    eDrender = true;
                }
            }

            if (erUrender && euRrender)
            {
                eerUrender = true;
            }
            if (elUrender && euLrender)
            {
                eelUrender = true;
            }
            if (erDrender && edRrender)
            {
                eerDrender = true;
            }
            if (elDrender && edLrender)
            {
                eelDrender = true;
            }
        }

        public void RenderGate()
        {
            transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            _allGateWalls = new List<GameObject>();
            
            var offset = 1 / 2f - 1 / 12f;
            var height_offset = 1 / 12f;
            
            var external_offset = 1 / 2f + 1 / 12f;
            
            var w_size = 4 / 6f;
            var c_size = 1 / 6f;
            var h_size = 1 / 6f;
            
            //rendering walls
            if (rrender)
            {
                GameObject tempobj = Instantiate(gatePrefab, transform.position + transform.right * offset + transform.forward * height_offset, Quaternion.identity, transform);
                tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                tempobj.name = "r";
                _allGateWalls.Add(tempobj);
            }
            if (lrender)
            {
                GameObject tempobj = Instantiate(gatePrefab, transform.position - transform.right * offset + transform.forward * height_offset, Quaternion.identity, transform);
                tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                tempobj.name = "l";
                _allGateWalls.Add(tempobj);
            }
            if (urender)
            {
                GameObject tempobj = Instantiate(gatePrefab, transform.position + transform.up * offset + transform.forward * height_offset, Quaternion.identity, transform);
                tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                tempobj.name = "u";
                _allGateWalls.Add(tempobj);
            }
            if (drender)
            {
                GameObject tempobj = Instantiate(gatePrefab, transform.position - transform.up * offset + transform.forward * height_offset, Quaternion.identity, transform);
                tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                tempobj.name = "d";
                _allGateWalls.Add(tempobj);
            }
            
            //rendering corner
            if (rUrender)
            {
                GameObject tempobj = Instantiate(gatePrefab,
                    transform.position + (transform.right + transform.up) * offset +
                    transform.forward * height_offset, transform.rotation, transform);
                tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                tempobj.name = "ru";
                _allGateWalls.Add(tempobj);
            }
            if (rDrender)
            {
                GameObject tempobj = Instantiate(gatePrefab,
                    transform.position + (transform.right - transform.up) * offset +
                    transform.forward * height_offset, transform.rotation, transform);
                tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                tempobj.name = "rd";
                _allGateWalls.Add(tempobj);
            }
            if (lUrender)
            {
                GameObject tempobj = Instantiate(gatePrefab,
                    transform.position + (-transform.right + transform.up) * offset +
                    transform.forward * height_offset, transform.rotation, transform);
                tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                tempobj.name = "lu";
                _allGateWalls.Add(tempobj);
            }
            if (lDrender)
            {
                GameObject tempobj = Instantiate(gatePrefab,
                    transform.position + (-transform.right - transform.up) * offset +
                    transform.forward * height_offset, transform.rotation, transform);
                tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                tempobj.name = "ld";
                _allGateWalls.Add(tempobj);
            }
            
            //rendering external edges
            if (eRrender)
            {
                GameObject tempobj = Instantiate(gatePrefab, transform.position + transform.right * external_offset + transform.forward * height_offset, Quaternion.identity, transform);
                tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                tempobj.name = "er";
                _allGateWalls.Add(tempobj);
            }
            
            if (eLrender)
            {
                GameObject tempobj = Instantiate(gatePrefab,
                    transform.position - transform.right * external_offset +
                    transform.forward * height_offset, Quaternion.identity, transform);
                tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                tempobj.name = "el";
                _allGateWalls.Add(tempobj);
            }
            
            if (eUrender)
            {
                GameObject tempobj = Instantiate(gatePrefab,
                    transform.position + transform.up * external_offset +
                    transform.forward * height_offset, Quaternion.identity, transform);
                tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                tempobj.name = "eu";
                _allGateWalls.Add(tempobj);
            }
            
            if (eDrender)
            {
                GameObject tempobj = Instantiate(gatePrefab,
                    transform.position - transform.up * external_offset +
                    transform.forward * height_offset, Quaternion.identity, transform);
                tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                tempobj.name = "ed";
                _allGateWalls.Add(tempobj);
            }
            
            if (erUrender)
            {
                GameObject tempobj = Instantiate(gatePrefab,
                    transform.position +
                    (transform.right * external_offset + transform.up * offset) +
                    transform.forward * height_offset, Quaternion.identity, transform);
                tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                tempobj.name = "eru";
                _allGateWalls.Add(tempobj);
            
            }
            
            if (erDrender)
            {
                GameObject tempobj = Instantiate(gatePrefab,
                    transform.position +
                    (transform.right * external_offset - transform.up * offset) +
                    transform.forward * height_offset, Quaternion.identity, transform);
                tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                tempobj.name = "erd";
                _allGateWalls.Add(tempobj);
            
            }
            
            if (elUrender)
            {
                GameObject tempobj = Instantiate(gatePrefab,
                    transform.position +
                    (-transform.right * external_offset + transform.up * offset) +
                    transform.forward * height_offset, Quaternion.identity, transform);
                tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                tempobj.name = "elu";
                _allGateWalls.Add(tempobj);
            
            }
            
            if (elDrender)
            {
                GameObject tempobj = Instantiate(gatePrefab,
                    transform.position +
                    (-transform.right * external_offset - transform.up * offset) +
                    transform.forward * height_offset, Quaternion.identity, transform);
                tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                tempobj.name = "eld";
                _allGateWalls.Add(tempobj);
            
            }
            
            if (euRrender)
            {
                GameObject tempobj = Instantiate(gatePrefab,
                    transform.position +
                    (transform.right * offset + transform.up * external_offset) +
                    transform.forward * height_offset, Quaternion.identity, transform);
                tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                tempobj.transform.localEulerAngles = new Vector3(0, 0, 90);
                tempobj.name = "eur";
                _allGateWalls.Add(tempobj);
            
            }
            
            if (euLrender)
            {
                GameObject tempobj = Instantiate(gatePrefab,
                    transform.position +
                    (-transform.right * offset + transform.up * external_offset) +
                    transform.forward * height_offset, Quaternion.identity, transform);
                tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                tempobj.transform.localEulerAngles = new Vector3(0, 0, 90);
                tempobj.name = "eul";
                _allGateWalls.Add(tempobj);
            
            }
            
            if (edRrender)
            {
                GameObject tempobj = Instantiate(gatePrefab,
                    transform.position +
                    (transform.right * offset - transform.up * external_offset) +
                    transform.forward * height_offset, Quaternion.identity, transform);
                tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                tempobj.transform.localEulerAngles = new Vector3(0, 0, 90);
                tempobj.name = "edr";
                _allGateWalls.Add(tempobj);
            
            }
            
            if (edLrender)
            {
                GameObject tempobj = Instantiate(gatePrefab,
                    transform.position +
                    (-transform.right * offset - transform.up * external_offset) +
                    transform.forward * height_offset, Quaternion.identity, transform);
                tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                tempobj.transform.localEulerAngles = new Vector3(0, 0, 90);
                tempobj.name = "edl";
                _allGateWalls.Add(tempobj);
            }
            
            if (eerUrender)
            {
                GameObject tempobj = Instantiate(gatePrefab,
                    transform.position +
                    (transform.right + transform.up) * external_offset +
                    transform.forward * height_offset, Quaternion.identity, transform);
                tempobj.transform.localScale = new Vector3(h_size, h_size, h_size);
                tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                tempobj.name = "eeru";
                _allGateWalls.Add(tempobj);
            }
            
            if (eerDrender)
            {
                GameObject tempobj = Instantiate(gatePrefab,
                    transform.position +
                    (transform.right - transform.up) * external_offset +
                    transform.forward * height_offset, Quaternion.identity, transform);
                tempobj.transform.localScale = new Vector3(h_size, h_size, h_size);
                tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                tempobj.name = "eerd";
                _allGateWalls.Add(tempobj);
            }
            
            if (eelUrender)
            {
                GameObject tempobj = Instantiate(gatePrefab,
                    transform.position +
                    (-transform.right + transform.up) * external_offset +
                    transform.forward * height_offset, Quaternion.identity, transform);
                tempobj.transform.localScale = new Vector3(h_size, h_size, h_size);
                tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                tempobj.name = "eelu";
                _allGateWalls.Add(tempobj);
            }
            
            if (eelDrender)
            {
                GameObject tempobj = Instantiate(gatePrefab,
                    transform.position +
                    (-transform.right - transform.up) * external_offset +
                    transform.forward * height_offset, Quaternion.identity, transform);
                tempobj.transform.localScale = new Vector3(h_size, h_size, h_size);
                tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                tempobj.name = "eeld";
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
