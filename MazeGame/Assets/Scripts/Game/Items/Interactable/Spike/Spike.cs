using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Items.Interactable.Spike
{
    public class Spike : MonoBehaviour, IInteractables, IItems
    {
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
