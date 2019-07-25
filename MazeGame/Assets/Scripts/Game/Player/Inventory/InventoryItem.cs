using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


namespace Game.Player.Inventory
{
    public enum ItemType
    {
        Collectable,
        Item,
        Ability,
        Spells
    }
    
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory")]
    public class InventoryItem : ScriptableObject
    {
        public string name;
        [TextArea]
        public string description;
        public Image icon;
        public ItemType type;
    }
}
