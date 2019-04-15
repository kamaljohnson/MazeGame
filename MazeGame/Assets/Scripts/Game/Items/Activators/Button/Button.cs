using System;
using System.Collections;
using System.Collections.Generic;
using Game.Items;
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

        public int interactionItemId;
        public IInteractables interactionItem;
        public ButtonTypes type;

        public bool buttonOn; //button on / off
        private bool _buttonActivated;

        private void Update()
        {
            if (buttonOn)
            {
                if (GameManager.PlayerCubeTransform.GetComponent<Movement>().movementSnappedFull && !_buttonActivated)
                {
                    _buttonActivated = true;
                    ActivateButtonEvent();
                }
            }
        }

        private void ActivateButtonEvent()
        {
            if(interactionItem == null)
                return;
            
            if (interactionItem.ActivationStatus())
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
            if (type == ButtonTypes.Temporary)
            {
                buttonOn = true;
            }
            else
            {
                buttonOn = !buttonOn;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (type == ButtonTypes.Temporary)
            {
                buttonOn = false;
                _buttonActivated = false;
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
        }

        public Button GetButton()
        {
            Button button = new Button();
            return button;
        }
    }
}