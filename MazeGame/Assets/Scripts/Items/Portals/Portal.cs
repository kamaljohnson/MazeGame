using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Items.Portal
{
    public class Portal : MonoBehaviour, Interactables
    {
        public enum States
        {
            Active,
            InActive
        };

        public bool IsCheckpoint;
        public string PortalName;     //format: "levelname mazeID portalID
        public string DestinationPortalName;
        
        private int portalID;
        private int mazeID;
        private string levelName;
        
        public void CheckpointSaveGameState()       //saves the entier state of the game for checkpoint reference
        {

        }

        public void GoToPortalDestination()         //goes to the destination portal
        {

        }
    }
}