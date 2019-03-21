using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Game.Items.Interactable.Portal;
using Game.Maze;
using UnityEngine.SceneManagement;

namespace Game
{
    public class LevelCreator : MonoBehaviour
    {
        public Transform MazeHolder;

        public GameObject MazeCube;
        public GameObject MazeWallPrefab;
        public GameObject PortalPrefab;

        public GameObject PlayerCube;
        private Vector3 _playerStartPosition;

        public void Awake()
        {
            LoadLevel();
            /*
             * TODO: make the maze with the portal 0 as the maze holder for the player
             */
        }

        private void LoadLevel()
        {
            var levelName = SceneManager.GetActiveScene().name;
            var state = LoadLevelDataFromFile(levelName);
            
            //calculating the render data from the path data
            foreach (var maze in state.m)
            {
                var tempMaze = new GameObject();
                tempMaze.AddComponent<Maze.MazeRotator>();
                tempMaze.transform.parent = MazeHolder;
                tempMaze.transform.position = new Vector3(
                    maze.x,
                    maze.y,
                    maze.z
                );

                foreach (var cube in maze.c)
                {
                    var tempCube = Instantiate(MazeCube);
                    tempCube.transform.parent = tempMaze.transform;
                    tempCube.transform.position = new Vector3(
                        cube.x,
                        cube.y,  
                        cube.z
                    );

                    foreach (var node in cube.nl)
                    {
                        var tempNode= new GameObject();
                        tempNode.transform.parent = tempCube.transform;
                        tempNode.transform.localPosition = Vector3.zero;
                        tempNode.transform.eulerAngles = new Vector3(
                            node.u,
                            node.v,
                            node.w
                        );
                        tempNode.transform.localPosition += tempNode.transform.forward * 0.5f;
                        tempNode.gameObject.AddComponent<Maze.Node>();

                        Node tempNodeObj = node.GetNode();
                        tempNodeObj.ParentCubePos = tempCube.transform.position;
                        
                        tempNode.gameObject.GetComponent<Maze.Node>().SetNodeFromNode(tempNodeObj, tempCube.transform.position);
                        tempNode.gameObject.GetComponent<Maze.Node>().CalculateRenderNodePath();
                    }
                    
                    tempCube.SetActive(false);
                }

                foreach (var portal in maze.p)
                {
                    var tempPortal= Instantiate(PortalPrefab);
                    
                    tempPortal.transform.parent = tempMaze.transform;
                    tempPortal.transform.position = new Vector3(
                        portal.x,
                        portal.y,
                        portal.z
                        );                    
                    tempPortal.transform.eulerAngles = new Vector3(
                        portal.u,
                        portal.v,
                        portal.w
                    );
                    tempPortal.GetComponent<Portal>().SetPortalValues(portal.GetPortal());
                    if (tempPortal.GetComponent<Portal>().PortalId == 0)
                    {
                        var playerCube = Instantiate(PlayerCube);
                        playerCube.GetComponent<Player.Movement>().SetParentMaze(MazeHolder.GetChild(tempPortal.GetComponent<Portal>().MazeId).gameObject);
                        playerCube.transform.position = tempPortal.transform.position - tempPortal.transform.up * tempPortal.transform.localScale.y * 0.5f + playerCube.transform.up * 1/6f;
                        playerCube.transform.eulerAngles = tempPortal.transform.eulerAngles;
                        tempPortal.SetActive(false);
                    }
                }
            }

            /*
            *Creates individual blocks of the maze walls using the render_* data from the nodes
            */
            for (int i = 0; i < MazeHolder.childCount; i++)     //for each maze
            {
                Transform _maze = MazeHolder.GetChild(i);
                for (int j = 0; j < _maze.childCount; j++)    //for each cube
                {
                    Transform _cube = _maze.GetChild(j);
                    if(_cube.CompareTag("MazeCube"))
                        _cube.gameObject.SetActive(true);

                    for (int k = 0; k < _cube.childCount; k++)      //for each node
                    {
                        Node node = _cube.GetChild(k).GetComponent<Node>();
                        if (node == null)
                            break;
                        
                        float offset = 1 / 2f - 1 / 12f;
                        float height_offset = 1 / 12f;

                        float external_offset = 1 / 2f + 1 / 12f;
                        float internal_offset = 1 / 2f - 1 / 12f;

                        float w_size = 4 / 6f;
                        float c_size = 1 / 6f;
                        float h_size = 1 / 6f;

                        if (node.Rrender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab, node.transform.position + node.transform.right * offset + node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                            tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                            tempobj.name = "r";
                            tempobj.transform.parent = node.transform;
                        }
                        if (node.Lrender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab, node.transform.position - node.transform.right * offset + node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                            tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                            tempobj.name = "l";
                            tempobj.transform.parent = node.transform;
                        }
                        if (node.Urender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab, node.transform.position + node.transform.up * offset + node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                            tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                            tempobj.name = "u";
                            tempobj.transform.parent = node.transform;
                        }
                        if (node.Drender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab, node.transform.position - node.transform.up * offset + node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                            tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                            tempobj.name = "d";
                            tempobj.transform.parent = node.transform;
                        }

                        //rendering corner
                        if (node.RUrender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab,
                                node.transform.position + (node.transform.right + node.transform.up) * offset +
                                node.transform.forward * height_offset, node.transform.rotation, node.transform);
                            tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                            tempobj.name = "ru";
                            tempobj.transform.parent = node.transform;
                        }
                        if (node.RDrender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab,
                                node.transform.position + (node.transform.right - node.transform.up) * offset +
                                node.transform.forward * height_offset, node.transform.rotation, node.transform);
                            tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                            tempobj.name = "rd";
                            tempobj.transform.parent = node.transform;
                        }
                        if (node.LUrender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab,
                                node.transform.position + (-node.transform.right + node.transform.up) * offset +
                                node.transform.forward * height_offset, node.transform.rotation, node.transform);
                            tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                            tempobj.name = "lu";
                            tempobj.transform.parent = node.transform;
                        }
                        if (node.LDrender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab,
                                node.transform.position + (-node.transform.right - node.transform.up) * offset +
                                node.transform.forward * height_offset, node.transform.rotation, node.transform);
                            tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                            tempobj.name = "ld";
                            tempobj.transform.parent = node.transform;
                        }

                        //rendering external edges
                        if (node.ERrender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab, node.transform.position + node.transform.right * external_offset + node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                            tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                            tempobj.name = "er";
                            tempobj.transform.parent = node.transform;
                        }

                        if (node.ELrender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab,
                                node.transform.position - node.transform.right * external_offset +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                            tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                            tempobj.name = "el";
                            tempobj.transform.parent = node.transform;
                        }

                        if (node.EUrender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab,
                                node.transform.position + node.transform.up * external_offset +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                            tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                            tempobj.name = "eu";
                            tempobj.transform.parent = node.transform;
                        }

                        if (node.EDrender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab,
                                node.transform.position - node.transform.up * external_offset +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                            tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                            tempobj.name = "ed";
                            tempobj.transform.parent = node.transform;
                        }

                        if (node.IRrender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab,
                                node.transform.position + node.transform.right * internal_offset +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                            tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                            tempobj.name = "er";
                            tempobj.transform.parent = node.transform;
                        }

                        if (node.ILrender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab,
                                node.transform.position - node.transform.right * internal_offset +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                            tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                            tempobj.name = "el";
                            tempobj.transform.parent = node.transform;
                        }

                        if (node.IUrender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab,
                                node.transform.position + node.transform.up * internal_offset +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                            tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                            tempobj.name = "eu";
                            tempobj.transform.parent = node.transform;
                        }

                        if (node.IDrender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab,
                                node.transform.position - node.transform.up * internal_offset +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                            tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                            tempobj.name = "ed";
                            tempobj.transform.parent = node.transform;
                        }

                        if (node.ERUrender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab,
                                node.transform.position +
                                (node.transform.right * external_offset + node.transform.up * offset) +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                            tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                            tempobj.name = "eru";
                            tempobj.transform.parent = node.transform;

                        }

                        if (node.ERDrender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab,
                                node.transform.position +
                                (node.transform.right * external_offset - node.transform.up * offset) +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                            tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                            tempobj.name = "erd";
                            tempobj.transform.parent = node.transform;

                        }

                        if (node.ELUrender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab,
                                node.transform.position +
                                (-node.transform.right * external_offset + node.transform.up * offset) +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                            tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                            tempobj.name = "elu";
                            tempobj.transform.parent = node.transform;

                        }

                        if (node.ELDrender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab,
                                node.transform.position +
                                (-node.transform.right * external_offset - node.transform.up * offset) +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                            tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                            tempobj.name = "eld";
                            tempobj.transform.parent = node.transform;

                        }

                        if (node.EURrender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab,
                                node.transform.position +
                                (node.transform.right * offset + node.transform.up * external_offset) +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                            tempobj.transform.localEulerAngles = new Vector3(0, 0, 90);
                            tempobj.name = "eur";
                            tempobj.transform.parent = node.transform;

                        }

                        if (node.EULrender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab,
                                node.transform.position +
                                (-node.transform.right * offset + node.transform.up * external_offset) +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                            tempobj.transform.localEulerAngles = new Vector3(0, 0, 90);
                            tempobj.name = "eul";
                            tempobj.transform.parent = node.transform;

                        }

                        if (node.EDRrender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab,
                                node.transform.position +
                                (node.transform.right * offset - node.transform.up * external_offset) +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                            tempobj.transform.localEulerAngles = new Vector3(0, 0, 90);
                            tempobj.name = "edr";
                            tempobj.transform.parent = node.transform;

                        }

                        if (node.EDLrender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab,
                                node.transform.position +
                                (-node.transform.right * offset - node.transform.up * external_offset) +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                            tempobj.transform.localEulerAngles = new Vector3(0, 0, 90);
                            tempobj.name = "edl";
                            tempobj.transform.parent = node.transform;
                        }

                        if (node.EERUrender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab,
                                node.transform.position +
                                (node.transform.right + node.transform.up) * external_offset +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localScale = new Vector3(h_size, h_size, h_size);
                            tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                            tempobj.name = "eeru";
                            tempobj.transform.parent = node.transform;
                        }

                        if (node.EERDrender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab,
                                node.transform.position +
                                (node.transform.right - node.transform.up) * external_offset +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localScale = new Vector3(h_size, h_size, h_size);
                            tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                            tempobj.name = "eerd";
                            tempobj.transform.parent = node.transform;
                        }

                        if (node.EELUrender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab,
                                node.transform.position +
                                (-node.transform.right + node.transform.up) * external_offset +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localScale = new Vector3(h_size, h_size, h_size);
                            tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                            tempobj.name = "eelu";
                            tempobj.transform.parent = node.transform;
                        }

                        if (node.EELDrender)
                        {
                            GameObject tempobj = Instantiate(MazeWallPrefab,
                                node.transform.position +
                                (-node.transform.right - node.transform.up) * external_offset +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localScale = new Vector3(h_size, h_size, h_size);
                            tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                            tempobj.name = "eeld";
                            tempobj.transform.parent = node.transform;
                        }
                    }
                }
            }
        }

        public SaveState LoadLevelDataFromFile(string levelName)
        {
            string directory = Application.streamingAssetsPath + "/Levels/" + levelName;
            SaveState state;

            string jsonString;
            if(Application.platform == RuntimePlatform.Android)
            {
                Debug.Log("loading old state");
                WWW reader = new WWW(directory);
                while (!reader.isDone) { }

                jsonString = reader.text;
                Debug.Log("loading complete : " + jsonString + " from :" + directory);
            }
            else
            {
                jsonString = File.ReadAllText(directory);
            }
            Debug.Log(jsonString);
            state = JsonUtility.FromJson<SaveState>(jsonString);
            return state;
        }
    }

    [System.Serializable]
    public class SaveState
    {
        //list of all the maze cubes along with its list of attached nodes
        public List<MazeData> m;
    }

    [System.Serializable]
    public class MazeData
    {
        public List<MazeCubeData> c;
        public int x;
        public int y;
        public int z;

        //item data
        public List<Items.Interactable.Portal.SerializableItem> p;    //the list of all portals on the maze
    }

    [System.Serializable]
    public class MazeCubeData
    {
        //maze cube transform
        public int x;
        public int y;
        public int z;
        
        //the list of nodes the cube has
        public List<SavableNode> nl;
    }
}