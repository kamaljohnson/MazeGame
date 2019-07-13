using System;
using System.Collections;
using System.Collections.Generic;
using Game.Items.Intractable.Spike;
using UnityEngine;

namespace Game.Items.Enemies.Hammer
{
    public class Hammer : MonoBehaviour, IItems
    {
        public bool itemSet;    //is the item set with values
        
        public ItemCategories GetItemType()
        {
            return ItemCategories.Enemie;
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

        public void ConvertToSerializable(Hammer hammer)
        {
            var transform = hammer.transform;
            var position = transform.position;
            x = (int) position.x;
            y = (int) position.y;
            z = (int) position.z;

            var eulerAngles = transform.eulerAngles;
            u = (int) eulerAngles.x;
            v = (int) eulerAngles.y;
            w = (int) eulerAngles.z;
        }

        public Hammer GetHammer()
        {
            var hammer = new Hammer();

            return hammer;
        }
    }
}
