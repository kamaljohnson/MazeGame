using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Items.Portal
{
    [SerializeField]
    public class Portal : MonoBehaviour, Interactables
    {
        [HideInInspector]
        public Vector3 portalPos;

        public bool IsCheckpoint;     //is the portal acting as a checkpoint
        public string PortalName;     //name format: "levelname mazeID portalID
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