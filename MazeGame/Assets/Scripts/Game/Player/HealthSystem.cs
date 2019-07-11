using UnityEditor.ShaderGraph;
using UnityEngine;

namespace Game.Player
{
    public class HealthSystem : MonoBehaviour
    {
        public int maxLives;
        private static int _lives;
        private static int _armour;

        public void Start()
        {
            _lives = maxLives;
        }
        
        public void Update()
        {
            if (_lives <= 0)
            {
                GameManager.Gamestate = GameManager.GameStates.GameOver;
            }
        }
        
        public static void Hit(int damage)
        {
            if (_armour != 0)
            {
                if (_armour >= damage)
                {
                    _armour -= damage;
    
                }
                else
                {
                    _armour = 0;
                    _lives -= 1;
                    GameManager.Gamestate = GameManager.GameStates.Dead;
                }
            }
            else
            {
                _lives -= 1;
            }
        }

        public static void AddLives(int amount)
        {
            _lives += amount;
        }
        
    }
}
