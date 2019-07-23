using System;
using System.Collections;
using System.Collections.Generic;
using Game.Items;
using Game.Managers;
using Game.Player;
using UnityEngine;

namespace Game.Items.Activators.Button
{
    public class Button : MonoBehaviour, IItems
    {
        public bool itemSet;
        public enum ButtonTypes
        {
            Permanent,
            Temporary
        }

        public static List<IIntractables> AllInteractableItems;
        
        public int interactionItemId;    //this can be used as the idex in allInteractableItems
        public IIntractables interactionItem;
        public ButtonTypes type;

        public bool buttonOn; //button on / off
        private bool _tempButtonState;
        private bool _buttonActivated;

        private void Start()
        {
            buttonOn = false;
            _tempButtonState = false;
            _buttonActivated = true;
        }
        
        private void Update()
        {
            if (GameManager.playerCubeTransform.GetComponent<Movement>().movementSnappedFull && !_buttonActivated)
            {
                _buttonActivated = true;
                if (type == ButtonTypes.Temporary)
                {
                    _tempButtonState = true;
                }
                else
                {
                    _tempButtonState = !_tempButtonState;
                }
            }

            if (_tempButtonState != buttonOn)
            {
                buttonOn = _tempButtonState;
                ActivateButtonEvent();
            }
        }

        
        public void ActivateButtonEvent()
        {
            if(interactionItem == null)
                return;
            
            if (buttonOn)
            {
                interactionItem.ActivateInteraction();
            }
            else
            {
                interactionItem.DeActivateInteraction();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            _buttonActivated = false;
        }

        private void OnTriggerExit(Collider other)
        {
            if (type == ButtonTypes.Temporary)
            {
                _tempButtonState = false;
            }
        }

        public ItemCategories GetItemType()
        {
            return ItemCategories.Activators;
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

        public int t;    //Button type
        public int i;    //Button ID
        
        public void ConvertToSerializable(Button button)
        {
            var transform = button.transform;
            var position = transform.position;
            x = (int)position.x;
            y = (int)position.y;
            z = (int)position.z;

            var eulerAngles = transform.eulerAngles;
            u = (int) eulerAngles.x;
            v = (int) eulerAngles.y;
            w = (int) eulerAngles.z;

            t = (int) button.type;
            i = button.interactionItemId;
        }

        public Button GetButton()
        {
            Button button = new Button();

            button.interactionItemId = i;
            button.type = (Button.ButtonTypes) t;
            
            return button;
        }
    }
}