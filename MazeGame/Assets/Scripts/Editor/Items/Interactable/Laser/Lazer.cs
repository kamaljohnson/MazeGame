﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Items.Intractable.Lazer
{
    
    public class Lazer : MonoBehaviour, IIntractables, IItems
    {
        public int interactableId;    //this id is used to link buttons
        public bool itemSet;    //is the item set with values

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
            throw new System.NotImplementedException();
        }

        public void ActivateInteraction()
        {
            throw new System.NotImplementedException();
        }

        public void DeActivateInteraction()
        {
            throw new System.NotImplementedException();
        }

        public int GetIntractableId()
        {
            throw new System.NotImplementedException();
        }

        public void SetIntractableId(int id)
        {
            throw new System.NotImplementedException();
        }

        public Color GetItemColor()
        {
            throw new System.NotImplementedException();
        }

        public ItemCategories GetItemType()
        {
            throw new System.NotImplementedException();
        }
    }
    
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

        public int i;       //intractable id
        public void ConvertToSerializable(Lazer lazer)
        {
            var transform = lazer.transform;
            var position = transform.position;
            x = (int)position.x;
            y = (int)position.y;
            z = (int)position.z;

            var eulerAngles = transform.eulerAngles;
            u = (int) eulerAngles.x;
            v = (int) eulerAngles.y;
            w = (int) eulerAngles.z;

            o = lazer.linkedButtonOnState;
            f = lazer.linkedButtonOffState;

            i = lazer.GetIntractableId();
        }

        public Lazer GetLazer()
        {
            Lazer lazer = new Lazer();

            lazer.linkedButtonOnState = o;
            lazer.linkedButtonOffState = f;
            
            return lazer;
        }
    }
}