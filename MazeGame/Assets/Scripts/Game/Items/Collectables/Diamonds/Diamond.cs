using System;
using Game.Managers;
using UnityEngine;

namespace Game.Items.Collectables.Diamond
{
    public class Diamond : MonoBehaviour, IItems
    {
        public int index;
        public ItemCategories GetItemType()
        {
            return ItemCategories.Collectable;
        }

        public void OnTriggerEnter(Collider other)
        {
            CollectDiamond();
        }

        public void CollectDiamond()
        {
            Debug.Log("diamond collected");
            //transform.GetChild(0).GetComponent<Animator>().Play("coinCollectAnimation", -1, 0);
            GameManager.stateManager.indexOfCoinsCollected.Add(index);
            Destroy(gameObject, 0.3f);
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

        public void ConvertToSerializable(Diamond diamond)
        {
            var transform = diamond.transform;
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
