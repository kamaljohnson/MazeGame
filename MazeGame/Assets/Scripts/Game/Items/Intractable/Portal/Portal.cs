using System;
using Game.Items.Path.Ice;
using Game.Managers;
using Game.Player;
using UnityEngine;

namespace Game.Items.Intractable.Portal
{
    public class Portal : MonoBehaviour, IIntractables, IItems
    {
        public Material checkpointActiveMaterial;
        public Material checkpointDeActiveMaterial;
        
        public int intractableId;    //this id is used to link buttons
        
        public bool itemSet;    //is the item set with values
        
        public bool isCheckpoint;                //is the portal acting as a checkpoint
        private Vector3 _checkpointMazeOrientation;
        private Vector3 _checkpointPlayerPosition;
        private Vector3 _checkpointPlayerOrientation;
        
        public string portalName;               //name format: "levelID:mazeID:portalID"
        public string destinationPortalName;    //name format: "levelID:mazeID:portalID"

        public static Portal CurrentCheckpointDestinationPortal;

        [Tooltip("set this to 0 if start position")]
        public int portalId;                   //0, 1, 2 ..etc [0 => level start location/portal]
        public int levelId;
        public int mazeId;

        [Header("State Properties")]
        [Tooltip("o    -    ON\n" +
                 "f    -    OFF\n" +
                 "u    -    UP\n" +
                 "d    -    DOWN\n" +
                 "r    -    RIGHT\n" +
                 "l    -    LEFT\n" +
                 "c    -    CLK-ROT\n" +
                 "a    -    A-CLK-ROT\n" +
                 "#num -    x-times")]
        public string linkedButtonOnState = "";
        public string linkedButtonOffState = "";
        
        private bool _onPortal;
        private bool _portalActivated;

        public void Update()
        {
            if(_onPortal)
            {
                if (GameManager.playerCubeTransform.GetComponent<Movement>().movementSnappedFull && !_portalActivated)
                {
                    _portalActivated = true;
                    ActivatePortalEvent();
                }
            }
        }

        public void ActivatePortalEvent()
        {
            if (portalId == -1)
            {
                GameManager.gameState = GameManager.GameStates.LevelComplete;
                Debug.Log("level end reached");
            }
            else if (isCheckpoint)
            {
                CheckpointSaveGameState();
            }
            else
            {
                GoToPortalDestination();
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

            intractableId = portal.intractableId;
            linkedButtonOnState = portal.linkedButtonOnState;
            linkedButtonOffState = portal.linkedButtonOffState;
        }

        
        public void CheckpointSaveGameState()   //saves the entire state of the game for checkpoint reference
        {
            var allPortals = FindObjectsOfType<Portal>();
            foreach (var portal in allPortals)
            {
                portal.transform.GetChild(0).GetComponent<MeshRenderer>().material = checkpointDeActiveMaterial;                
            }
            transform.GetChild(0).GetComponent<MeshRenderer>().material = checkpointActiveMaterial;
            CurrentCheckpointDestinationPortal = this;
            _checkpointMazeOrientation = GameManager.mazeTransform.eulerAngles;
            _checkpointPlayerPosition = GameManager.playerCubeTransform.localPosition;
            _checkpointPlayerOrientation = GameManager.playerCubeTransform.localEulerAngles;
        }

        public void CheckpointLoadGameState()
        {
            Debug.Log("Loading Game State");
            GameManager.mazeTransform.eulerAngles = _checkpointMazeOrientation;
            GameManager.playerCubeTransform.localPosition = _checkpointPlayerPosition;
            GameManager.playerCubeTransform.localEulerAngles = _checkpointPlayerOrientation;
        }
        
        public void ActivateCheckpoint()    //sends the player to this portal destination if dead
        {
            Debug.Log("Checkpoint Activated");
            CheckpointLoadGameState();
            GoToPortalDestination();
        }

        public void GoToPortalDestination()     //teleport the player to the destination portal
        {

        }
        
        private void OnTriggerEnter(Collider other)
        {
            _onPortal = true;
        }

        private void OnTriggerExit(Collider other)
        {
            _portalActivated = false;
            _onPortal = false;
        }

        public ItemCategories GetItemType()
        {
            return ItemCategories.Intractable;
        }

        public bool ActivationStatus()
        {
            return gameObject.activeSelf;
        }

        public void ActivateInteraction()
        {
            //TODO: change this to animation
            if (linkedButtonOnState == "o")
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);                
            }
        }

        public void DeActivateInteraction()
        {
            //TODO: change this to animation
            if (linkedButtonOffState == "o")
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);                
            }
        }

        public int GetIntractableId()
        {
            return intractableId;
        }

        public void SetIntractableId(int id)
        {
            intractableId = id;
        }

        public Color GetItemColor()
        {
            return Color.black;
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

        public string o;    //Linked Button On State
        public string f;    //Linked Button Off State

        public int i;    //intractable id

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

            o = portal.linkedButtonOnState;
            f = portal.linkedButtonOffState;

            i = portal.GetIntractableId();
        }

        public Portal GetPortal()
        {
            var portal = new Portal();
            portal.isCheckpoint = c == 1;

            portal.portalId = p;
            portal.mazeId = m;
            portal.levelId = l;

            portal.linkedButtonOnState = o;
            portal.linkedButtonOffState = f;

            portal.intractableId = i;
            
            return portal;
        }
    }
}
