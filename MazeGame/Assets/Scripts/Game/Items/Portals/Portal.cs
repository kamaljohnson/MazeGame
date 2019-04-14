using System;
using UnityEngine;

namespace Game.Items.Interactable.Portal
{
    public class Portal : MonoBehaviour, IInteractables, IItems
    {
        public bool itemSet;    //is the item set with values
        
        public bool isCheckpoint;                //is the portal acting as a checkpoint
        public string portalName;               //name format: "levelID:mazeID:portalID"
        public string destinationPortalName;    //name format: "levelID:mazeID:portalID"

        [Tooltip("set this to 0 if start position")]
        public int portalId;                   //0, 1, 2 ..etc [0 => level start location/portal]
        public int levelId;
        public int mazeId;

        public static int CurrentCheckpointPortalId;

        private bool _partialOnPortal;
        private bool _onPortal;
        private bool _portalActivated;

        public void Update()
        {
            if(_partialOnPortal)
            {
                if (!GameManager.CurrentMazeTransform.GetComponent<Maze.MazeRotator>().IsRotating && !_portalActivated)
                {
                    _portalActivated = true;
                    ActivatePortalEvent();
                }
            }
        }

        public void ActivatePortalEvent()
        {
            if (isCheckpoint)
            {
                CheckpointSaveGameState();
            }
        }
        
        public void SetPortalValues(Portal portal)
        {
            isCheckpoint = portal.isCheckpoint;
            portalName = portal.portalName;
            destinationPortalName = portal.destinationPortalName;

            portalId = portal.portalId;
            levelId = portal.levelId;
            mazeId = portal.mazeId;
        }

        
        public void CheckpointSaveGameState()   //saves the entire state of the game for checkpoint reference
        {

        }

        public void GoToPortalDestination()     //teleports the player to the destination portal
        {

        }
        
        public void OnTriggerEnter()
        {
            _partialOnPortal = true;
        }

        public void OnTriggerExit()
        {
            _onPortal = false;
            _portalActivated = false;
            _partialOnPortal = false;
        }

        public ItemCategories GetItemType()
        {
            return ItemCategories.Interactable;
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
        
        public int c;   //IsCheckpoint

        public int p;   //PortalID
        public int m;   //MazeID
        public int l;   //LevelID

        public void ConvertToSerializable(Portal portal)
        {
            var transform = portal.transform;
            var position = transform.position;
            x = (int)position.x;
            y = (int)position.y;
            z = (int)position.z;

            var eulerAngles = transform.eulerAngles;
            u = (int) eulerAngles.x;
            v = (int) eulerAngles.y;
            w = (int) eulerAngles.z;
            
            c = portal.isCheckpoint ? 1 : 0;

            p = portal.portalId;
            m = portal.mazeId;
            l = portal.levelId;
        }

        public Portal GetPortal()
        {
            Portal portal = new Portal();
            portal.isCheckpoint = (c == 1);

            portal.portalId = p;
            portal.mazeId = m;
            portal.levelId = l;

            return portal;
        }
    }
}
