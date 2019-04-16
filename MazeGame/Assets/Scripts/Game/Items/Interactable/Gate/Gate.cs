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
