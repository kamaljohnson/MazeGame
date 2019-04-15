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

        public static List<IInteractables> AllInteractableItems;
        
        public int interactionItemId;    //this can be used as the idex in allInteractableItems
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