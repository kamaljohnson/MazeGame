using TMPro;
using UnityEngine;

namespace Game.Managers
{
    public class GameUIManager : MonoBehaviour
    {
    
        public TMP_Text lives;
        public TMP_Text armour;
    
    
        public void UpdateHealthSystem(int lives, int armour)
        {
            this.lives.text = lives.ToString();
            this.armour.text = armour.ToString();
        }
    }
}
