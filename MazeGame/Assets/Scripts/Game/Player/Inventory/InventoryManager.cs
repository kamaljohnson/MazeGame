using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Player.Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        [Header("Collectable Items")]
        public InventoryItem coin;
        public InventoryItem diamond;
        
        [Header("General Items")]
        public InventoryItem water;
        public InventoryItem energyBlock;
        public InventoryItem fire;

        [Header("UI Elements")] 
        public Image coinIcon;
        public Image diamondIcon;
        public Image waterIcon;
        public Image energyBlockIcon;
        public Image fireIcon;

        public TMP_Text name;
        public TMP_Text description;

        public Button closeButton;
        
        private static InventoryManager _manager;
        
        public void Start()
        {
            coinIcon = coin.icon;
            diamondIcon = diamond.icon;
            
            waterIcon = water.icon;
            energyBlockIcon = energyBlock.icon;
            fireIcon = fire.icon;

            name.text = coin.name;
            description.text = coin.description;
            _manager = this;
            
            closeButton.onClick.AddListener(HideInventory);
        }

        public static void ShowInventory()
        {
            _manager.gameObject.SetActive(true);
        }

        public static void HideInventory()
        {
            _manager.gameObject.SetActive(false);
        }
    }
}
