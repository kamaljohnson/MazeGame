using System;
using System.Collections;
using System.Collections.Generic;
using Game.Items.Intractable.Spike;
using Game.Player;
using UnityEngine;

namespace Game.Items.Enemies.Guardian
{
    public class Guardian : MonoBehaviour, IItems
    {
        public List<Vector3> locations;
        public bool itemSet;    //is the item set with values

        [Header("Damage Properties")] 
        public int damage;
        
        [Header("Movement Properties")]
        public float speed;
        public float directionChangeDelay;

        private bool _changingDirection;
        private int _nextLocationIndex;
        private float _timer;
        private int _direction;
        
        public void Start()
        {
            _nextLocationIndex = 1;
            _direction = 1;
            if (_nextLocationIndex == locations.Count - 1 || _nextLocationIndex == 0)
            {
                _direction = -_direction;
            }
        }

        public void Update()
        {
            if (!_changingDirection)
            {
                Move();
            }
            else
            {
                _timer += Time.deltaTime;
                if (_timer >= directionChangeDelay)
                {
                    _changingDirection = false;
                    _timer = 0;
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                HealthSystem.Hit(damage, DamageType.Continuous);
            }
        }
        
        private void Move()
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, locations[_nextLocationIndex], speed * Time.deltaTime);
            if (Vector3.Distance(transform.localPosition, locations[_nextLocationIndex]) < 0.01f)
            {
                transform.localPosition = locations[_nextLocationIndex];
                _nextLocationIndex += _direction;
                if (_nextLocationIndex == locations.Count - 1 || _nextLocationIndex == 0)
                {
                    _direction = -_direction;
                }
            }
        }
        
        public ItemCategories GetItemType()
        {
            return ItemCategories.Enemie;
        }
        
        public void SetGuardianValues(Guardian guardian)
        {
            locations = guardian.locations;
        }
        
    }

    [Serializable]
    public struct IntVector3
    {
        public int x;
        public int y;
        public int z;
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

        public List<IntVector3> l;
        
        public void ConvertToSerializable(Guardian guardian)
        {
            var transform = guardian.transform;
            var position = transform.position;
            x = (int)position.x;
            y = (int)position.y;
            z = (int)position.z;

            var eulerAngles = transform.eulerAngles;
            u = (int) eulerAngles.x;
            v = (int) eulerAngles.y;
            w = (int) eulerAngles.z;

            l = new List<IntVector3>();
            foreach (var loc in guardian.locations)
            {
                IntVector3 pos;
                pos.x = (int)loc.x;
                pos.y = (int)loc.y;
                pos.z = (int)loc.z;
                l.Add(pos);
            }
        }

        public Guardian GetGuardian()
        {
            var guardian = new Guardian();
            guardian.locations = new List<Vector3>();
            foreach (var loc in l)
            {
                Vector3 pos;
                pos.x = loc.x;
                pos.y = loc.y;
                pos.z = loc.z;
                guardian.locations.Add(pos);
            }
            return guardian;
        }
    }
}
