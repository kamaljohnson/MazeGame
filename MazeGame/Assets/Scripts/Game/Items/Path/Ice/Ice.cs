using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Items.Path.Ice
{
    public class Ice : MonoBehaviour, IItems, IPath
    {
        public static bool onIce;
     
        public ItemCategories GetItemType()
        {
            return ItemCategories.Path;
        }

        private void OnTriggerStay(Collider other)
        {
            onIce = true;
        }

        private void OnTriggerExit(Collider other)
        {
            onIce = false;
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
        
        public int c;   //IsCheckpoint

        public int p;   //PortalID
        public int m;   //MazeID
        public int l;   //LevelID

        public string o;    //Linked Button On State
        public string f;    //Linked Button Off State

        public int i;    //interactable id

        public void ConvertToSerializable(Ice ice)
        {
            var transform = ice.transform;
            var position = transform.position;
            x = (int)position.x;
            y = (int)position.y;
            z = (int)position.z;

            var eulerAngles = transform.eulerAngles;
            u = (int) eulerAngles.x;
            v = (int) eulerAngles.y;
            w = (int) eulerAngles.z;
        }
    }
}
