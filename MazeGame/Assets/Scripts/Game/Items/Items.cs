using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Items
{
    public enum ItemCategories
    {
        Path,                   // [ICE, FIRE]
        Intractable,           // [PORTAL, LASER, SPIKES, GATES, BRIDGE]
        Activators,             // [BUTTON]
        Collectable,            // [COIN, DIAMOND, COLLECTION POINT]
        Enemie,                 // [GUARDIAN, KNIGHT, HAMMER]
        Decoratable,            // [PLANT_01, FOUNTAIN, ...]
    }
    
    public interface IItems
    {
        /// <summary>
        /// returns the item category of the item
        /// </summary>
        /// <returns></returns>
        ItemCategories GetItemType();
    }

    public interface IPath
    {
        
    }
    
    public interface IIntractables
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
        /// returns the intractable-id of the item
        /// </summary>
        /// <returns></returns>
        int GetIntractableId();
        
        /// <summary>
        /// call this to set the intractable-id of the item
        /// </summary>
        /// <param name="id"></param>
        void SetIntractableId(int id);

        /// <summary>
        /// returs the color of the item
        /// which will be used by the linked buttons to change its color
        /// </summary>
        /// <returns></returns>
        Color GetItemColor();
    }
}