using System;
using UnityEngine;

namespace Game.Items.Interactable.Portal
{
    public class Portal : MonoBehaviour, IInteractables, IItems
    {
        public bool ItemSet;    //is the item set with values
        
        public bool IsCheckpoint;                //is the portal acting as a checkpoint
        public string PortalName;               //name format: "levelID:mazeID:portalID"
        public string DestinationPortalName;    //name format: "levelID:mazeID:portalID"

        [Tooltip("set this to 0 if start position")]
        public int PortalId;                   //0, 1, 2 ..etc [0 => level start location/portal]
        public int LevelId;
        public int MazeId;

        public void SetPortalValues(Portal portal)
        {
            IsCheckpoint = portal.IsCheckpoint;
            PortalName = portal.PortalName;
            DestinationPortalName = portal.DestinationPortalName;

            PortalId = portal.PortalId;
            LevelId = portal.LevelId;
            MazeId = portal.MazeId;
        }
        
        public void CheckpointSaveGameState()   //saves the entire state of the game for checkpoint reference
        {

        }

        public void GoToPortalDestination()     //teleports the player to the destination portal
        {

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
            
            c = portal.IsCheckpoint ? 1 : 0;

            p = portal.PortalId;
            m = portal.MazeId;
            l = portal.LevelId;
        }

        public Portal GetPortal()
        {
            Portal portal = new Portal();
            portal.IsCheckpoint = (c == 1);

            portal.PortalId = p;
            portal.MazeId = m;
            portal.LevelId = l;

            return portal;
        }
    }
}
