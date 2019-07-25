using Game.Items.Activators.Button;
using Game.Player.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace Game.Managers
{
    public class GameUIManager : MonoBehaviour
    {
    
        public TMP_Text lives;
        public TMP_Text armour;

        public TMP_Text debug;
        static string _debugText;

        public Button inventoryButton;

        public void Start()
        {
            inventoryButton.onClick.AddListener(InventoryManager.ShowInventory);
        }
        
        public void Update()
        {
            debug.text = _debugText;
        }
        

        public void UpdateHealthSystem(int lives, int armour)
        {
            this.lives.text = lives.ToString();
            this.armour.text = armour.ToString();
        }

        public static void Debug(string text)
        {
            _debugText = text;
        }
    }
}
