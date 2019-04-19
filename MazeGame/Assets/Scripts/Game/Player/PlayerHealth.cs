using UnityEngine;

namespace Game.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        public int maxHealth;
        private static int _health;

        public void Start()
        {
            _health = maxHealth;
        }
        
        public void Update()
        {
            if (_health <= 0)
            {
                GameManager.Gamestate = GameManager.GameStates.GameOver;
            }
        }
        
        public static void DecreaseHealth(int amount)
        {
            _health -= amount;
        }

        public static void IncreaseHealth(int amount)
        {
            _health += amount;
        }
        
    }
}
