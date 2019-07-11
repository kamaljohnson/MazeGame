using System;
using System.Collections;
using System.Collections.Generic;
using Game.Player;
using UnityEngine;

namespace Game.Items.Intractable.Spike
{
    public enum ActivationType
    {
        Cyclic,
        Linear
    }
    public class Spike : MonoBehaviour, IIntractables, IItems
    {
        public int intractableId;    //this id is used to link buttons
        public int spikeId;    //the id of the spike in the group
        public int groupId;    //the group id

        public bool itemSet;    //is the item set with values
        public ActivationType type;    //the type of activation of the spike group
        
        [Header("State Properties")]
        [Tooltip("o    -    ON\n" +
                 "f    -    OFF\n" +
                 "u    -    UP\n" +
                 "d    -    DOWN\n" +
                 "r    -    RIGHT\n" +
                 "l    -    LEFT\n" +
                 "c    -    CLK-ROT\n" +
                 "a    -    A-CLK-ROT\n" +
                 "#num -    x-times")]
        public string linkedButtonOnState = "";
        public string linkedButtonOffState = "";

        [Header("Spike Activation Properties")]
        public float timeBetweenSpikeActivation;
        
        private int _currentSpikeId;
        private bool _isActivated;
        private float _timer;
        private int _direction;
        private int _maxSpikeId;
        
        public void Start()
        {
            _currentSpikeId = 0;
            _direction = 1;
            _timer = 0;

            var allSpike = FindObjectsOfType<Spike>();
            _maxSpikeId = -1;
            foreach (var spike in allSpike)
            {
                if (spike.groupId == groupId)
                {
                    _maxSpikeId++;
                }
            }
            transform.GetChild(0).GetComponent<Animator>().Play("SpikeDeActivationAnimation", -1, 1);
        }

        public void Update()
        {            
            _timer += Time.deltaTime;
            if (_timer > timeBetweenSpikeActivation)
            {
                _currentSpikeId += _direction;
                _timer = 0;
            }

            if (_currentSpikeId > _maxSpikeId)
            {
                _currentSpikeId = 0;
            }
            
            if (_currentSpikeId == spikeId && !_isActivated)
            {
                ActivateSpike();
            }
            else if(_currentSpikeId != spikeId && _isActivated)
            {
                DeactivateSpike();
            }
        }
        
        public void ActivateSpike()
        {
            transform.GetChild(0).GetComponent<Animator>().Play("SpikeActivationAnimation", -1, 0);
            transform.GetComponent<Collider>().enabled = true;
            _isActivated = true;
        }

        public void DeactivateSpike()
        {
            transform.GetChild(0).GetComponent<Animator>().Play("SpikeDeActivationAnimation", -1, 0);
            transform.GetComponent<Collider>().enabled = false;
            _isActivated = false;
        }

        private void OnTriggerStay(Collider other)
        {
            if (GameManager.PlayerCubeTransform.GetComponent<Movement>().movementSnappedFull)
            {
                HealthSystem.Hit(1);
            }
        }

        public bool ActivationStatus()
        {
            return true;
        }

        public void ActivateInteraction()
        {
            
        }

        public void DeActivateInteraction()
        {
            
        }

        public int GetIntractableId()
        {
            return intractableId;
        }

        public void SetIntractableId(int id)
        {
            intractableId = id;
        }

        public Color GetItemColor()
        {
            return Color.black;
        }

        public ItemCategories GetItemType()
        {
            return ItemCategories.Intractable;
        }

        public void SetSpikeValues(Spike spike)
        {
            groupId = spike.groupId;
            spikeId = spike.spikeId;
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

        public string o;    //Linked Button On State
        public string f;    //Linked Button Off State

        public int s;    //spike id
        public int g;    //group id

        public int t;    //activation type

        public int i;    //intractable id

        public void ConvertToSerializable(Spike spike)
        {
            var transform = spike.transform;
            var position = transform.position;
            x = (int)position.x;
            y = (int)position.y;
            z = (int)position.z;

            var eulerAngles = transform.eulerAngles;
            u = (int) eulerAngles.x;
            v = (int) eulerAngles.y;
            w = (int) eulerAngles.z;

            o = spike.linkedButtonOnState;
            f = spike.linkedButtonOffState;

            i = spike.GetIntractableId();

            t = (int) spike.type;
            
            s = spike.spikeId;
            g = spike.groupId;
        }

        public Spike GetSpike()
        {
            var spike = new Spike();

            spike.linkedButtonOnState = o;
            spike.linkedButtonOffState = f;

            spike.intractableId = i;

            spike.type = (ActivationType) t;
            
            spike.spikeId = s;
            spike.groupId = g;
            
            return spike;
        }
    }
}
