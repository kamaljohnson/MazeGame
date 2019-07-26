using System.Collections;
using System.Collections.Generic;
using Game.Managers;
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
        public Button coinIcon;
        public Button diamondIcon;
        public Button waterIcon;
        public Button energyBlockIcon;
        public Button fireIcon;

        public TMP_Text name;
        public TMP_Text description;

        public Button closeButton;
        
        private static InventoryManager _manager;
        
        public void Start()
        {
            coinIcon.image.sprite = coin.icon;
            diamondIcon.image.sprite = diamond.icon;
            
            waterIcon.image.sprite = water.icon;
            energyBlockIcon.image.sprite = energyBlock.icon;
            fireIcon.image.sprite = fire.icon;

            ShowDetailsOfItem(coin);
                
            _manager = this;
            
            closeButton.onClick.AddListener(HideInventory);
            
            coinIcon.onClick.AddListener(delegate { ShowDetailsOfItem (coin); });
            diamondIcon.onClick.AddListener(delegate { ShowDetailsOfItem (diamond); });
            
            waterIcon.onClick.AddListener(delegate { ShowDetailsOfItem (water); });
            energyBlockIcon.onClick.AddListener(delegate { ShowDetailsOfItem (energyBlock); });
            fireIcon.onClick.AddListener(delegate { ShowDetailsOfItem (fire); });
            
            HideInventory();
        }

        public void ShowDetailsOfItem(InventoryItem item)
        {
            name.text = item.name;
            description.text = item.description;
        }

        public static void ShowInventory()
        {
            GameManager.gameState = GameManager.GameStates.InGameUi;
            _manager.gameObject.SetActive(true);
        }

        public static void HideInventory()
        {
            _manager.gameObject.SetActive(false);
            GameManager.gameState = GameManager.GameStates.Playing;
        }
    }
}
