using System;
using System.Collections;
using System.Collections.Generic;
using Game.Items.Intractable.Spike;
using UnityEngine;

namespace Game.Items.Enemies.Guardian
{
    public class Guardian : MonoBehaviour, IItems
    {
        public List<Vector3> locations;
        public bool itemSet;    //is the item set with values
        
        public ItemCategories GetItemType()
        {
            return ItemCategories.Enemie;
        }
        
        public void SetGuardianValues(Guardian guardian)
        {
            locations = guardian.locations;
        }
        
    }

    [Serializable]
    public struct IntVector3
    {
        public int x;
        public int y;
        public int z;
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

        public List<IntVector3> l;
        
        public void ConvertToSerializable(Guardian guardian)
        {
            var transform = guardian.transform;
            var position = transform.position;
            x = (int)position.x;
            y = (int)position.y;
            z = (int)position.z;

            var eulerAngles = transform.eulerAngles;
            u = (int) eulerAngles.x;
            v = (int) eulerAngles.y;
            w = (int) eulerAngles.z;

            foreach (var loc in guardian.locations)
            {
                IntVector3 pos;
                pos.x = (int)loc.x;
                pos.y = (int)loc.y;
                pos.z = (int)loc.z;
                l.Add(pos);
            }
        }

        public Guardian GetGuardian()
        {
            var guardian = new Guardian();
            foreach (var loc in l)
            {
                Vector3 pos;
                pos.x = loc.x;
                pos.y = loc.y;
                pos.z = loc.z;
                guardian.locations.Add(pos);
            }
            return guardian;
        }
    }
}
