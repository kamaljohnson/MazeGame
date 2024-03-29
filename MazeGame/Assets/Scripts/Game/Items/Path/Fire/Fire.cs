﻿using System;
using Game.Player;
using UnityEngine;

namespace Game.Items.Path.Fire
{
    public class Fire : MonoBehaviour, IItems, IPath
    {
        public int damage;
        
        public ItemCategories GetItemType()
        {
            return ItemCategories.Path;
        }
        
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                HealthSystem.Hit(damage, DamageType.Continuous);
            }
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

        public void ConvertToSerializable(Fire fire)
        {
            var transform = fire.transform;
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

