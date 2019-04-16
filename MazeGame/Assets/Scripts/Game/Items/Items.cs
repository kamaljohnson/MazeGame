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
        /// <summary>
        /// returns the item catagory of the item
        /// </summary>
        /// <returns></returns>
        ItemCategories GetItemType();
    }
    
    public interface IInteractables
    {
        /// <summary>
        /// returns if the item is activated or not,
        /// i.e. the button linked to the item is on or off
        /// </summary>
        /// <returns></returns>
        bool ActivationStatus();
        
        /// <summary>
        /// activates the item,
        /// call this when the button is on
        /// </summary>
        void ActivateInteraction();
        
        /// <summary>
        /// de-activates the item,
        /// call this when the button is off
        /// </summary>
        void DeActivateInteraction();
        
        /// <summary>
        /// returns the interactable-id of the item
        /// </summary>
        /// <returns></returns>
        int GetInteractableId();
        
        /// <summary>
        /// call this to set the interactable-id of the item
        /// </summary>
        /// <param name="id"></param>
        void SetInteractableId(int id);
    }
}