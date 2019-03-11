using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

namespace Game
{
    public class LevelCreator : MonoBehaviour
    {
        public Transform MazeHolder;

        public GameObject MazeCube;
        public GameObject MazeWallPrefab;

        public GameObject PlayerCube;

        public void Awake()
        {
            LoadLevel();
            GameObject playerCube = Instantiate(PlayerCube);
            playerCube.GetComponent<Player.Movement>().SetParentMaze(MazeHolder.GetChild(0).gameObject);
        }

        public void LoadLevel()
        {
            string levelName = SceneManager.GetActiveScene().name;
            SaveState state = LoadLevelDataFromFile(levelName);
            
            //calculating the render data from the path data
            for(int i = 0; i < state.m.Count; i++)  //for each maze
            {
                GameObject _maze = new GameObject();
                _maze.AddComponent<Maze.MazeRotator>();
                _maze.transform.parent = MazeHolder;
                _maze.transform.position = new Vector3(
                    state.m[i].x,
                    state.m[i].y,
                    state.m[i].z
                    );

                for (int j = 0; j < state.m[i].c.Count; j++)    //for each cube
                {
                    GameObject _cube = Instantiate(MazeCube);
                    _cube.transform.parent = _maze.transform;
                    _cube.transform.position = new Vector3(
                        state.m[i].c[j].x,
                        state.m[i].c[j].y,  
                        state.m[i].c[j].z
                        );

                    for (int k = 0; k < state.m[i].c[j].nl.Count; k++)  //for each node
                    {
                        GameObject _node= new GameObject();
                        _node.transform.parent = _cube.transform;
                        _node.transform.localPosition = Vector3.zero;
                        _node.transform.eulerAngles = new Vector3(
                            state.m[i].c[j].nl[k].u,
                            state.m[i].c[j].nl[k].v,
                            state.m[i].c[j].nl[k].w
                            );
                        _node.transform.localPosition += _node.transform.forward * 0.5f;
                        _node.gameObject.AddComponent<Maze.Node>();

                        Maze.Node _tempNode = state.m[i].c[j].nl[k].GetNode();
                        _tempNode.parentCube_pos = _cube.transform.position;
                        
                        _node.gameObject.GetComponent<Maze.Node>().SetNodeFromNode(_tempNode, _cube.transform.position);
                        _node.gameObject.GetComponent<Maze.Node>().CalculateRenderNodePath();
                    }
                    _cube.SetActive(false);
                }
            }

            /*
            *Creates individual blocks of the maze walls using the render_* data from the nodes
            * 
            */

            for (int i = 0; i < MazeHolder.childCount; i++)     //for each maze
            {
                Transform _maze = MazeHolder.GetChild(i);
                for (int j = 0; j < _maze.childCount; j++)    //for each cube
                {
                    Transform _cube = _maze.GetChild(j);
                    _cube.gameObject.SetActive(true);

                    
                    for (int k = 0; k < _cube.childCount; k++)      //for each node
                    {
                        Maze.Node node = _cube.GetChild(k).GetComponent<Maze.Node>();

                        float offset = 1 / 2f - 1 / 12f;
                        float height_offset = 1 / 12f;

                        float external_offset = 1 / 2f + 1 / 12f;
                        float internal_offset = 1 / 2f - 1 / 12f;

                        float w_size = 4 / 6f;
                        float c_size = 1 / 6f;
                        float h_size = 1 / 6f;

                        if (node.r_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab, node.transform.position + node.transform.right * offset + node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                            tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                            tempobj.name = "r";
                            tempobj.transform.parent = node.transform;
                        }
                        if (node.l_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab, node.transform.position - node.transform.right * offset + node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                            tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                            tempobj.name = "l";
                            tempobj.transform.parent = node.transform;
                        }
                        if (node.u_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab, node.transform.position + node.transform.up * offset + node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                            tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                            tempobj.name = "u";
                            tempobj.transform.parent = node.transform;
                        }
                        if (node.d_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab, node.transform.position - node.transform.up * offset + node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                            tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                            tempobj.name = "d";
                            tempobj.transform.parent = node.transform;
                        }

                        //rendering corner
                        if (node.ru_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab,
                                node.transform.position + (node.transform.right + node.transform.up) * offset +
                                node.transform.forward * height_offset, node.transform.rotation, node.transform);
                            tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                            tempobj.name = "ru";
                            tempobj.transform.parent = node.transform;
                        }
                        if (node.rd_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab,
                                node.transform.position + (node.transform.right - node.transform.up) * offset +
                                node.transform.forward * height_offset, node.transform.rotation, node.transform);
                            tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                            tempobj.name = "rd";
                            tempobj.transform.parent = node.transform;
                        }
                        if (node.lu_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab,
                                node.transform.position + (-node.transform.right + node.transform.up) * offset +
                                node.transform.forward * height_offset, node.transform.rotation, node.transform);
                            tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                            tempobj.name = "lu";
                            tempobj.transform.parent = node.transform;
                        }
                        if (node.ld_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab,
                                node.transform.position + (-node.transform.right - node.transform.up) * offset +
                                node.transform.forward * height_offset, node.transform.rotation, node.transform);
                            tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                            tempobj.name = "ld";
                            tempobj.transform.parent = node.transform;
                        }

                        //rendering external edges
                        if (node.er_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab, node.transform.position + node.transform.right * external_offset + node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                            tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                            tempobj.name = "er";
                            tempobj.transform.parent = node.transform;
                        }

                        if (node.el_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab,
                                node.transform.position - node.transform.right * external_offset +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                            tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                            tempobj.name = "el";
                            tempobj.transform.parent = node.transform;
                        }

                        if (node.eu_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab,
                                node.transform.position + node.transform.up * external_offset +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                            tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                            tempobj.name = "eu";
                            tempobj.transform.parent = node.transform;
                        }

                        if (node.ed_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab,
                                node.transform.position - node.transform.up * external_offset +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                            tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                            tempobj.name = "ed";
                            tempobj.transform.parent = node.transform;
                        }

                        if (node.ir_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab,
                                node.transform.position + node.transform.right * internal_offset +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                            tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                            tempobj.name = "er";
                            tempobj.transform.parent = node.transform;
                        }

                        if (node.il_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab,
                                node.transform.position - node.transform.right * internal_offset +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                            tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                            tempobj.name = "el";
                            tempobj.transform.parent = node.transform;
                        }

                        if (node.iu_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab,
                                node.transform.position + node.transform.up * internal_offset +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                            tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                            tempobj.name = "eu";
                            tempobj.transform.parent = node.transform;
                        }

                        if (node.id_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab,
                                node.transform.position - node.transform.up * internal_offset +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                            tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                            tempobj.name = "ed";
                            tempobj.transform.parent = node.transform;
                        }

                        if (node.eru_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab,
                                node.transform.position +
                                (node.transform.right * external_offset + node.transform.up * offset) +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                            tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                            tempobj.name = "eru";
                            tempobj.transform.parent = node.transform;

                        }

                        if (node.erd_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab,
                                node.transform.position +
                                (node.transform.right * external_offset - node.transform.up * offset) +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                            tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                            tempobj.name = "erd";
                            tempobj.transform.parent = node.transform;

                        }

                        if (node.elu_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab,
                                node.transform.position +
                                (-node.transform.right * external_offset + node.transform.up * offset) +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                            tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                            tempobj.name = "elu";
                            tempobj.transform.parent = node.transform;

                        }

                        if (node.eld_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab,
                                node.transform.position +
                                (-node.transform.right * external_offset - node.transform.up * offset) +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                            tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                            tempobj.name = "eld";
                            tempobj.transform.parent = node.transform;

                        }

                        if (node.eur_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab,
                                node.transform.position +
                                (node.transform.right * offset + node.transform.up * external_offset) +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                            tempobj.transform.localEulerAngles = new Vector3(0, 0, 90);
                            tempobj.name = "eur";
                            tempobj.transform.parent = node.transform;

                        }

                        if (node.eul_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab,
                                node.transform.position +
                                (-node.transform.right * offset + node.transform.up * external_offset) +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                            tempobj.transform.localEulerAngles = new Vector3(0, 0, 90);
                            tempobj.name = "eul";
                            tempobj.transform.parent = node.transform;

                        }

                        if (node.edr_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab,
                                node.transform.position +
                                (node.transform.right * offset - node.transform.up * external_offset) +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                            tempobj.transform.localEulerAngles = new Vector3(0, 0, 90);
                            tempobj.name = "edr";
                            tempobj.transform.parent = node.transform;

                        }

                        if (node.edl_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab,
                                node.transform.position +
                                (-node.transform.right * offset - node.transform.up * external_offset) +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                            tempobj.transform.localEulerAngles = new Vector3(0, 0, 90);
                            tempobj.name = "edl";
                            tempobj.transform.parent = node.transform;
                        }

                        if (node.eeru_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab,
                                node.transform.position +
                                (node.transform.right + node.transform.up) * external_offset +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localScale = new Vector3(h_size, h_size, h_size);
                            tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                            tempobj.name = "eeru";
                            tempobj.transform.parent = node.transform;
                        }

                        if (node.eerd_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab,
                                node.transform.position +
                                (node.transform.right - node.transform.up) * external_offset +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localScale = new Vector3(h_size, h_size, h_size);
                            tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                            tempobj.name = "eerd";
                            tempobj.transform.parent = node.transform;
                        }

                        if (node.eelu_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab,
                                node.transform.position +
                                (-node.transform.right + node.transform.up) * external_offset +
                                node.transform.forward * height_offset, Quaternion.identity, node.transform);
                            tempobj.transform.localScale = new Vector3(h_size, h_size, h_size);
                            tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                            tempobj.name = "eelu";
                            tempobj.transform.parent = node.transform;
                        }

                        if (node.eeld_render)
                        {
                            GameObject tempobj = (GameObject)Instantiate(MazeWallPrefab,
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
            if (Application.platform == RuntimePlatform.Android)
            {
                WWW reader = new WWW(directory);
                while (!reader.isDone) { }

                jsonString = reader.text;
            }
            else
            {
                jsonString = File.ReadAllText(directory);
            }
            state = JsonUtility.FromJson<SaveState>(jsonString);
            return state;
        }
    }

    [System.Serializable]
    public class SaveState
    {
        //list of all the maze cubes along with its list of attached nodes
        public List<Maze_data> m;
    }

    [System.Serializable]
    public class Maze_data
    {
        public List<MazeCube_data> c;
        public int x;
        public int y;
        public int z;
    }

    [System.Serializable]
    public class MazeCube_data
    {
        //maze cube transform
        public int x;
        public int y;
        public int z;

        //the list of nodes the cube has
        public List<Maze.SavableNode> nl;

        public void ConvertToSavable(GameObject mace_cube)
        {
            x = (int)mace_cube.transform.position.x;
            y = (int)mace_cube.transform.position.y;
            z = (int)mace_cube.transform.position.z;

            nl = new List<Maze.SavableNode>();
            for (int j = 0; j < mace_cube.transform.childCount; j++)
            {
                Maze.SavableNode sn = new Maze.SavableNode();

                sn.ConvertToSavable(mace_cube.transform.GetChild(j).GetComponent<Maze.Node>());
                nl.Add(sn);
            }
        }
    }
}