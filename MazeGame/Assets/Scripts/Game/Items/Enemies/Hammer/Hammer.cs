using System;
using Game.Player;
using UnityEngine;

namespace Game.Items.Enemies.Hammer
{
    public class Hammer : MonoBehaviour, IItems
    {
        public bool itemSet;
        public float strikeDelay;
        public float animationDelay;

        private bool _playerUnderHammer;
        private bool _activated;
        private float _timer;
        private bool _stricked;

        public void Start()
        {
            _activated = false;
        }

        public void Update()
        {
            if (_activated)
            {
                _timer += Time.deltaTime;
                if (_timer >= strikeDelay && !_stricked)
                {
                    Strike();
                    _timer = 0;
                }

                if (_stricked && _timer >= animationDelay)
                {
                    _activated = false;
                    _stricked = false;
                    _timer = 0;
                }
            }
        }
        
        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Hammer"))
            {
                _activated = true;
                if (other.CompareTag("Player"))
                {
                    _playerUnderHammer = true;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_stricked)
            {
                _activated = false;
            }
            if (other.CompareTag("Player"))
            {
                _playerUnderHammer = false;
            }
        }

        private void Strike()
        {
            _stricked = true;
            transform.GetChild(0).GetComponent<Animator>().Play("HammerStrikeAnimation", -1, 0);
            if (_playerUnderHammer)
            {
                HealthSystem.Hit(1, DamageType.Quantized);
            }
        }
        
        public ItemCategories GetItemType()
        {
            return ItemCategories.Enemie;
        }
    }
    

    [Serializable]
    public class SerializableItem
    {
        public int x;
        public int y;
        public int z;

        public int u;
        public int v;
        public int w;

        public void ConvertToSerializable(Hammer hammer)
        {
            var transform = hammer.transform;
            var position = transform.position;
            x = (int) position.x;
            y = (int) position.y;
            z = (int) position.z;

            var eulerAngles = transform.eulerAngles;
            u = (int) eulerAngles.x;
            v = (int) eulerAngles.y;
            w = (int) eulerAngles.z;
        }

        public Hammer GetHammer()
        {
            var hammer = new Hammer();

            return hammer;
        }
    }
}
