using System;
using Game.Items.Path.Ice;
using UnityEngine;

namespace Game.Items.Collectables.Coin
{
    public class Coin : MonoBehaviour, IItems
    {
        public int index;
        public ItemCategories GetItemType()
        {
            return ItemCategories.Collectable;
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

        public void ConvertToSerializable(Coin coin)
        {
            var transform = coin.transform;
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
