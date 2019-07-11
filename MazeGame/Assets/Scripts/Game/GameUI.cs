using System.Collections;
using System.Collections.Generic;
using Game;
using Game.Player;
using TMPro;
using UnityEngine;

namespace Game
{
    public class GameUI : MonoBehaviour
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
