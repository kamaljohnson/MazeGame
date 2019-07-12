using System;
using UnityEngine;

namespace Game.Player
{

    public enum DamageType
    {
        Continuous,
        Quantized
    }
    
    public class HealthSystem : MonoBehaviour
    {
        private static Transform _playerCube;
        
        public int initialLives;
        public int initialArmor;
        public int armourStrength;
        
        private static int _lives;
        private static int _armour;

        private static bool _isGettingDamage;
        private float _damageTimer;
        
        public void Start()
        {
            _playerCube = transform.GetChild(0);
            
            _isGettingDamage = false;
            _damageTimer = 0;
            _lives = initialLives;
            _armour = initialArmor;
            FindObjectOfType<GameUI>().UpdateHealthSystem(_lives, _armour);
        }
        
        public void Update()
        {
            if (_lives <= 0)
            {
                GameManager.Gamestate = GameManager.GameStates.GameOver;
            }

            if (_isGettingDamage && _armour != 0)
            {
                _damageTimer += Time.deltaTime;
                if (_damageTimer >= armourStrength)
                {
                    _damageTimer = 0;
                    _armour -= 1;
                    _isGettingDamage = false;
                    LostArmour();
                }
            }
        }
        
        public static void Hit(int damage, DamageType type)
        {   
            if (_armour != 0)
            {
                switch (type)
                {
                    case DamageType.Continuous:
                        if (_armour >= damage)
                        {
                            _isGettingDamage = true;
                        }
                        else
                        {
                            _lives -= 1;
                            LostLife();
                            GameManager.Gamestate = GameManager.GameStates.Dead;
                        }
                        break;
                    case DamageType.Quantized:
                        if (_armour >= damage)
                        {
                            _armour -= damage;
                            LostArmour();
                        }
                        else
                        {
                            _armour = 0;
                            _lives -= 1;
                            LostLife();
                            GameManager.Gamestate = GameManager.GameStates.Dead;
                        }
                        break;
                }
            }
            else
            {
                _lives -= 1;
                LostLife();
                GameManager.Gamestate = GameManager.GameStates.Dead;
            }
            FindObjectOfType<GameUI>().UpdateHealthSystem(_lives, _armour);
        }

        private static void LostLife()
        {
            Debug.Log("Lost Life");
            _playerCube.GetComponent<Animator>().Play("LoseLifeAnimation", -1, 0);
        }

        private static void LostArmour()
        {
            Debug.Log("Lost Armour");            
            _playerCube.GetComponent<Animator>().Play("LoseArmourAnimation", -1, 0);
        }

        public static void AddLives(int amount)
        {
            _lives += amount;
        }
        
    }
}
