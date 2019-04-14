using System.Collections;
using System.Collections.Generic;
using Game.Items;
using UnityEngine;

namespace Game.Items.Activators.Button
{
    public class Button : MonoBehaviour, IItems
    {
        public enum ButtonTypes
        {
            Permanent,
            Temporary
        }

        public int interactionItemId;
        public IInteractables interactionItem;
        public ButtonTypes type;

        private bool _buttonOn; //button on / off
        private bool _buttonActivated;

        private void Update()
        {
            if (_buttonOn)
            {
                if (!GameManager.CurrentMazeTransform.GetComponent<Maze.MazeRotator>().IsRotating && !_buttonActivated)
                {
                    _buttonActivated = true;
                    ActivateButtonEvent();
                }
            }
        }

        private void ActivateButtonEvent()
        {
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
                _buttonOn = true;
            }
            else
            {
                _buttonOn = !_buttonOn;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (type == ButtonTypes.Temporary)
            {
                _buttonOn = false;
            }
        }

        public ItemCategories GetItemType()
        {
            return ItemCategories.Activators;
        }
    }

    public class SerializableItem
    {
        public void ConvertToSerializable(Button button)
        {
            
        }
    }
}