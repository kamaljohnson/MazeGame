using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Items.Intractable.Laser
{
    
    public class Laser : MonoBehaviour, IIntractables, IItems
    {
        public int intractableId;    //this id is used to link buttons
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

        [Header("activated laser directions")]
        public bool right;
        public bool left;
        public bool forward;
        public bool back;
        
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
            return intractableId;
        }

        public void SetIntractableId(int id)
        {
            intractableId = id;
        }

        public Color GetItemColor()
        {
            return Color.black;
        }

        public ItemCategories GetItemType()
        {
            return ItemCategories.Intractable;
        }
        
        public void SetLazerValues(Laser laser)
        {   
            intractableId = laser.intractableId;
            linkedButtonOnState = laser.linkedButtonOnState;
            linkedButtonOffState = laser.linkedButtonOffState;
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

        public int l;    //activated laser directions

        public int i;       //intractable id
        public void ConvertToSerializable(Laser laser)
        {
            var transform = laser.transform;
            var position = transform.position;
            x = (int)position.x;
            y = (int)position.y;
            z = (int)position.z;

            var eulerAngles = transform.eulerAngles;
            u = (int) eulerAngles.x;
            v = (int) eulerAngles.y;
            w = (int) eulerAngles.z;

            o = laser.linkedButtonOnState;
            f = laser.linkedButtonOffState;

            i = laser.GetIntractableId();
            string tempBin = (laser.right ? "1" : "0") +
                             (laser.left ? "1" : "0") +
                             (laser.forward ? "1" : "0") +
                             (laser.back ? "1" : "0");
            l = Convert.ToInt32(tempBin, 2);

        }

        public Laser GetLaser()
        {
            Laser laser = new Laser();

            laser.linkedButtonOnState = o;
            laser.linkedButtonOffState = f;
            
            //converting p to binary and then to individual ints
            int  n, k;       
            int[] tempPathData = new int[4];
            for (int j = 0; j < 4; j++)
            {
                tempPathData[j] = 0;
            }
            n = l;     
            for(k=0; n>0; k++)      
            {      
                tempPathData[k]=n%2;      
                n = n/2;    
            }
            
            laser.right = tempPathData[3] == 1;
            laser.left = tempPathData[2] == 1;
            laser.forward = tempPathData[1] == 1;
            laser.back = tempPathData[0] == 1;
            
            return laser;
        }
    }
}
