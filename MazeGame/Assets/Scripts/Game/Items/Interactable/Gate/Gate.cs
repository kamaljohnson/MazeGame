using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Items.Interactable.Gate
{
    public class Gate : MonoBehaviour, IInteractables, IItems
    {
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
            return 0;
        }

        public void SetInteractableId(int id)
        {
            
        }

        public ItemCategories GetItemType()
        {
            return ItemCategories.Interactable;
        }
    }
}
