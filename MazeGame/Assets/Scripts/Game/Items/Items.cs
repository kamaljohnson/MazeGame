using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Items
{
    public enum ItemCategories
    {
        Path,                   // [ICE, FIRE]
        Interactable,           // [PORTAL, LASER, SPIKES, GATES, BRIDGE]
        Activators,             // [BUTTON]
        Collectable,            // [COIN, DIAMOND, COLLECTION POINT]
        Enemie,                 // [GUARDIAN, KNIGHT, HAMMER]
        Decoratable,            // [PLANT_01, FOUNTAIN, ...]
    }
    
    public interface IItems
    {
        ItemCategories GetItemType();
    }
    
    public interface IInteractables
    {
        bool ActivationStatus();
        void ActivateInteraction();
        void DeActivateInteraction();
        int GetInteractableId();
        void SetInteractableId(int id);
    }
}