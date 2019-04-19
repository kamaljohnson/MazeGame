using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Items.Intractable.Spike
{
    public enum ActivationType
    {
        Cyclic,
        Linear
    }
    public class Spike : MonoBehaviour, IIntractables, IItems
    {
        public int interactableId;    //this id is used to link buttons
        public int spikeId;    //the id of the spike in the group
        public int groupId;    //the group id
        
        public bool itemSet;    //is the item set with values
        public ActivationType type;    //the type of activation of the spike group
        
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
            return true;
        }

        public void ActivateInteraction()
        {
            
        }

        public void DeActivateInteraction()
        {
            
        }

        public int GetIntractableId()
        {
            return 0;
        }

        public void SetIntractableId(int id)
        {
            
        }

        public ItemCategories GetItemType()
        {
            return ItemCategories.Intractable;
        }

        public void SetSpikeValues(Spike spike)
        {
            groupId = spike.groupId;
            spikeId = spike.spikeId;
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

        public int s;    //spike id
        public int g;    //group id

        public int t;    //activation type

        public int i;    //intractable id

        public void ConvertToSerializable(Spike spike)
        {
            var transform = spike.transform;
            var position = transform.position;
            x = (int)position.x;
            y = (int)position.y;
            z = (int)position.z;

            var eulerAngles = transform.eulerAngles;
            u = (int) eulerAngles.x;
            v = (int) eulerAngles.y;
            w = (int) eulerAngles.z;

            o = spike.linkedButtonOnState;
            f = spike.linkedButtonOffState;

            i = spike.GetIntractableId();

            t = (int) spike.type;
            
            s = spike.spikeId;
            g = spike.groupId;
        }

        public Spike GetSpike()
        {
            var spike = new Spike();

            spike.linkedButtonOnState = o;
            spike.linkedButtonOffState = f;

            spike.interactableId = i;

            spike.type = (ActivationType) t;
            
            spike.spikeId = s;
            spike.groupId = g;
            
            return spike;
        }
    }
}
