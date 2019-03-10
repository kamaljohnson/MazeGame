using UnityEngine;

namespace Game.Items.Portal
{
    public class Portal : MonoBehaviour, Interactables
    {
        public bool IsCheckpoint;               //is the portal acting as a checkpoint
        private string PortalName;               //name format: "levelID:mazeID:portalID"
        private string DestinationPortalName;    //name format: "levelID:mazeID:portalID"

        public int portalID;                   //0, 1, 2 ..etc [0 => level start location/portal]
        public int mazeID;                     //0, 1, 2 ..etc [0 => initial maze]
        public int levelID;                    //0, 1, 2 ..etc [level 1, level 2 ...]

        public void CheckpointSaveGameState()   //saves the entier state of the game for checkpoint reference
        {

        }

        public void GoToPortalDestination()     //teleports the player to the destination portal
        {

        }
    }

    [SerializeField]
    public class SerializablePortalData
    {
        public int c;   //IsCheckpoint

        public int p;   //PortalID
        public int m;   //MazeID
        public int l;   //LevelID

        public void ConvertToSerializableData(Portal portal)
        {
            c = (portal.IsCheckpoint ? 1 : 0);

            p = portal.portalID;
            m = portal.mazeID;
            l = portal.levelID;
        }

        public Portal ConvertToPortal()
        {
            Portal portal = new Portal();
            portal.IsCheckpoint = (c == 1);

            portal.portalID = p;
            portal.mazeID = m;
            portal.levelID = l;

            return portal;
        }
    }
}