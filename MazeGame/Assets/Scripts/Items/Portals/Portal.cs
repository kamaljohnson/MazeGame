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

        [Tooltip("set this to 0 if start positoin")]
        public int PortalId;                   //0, 1, 2 ..etc [0 => level start location/portal]
        public int LevelId;
        public int MazeId;

        
        public void CheckpointSaveGameState()   //saves the entier state of the game for checkpoint reference
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
        public int c;   //IsCheckpoint

        public int p;   //PortalID
        public int m;   //MazeID
        public int l;   //LevelID

        public void ConvertToSerializable(Portal portal)
        {
            c = portal.IsCheckpoint ? 1 : 0;

            p = portal.PortalId;
            m = portal.MazeId;
            l = portal.LevelId;
        }

        public Portal ConvertToPortal()
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
