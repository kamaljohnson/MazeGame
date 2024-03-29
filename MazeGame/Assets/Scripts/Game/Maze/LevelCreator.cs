﻿using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Game.Items;
using Game.Items.Activators.Button;
using Game.Items.Collectables.Coin;
using Game.Items.Collectables.Diamond;
using Game.Items.Enemies.Blade;
using Game.Items.Enemies.Guardian;
using Game.Items.Enemies.Hammer;
using Game.Items.Intractable.Gate;
using Game.Items.Intractable.Laser;
using Game.Items.Intractable.Portal;
using Game.Items.Intractable.Spike;
using Game.Managers;
using Game.Maze;
using Game.Player;
using UnityEngine.SceneManagement;

namespace Game.Maze
{
    public class LevelCreator : MonoBehaviour
    {
        [Header("Maze Properties")]
        public Transform mazeHolder;
        public GameObject mazeCubePrefab;
        public GameObject mazeWallPrefab;
        
        //the item prefabs
        [Header("Maze Items Properties")]
        public GameObject portalPrefab;
        public GameObject spikePrefab;
        public GameObject laserPrefab;
        public GameObject gatePrefab;
        public GameObject buttonPrefab;
        public GameObject icePrefab;
        public GameObject hammerPrefab;
        public GameObject guardianPrefab;
        public GameObject bladePrefab;
        public GameObject knightPrefab;
        public GameObject firePrefab;
        public GameObject coinPrefab;
        public GameObject diamondPrefab;

        [Header("General Properties")]
        public GameObject playerCube;
        public GameObject inputManager;
        private Vector3 _playerStartPosition;

        public void Start()
        {
            try
            {
                GameManager.stateManager.LoadLevelState();
            }
            catch (Exception exception)
            {
                Debug.Log("error -> " + exception);
            }
            LoadLevel();
            GameManager.gameState = GameManager.GameStates.Playing;
        }

        private void LoadLevel()
        {
            var state = LoadLevelDataFromFile(GameManager.levelName);
            
            Button.AllInteractableItems = new List<IIntractables>();
            /*
             * Calculating the render data from the path data
             */
            foreach (var maze in state.m)
            {
                var tempMaze = new GameObject();
                var mazeCubes = new GameObject();
                var mazeNodes = new GameObject();
                var mazeWalls = new GameObject();
                tempMaze.AddComponent<MazeRotator>();
                tempMaze.transform.parent = mazeHolder;
                tempMaze.transform.position = new Vector3(
                    maze.x,
                    maze.y,
                    maze.z
                );
                tempMaze.name = "Maze";
                mazeCubes.transform.parent = tempMaze.transform;
                mazeCubes.name = "Cubes";
                mazeCubes.AddComponent<MeshFilter>();
                mazeCubes.AddComponent<MeshRenderer>();
                mazeCubes.AddComponent<MeshCollider>();
                mazeNodes.transform.parent = tempMaze.transform;
                mazeNodes.name = "Nodes";
                mazeWalls.transform.parent = tempMaze.transform;
                mazeWalls.name = "Walls";
                mazeWalls.AddComponent<MeshFilter>();
                mazeWalls.AddComponent<MeshRenderer>();
                mazeWalls.AddComponent<MeshCollider>();

                var tempMazeCubes = new List<GameObject>();
                foreach (var cube in maze.c)
                {
                    var tempCube = Instantiate(mazeCubePrefab, mazeCubes.transform, true);
                    tempCube.transform.position = new Vector3(
                        cube.x,
                        cube.y,  
                        cube.z
                    );
                    tempMazeCubes.Add(tempCube);
                }    
                var i = 0;
                foreach (var cube in maze.c)
                {
                    foreach (var node in cube.nl)
                    {
                        var tempNode = new GameObject();
                        tempNode.transform.parent = tempMazeCubes[i].transform;
                        tempNode.transform.eulerAngles = new Vector3(
                            node.u,
                            node.v,
                            node.w
                        );
                        tempNode.transform.localPosition = tempNode.transform.forward * 0.5f;
                        tempNode.transform.parent = mazeNodes.transform;
                        tempNode.gameObject.AddComponent<Node>();

                        var tempNodeObj = node.GetNode();
                        tempNode.gameObject.GetComponent<Node>().SetNodeFromNode(tempNodeObj, tempMazeCubes[i].transform.position);
                        tempNode.gameObject.GetComponent<Node>().CalculateRenderNodePath();
                    }
                    i++;
                }

                foreach (var spike in maze.s)
                {
                    var tempSpike = Instantiate(spikePrefab, tempMaze.transform, true);
                    tempSpike.GetComponent<Collider>().enabled = false;
                    
                    tempSpike.transform.position = new Vector3(
                        spike.x,
                        spike.y,
                        spike.z
                        );                    
                    tempSpike.transform.eulerAngles = new Vector3(
                        spike.u,
                        spike.v,
                        spike.w
                    );
                    tempSpike.GetComponent<Spike>().SetSpikeValues(spike.GetSpike());

                    Button.AllInteractableItems.Add(tempSpike.GetComponent<Spike>());
                }
                
                foreach (var portal in maze.p)
                {
                    var tempPortal = Instantiate(portalPrefab, tempMaze.transform, true);
                    tempPortal.GetComponent<Collider>().enabled = true;
                    
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
                    if (tempPortal.GetComponent<Portal>().portalId == 0)    //the starting of the maze
                    {
                        //TODO: replace this code with the Portal.GoToPortal(portalID = 0)
                        var playerCube = Instantiate(this.playerCube);
                        playerCube.GetComponent<Movement>().input = inputManager.GetComponent<PlayerInput>();
                        playerCube.GetComponent<Movement>().SetParentMaze(mazeHolder.GetChild(tempPortal.GetComponent<Portal>().mazeId).gameObject);
                        playerCube.transform.position = tempPortal.transform.position - playerCube.transform.up * 1/3f;
                        playerCube.transform.eulerAngles = tempPortal.transform.eulerAngles;
                        //tempPortal.SetActive(false);
                        GameManager.playerCubeTransform = playerCube.transform;
                        GameManager.mazeTransform = mazeHolder.GetChild(tempPortal.GetComponent<Portal>().mazeId);
                        tempPortal.GetComponent<Portal>().CheckpointSaveGameState();

                    }

                    Button.AllInteractableItems.Add(tempPortal.GetComponent<Portal>());
                }

                foreach (var gate in maze.g)
                {
                    var tempGate = Instantiate(gatePrefab, tempMaze.transform);
                    
                    tempGate.transform.GetChild(0).GetChild(0).GetComponent<Collider>().enabled = true;
                    tempGate.transform.GetChild(0).GetChild(1).GetComponent<Collider>().enabled = true;
                    tempGate.transform.GetChild(0).GetChild(2).GetComponent<Collider>().enabled = true;
                    
                    tempGate.transform.position = new Vector3(
                        gate.x,
                        gate.y,
                        gate.z
                    );                    
                    tempGate.transform.eulerAngles = new Vector3(
                        gate.u,
                        gate.v,
                        gate.w
                    );
                    tempGate.GetComponent<Gate>().SetGateValues(gate.GetGate());
                    tempGate.GetComponent<Collider>().enabled = false;
                    tempGate.GetComponent<Gate>().CloseGate();
                    
                    Button.AllInteractableItems.Add(tempGate.GetComponent<Gate>());
                }
                
                foreach (var laser in maze.l)
                {
                    var tempLaser = Instantiate(laserPrefab, tempMaze.transform);
                    
                    
                    tempLaser.transform.position = new Vector3(
                        laser.x,
                        laser.y,
                        laser.z
                    );                    
                    tempLaser.transform.eulerAngles = new Vector3(
                        laser.u,
                        laser.v,
                        laser.w
                    );
                    tempLaser.GetComponent<Laser>().SetLaserValues(laser.GetLaser());
                    
                    Button.AllInteractableItems.Add(tempLaser.GetComponent<Laser>());
                }
                
                foreach (var button in maze.b)
                {
                    var tempButton = Instantiate(buttonPrefab, tempMaze.transform, true);
                    tempButton.GetComponent<Collider>().enabled = true;
                    tempButton.transform.GetChild(0).GetComponent<Collider>().enabled = true;

                    tempButton.transform.position = new Vector3(
                        button.x,
                        button.y,
                        button.z
                    );                    
                    tempButton.transform.eulerAngles = new Vector3(
                        button.u,
                        button.v,
                        button.w
                    );
                    
                    foreach (var intractableItem in Button.AllInteractableItems)
                    {
                        if (intractableItem.GetIntractableId() == button.i)
                        {
                            tempButton.GetComponent<Button>().interactionItem = intractableItem;
                            tempButton.GetComponent<Button>().InitButtonState(intractableItem.GetLinkedButtonState() == "f");
                            tempButton.GetComponent<Button>().ActivateButtonEvent();
                            
                            if (intractableItem.GetItemColor() != null)
                            {
                                tempButton.transform.GetChild(0).GetComponent<MeshRenderer>().material.color =
                                    intractableItem.GetItemColor();
                            }
                        }
                    }
                }

                foreach (var hammer in maze.h)
                {
                    var tempHammer = Instantiate(hammerPrefab, tempMaze.transform);
                    
                    tempHammer.transform.position = new Vector3(
                        hammer.x,
                        hammer.y,
                        hammer.z
                    );                    
                    tempHammer.transform.eulerAngles = new Vector3(
                        hammer.u,
                        hammer.v,
                        hammer.w
                    );
                }
                
                foreach (var guardian in maze.gr)
                {
                    var tempGuardian = Instantiate(guardianPrefab, tempMaze.transform);
                    
                    tempGuardian.transform.position = new Vector3(
                        guardian.x,
                        guardian.y,
                        guardian.z
                    );                    
                    tempGuardian.transform.eulerAngles = new Vector3(
                        guardian.u,
                        guardian.v,
                        guardian.w
                    );
                    tempGuardian.GetComponent<Guardian>().SetGuardianValues(guardian.GetGuardian());
                }
                
                foreach (var blade in maze.bl)
                {
                    var tempBlade = Instantiate(bladePrefab, tempMaze.transform);
                    
                    tempBlade.transform.position = new Vector3(
                        blade.x,
                        blade.y,
                        blade.z
                    );                    
                    tempBlade.transform.eulerAngles = new Vector3(
                        blade.u,
                        blade.v,
                        blade.w
                    );
                    tempBlade.GetComponent<Blade>().SetBladeValues(blade.GetBlade());
                }
                
                foreach (var knight in maze.k)
                {
                    var tempKnight = Instantiate(knightPrefab, tempMaze.transform);
                    
                    tempKnight.transform.position = new Vector3(
                        knight.x,
                        knight.y,
                        knight.z
                    );                    
                    tempKnight.transform.eulerAngles = new Vector3(
                        knight.u,
                        knight.v,
                        knight.w
                    );
                }
                
                foreach (var ice in maze.i)
                {
                    var tempIce = Instantiate(icePrefab, tempMaze.transform, true);
                    tempIce.GetComponent<Collider>().enabled = true;
                    tempIce.transform.GetChild(0).GetComponent<Collider>().enabled = true;
                    
                    tempIce.transform.position = new Vector3(
                        ice.x,
                        ice.y,
                        ice.z
                    );                    
                    tempIce.transform.eulerAngles = new Vector3(
                        ice.u,
                        ice.v,
                        ice.w
                    );

                }
                
                foreach (var fire in maze.f)
                {
                    var tempFire = Instantiate(firePrefab, tempMaze.transform, true);
                    tempFire.GetComponent<Collider>().enabled = true;
                    tempFire.transform.GetChild(0).GetComponent<Collider>().enabled = true;
                    
                    tempFire.transform.position = new Vector3(
                        fire.x,
                        fire.y,
                        fire.z
                    );                    
                    tempFire.transform.eulerAngles = new Vector3(
                        fire.u,
                        fire.v,
                        fire.w
                    );
                }

                int currentCoinIndex = -1;
                
                foreach (var coin in maze.co)
                {
                    currentCoinIndex++;
                    if (GameManager.stateManager.indexOfCoinsCollected.Contains(currentCoinIndex))
                    {
                        continue;
                    }
                    var tempCoin = Instantiate(coinPrefab, tempMaze.transform, true);
                    tempCoin.GetComponent<Collider>().enabled = true;
                    tempCoin.transform.GetComponent<Collider>().enabled = true;
                    
                    tempCoin.transform.position = new Vector3(
                        coin.x,
                        coin.y,
                        coin.z
                    );                    
                    tempCoin.transform.eulerAngles = new Vector3(
                        coin.u,
                        coin.v,
                        coin.w
                    );
                    tempCoin.GetComponent<Coin>().index = currentCoinIndex;
                }
                
                int currentDiamondIndex = -1;

                foreach (var diamond in maze.d)
                {
                    currentDiamondIndex++;
                    if (GameManager.stateManager.indexOfDiamondsCollected.Contains(currentDiamondIndex))
                    {
                        continue;
                    }
                    var tempDiamond = Instantiate(diamondPrefab, tempMaze.transform, true);
                    tempDiamond.GetComponent<Collider>().enabled = true;
                    tempDiamond.transform.GetComponent<Collider>().enabled = true;
                    
                    tempDiamond.transform.position = new Vector3(
                        diamond.x,
                        diamond.y,
                        diamond.z
                    );                    
                    tempDiamond.transform.eulerAngles = new Vector3(
                        diamond.u,
                        diamond.v,
                        diamond.w
                    );
                    tempDiamond.GetComponent<Diamond>().index = currentDiamondIndex;
                }
                
                
                
            }

            /*
             * Creates individual blocks of the maze walls using the render_* data from the nodes
             * Combining all the wall meshes
             * Combining all the body cube meshes
             */
            for (int i = 0; i < mazeHolder.childCount; i++)     //for each maze
            {
                Transform maze = mazeHolder.GetChild(i);
                Transform cubes = maze.GetChild(0);
                Transform nodes = maze.GetChild(1);
                Transform walls = maze.GetChild(2);
                
                /*
                 * creating the walls and adding them to the walls gameObject
                 */
                for (int k = 0; k < nodes.childCount; k++)      //for each node
                {
                    Node node = nodes.GetChild(k).GetComponent<Node>();
                    if (node == null)
                        break;
                    
                    float offset = 1 / 2f - 1 / 12f;
                    float height_offset = 1 / 12f;

                    float external_offset = 1 / 2f + 1 / 12f;

                    float w_size = 4 / 6f;
                    float c_size = 1 / 6f;
                    float h_size = 1 / 6f;

                    var nodeTransform = node.transform;
                    
                    if (node.rrender)
                    {
                        GameObject tempobj = Instantiate(mazeWallPrefab,nodeTransform.position +nodeTransform.right * offset +nodeTransform.forward * height_offset, Quaternion.identity,nodeTransform);
                        tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                        tempobj.name = "r";
                        tempobj.transform.parent = walls.transform;
                    }
                    if (node.lrender)
                    {
                        GameObject tempobj = Instantiate(mazeWallPrefab,nodeTransform.position -nodeTransform.right * offset +nodeTransform.forward * height_offset, Quaternion.identity,nodeTransform);
                        tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                        tempobj.name = "l";
                        tempobj.transform.parent = walls.transform;
                    }
                    if (node.urender)
                    {
                        GameObject tempobj = Instantiate(mazeWallPrefab,nodeTransform.position +nodeTransform.up * offset +nodeTransform.forward * height_offset, Quaternion.identity,nodeTransform);
                        tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                        tempobj.name = "u";
                        tempobj.transform.parent = walls.transform;
                    }
                    if (node.drender)
                    {
                        GameObject tempobj = Instantiate(mazeWallPrefab,nodeTransform.position -nodeTransform.up * offset +nodeTransform.forward * height_offset, Quaternion.identity,nodeTransform);
                        tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, c_size);
                        tempobj.name = "d";
                        tempobj.transform.parent = walls.transform;
                    }

                    //rendering corner
                    if (node.rUrender)
                    {
                        GameObject tempobj = Instantiate(mazeWallPrefab,
                           nodeTransform.position + (node.transform.right +nodeTransform.up) * offset +
                           nodeTransform.forward * height_offset,nodeTransform.rotation,nodeTransform);
                        tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                        tempobj.name = "ru";
                        tempobj.transform.parent = walls.transform;
                    }
                    if (node.rDrender)
                    {
                        GameObject tempobj = Instantiate(mazeWallPrefab,
                           nodeTransform.position + (node.transform.right -nodeTransform.up) * offset +
                           nodeTransform.forward * height_offset,nodeTransform.rotation,nodeTransform);
                        tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                        tempobj.name = "rd";
                        tempobj.transform.parent = walls.transform;
                    }
                    if (node.lUrender)
                    {
                        GameObject tempobj = Instantiate(mazeWallPrefab,
                           nodeTransform.position + (-node.transform.right +nodeTransform.up) * offset +
                           nodeTransform.forward * height_offset,nodeTransform.rotation,nodeTransform);
                        tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                        tempobj.name = "lu";
                        tempobj.transform.parent = walls.transform;
                    }
                    if (node.lDrender)
                    {
                        GameObject tempobj = Instantiate(mazeWallPrefab,
                           nodeTransform.position + (-node.transform.right -nodeTransform.up) * offset +
                           nodeTransform.forward * height_offset,nodeTransform.rotation,nodeTransform);
                        tempobj.transform.localScale = new Vector3(c_size, c_size, h_size);
                        tempobj.name = "ld";
                        tempobj.transform.parent = walls.transform;
                    }

                    //rendering external edges
                    if (node.eRrender)
                    {
                        GameObject tempobj = Instantiate(mazeWallPrefab,nodeTransform.position +nodeTransform.right * external_offset +nodeTransform.forward * height_offset, Quaternion.identity,nodeTransform);
                        tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                        tempobj.name = "er";
                        tempobj.transform.parent = walls.transform;
                    }

                    if (node.eLrender)
                    {
                        GameObject tempobj = Instantiate(mazeWallPrefab,
                           nodeTransform.position -nodeTransform.right * external_offset +
                           nodeTransform.forward * height_offset, Quaternion.identity,nodeTransform);
                        tempobj.transform.localEulerAngles = new Vector3(0, 90, 90);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                        tempobj.name = "el";
                        tempobj.transform.parent = walls.transform;
                    }

                    if (node.eUrender)
                    {
                        GameObject tempobj = Instantiate(mazeWallPrefab,
                           nodeTransform.position +nodeTransform.up * external_offset +
                           nodeTransform.forward * height_offset, Quaternion.identity,nodeTransform);
                        tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                        tempobj.name = "eu";
                        tempobj.transform.parent = walls.transform;
                    }

                    if (node.eDrender)
                    {
                        GameObject tempobj = Instantiate(mazeWallPrefab,
                           nodeTransform.position -nodeTransform.up * external_offset +
                           nodeTransform.forward * height_offset, Quaternion.identity,nodeTransform);
                        tempobj.transform.localEulerAngles = new Vector3(-90, 0, 0);
                        tempobj.transform.localScale = new Vector3(w_size, h_size, h_size);
                        tempobj.name = "ed";
                        tempobj.transform.parent = walls.transform;
                    }

                    if (node.erUrender)
                    {
                        GameObject tempobj = Instantiate(mazeWallPrefab,
                           nodeTransform.position +
                            (node.transform.right * external_offset +nodeTransform.up * offset) +
                           nodeTransform.forward * height_offset, Quaternion.identity,nodeTransform);
                        tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                        tempobj.name = "eru";
                        tempobj.transform.parent = walls.transform;

                    }

                    if (node.erDrender)
                    {
                        GameObject tempobj = Instantiate(mazeWallPrefab,
                           nodeTransform.position +
                            (node.transform.right * external_offset -nodeTransform.up * offset) +
                           nodeTransform.forward * height_offset, Quaternion.identity,nodeTransform);
                        tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                        tempobj.name = "erd";
                        tempobj.transform.parent = walls.transform;

                    }

                    if (node.elUrender)
                    {
                        GameObject tempobj = Instantiate(mazeWallPrefab,
                           nodeTransform.position +
                            (-node.transform.right * external_offset +nodeTransform.up * offset) +
                           nodeTransform.forward * height_offset, Quaternion.identity,nodeTransform);
                        tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                        tempobj.name = "elu";
                        tempobj.transform.parent = walls.transform;

                    }

                    if (node.elDrender)
                    {
                        GameObject tempobj = Instantiate(mazeWallPrefab,
                           nodeTransform.position +
                            (-node.transform.right * external_offset -nodeTransform.up * offset) +
                           nodeTransform.forward * height_offset, Quaternion.identity,nodeTransform);
                        tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                        tempobj.name = "eld";
                        tempobj.transform.parent = walls.transform;

                    }

                    if (node.euRrender)
                    {
                        GameObject tempobj = Instantiate(mazeWallPrefab,
                           nodeTransform.position +
                            (node.transform.right * offset +nodeTransform.up * external_offset) +
                           nodeTransform.forward * height_offset, Quaternion.identity,nodeTransform);
                        tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 90);
                        tempobj.name = "eur";
                        tempobj.transform.parent = walls.transform;

                    }

                    if (node.euLrender)
                    {
                        GameObject tempobj = Instantiate(mazeWallPrefab,
                           nodeTransform.position +
                            (-node.transform.right * offset +nodeTransform.up * external_offset) +
                           nodeTransform.forward * height_offset, Quaternion.identity,nodeTransform);
                        tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 90);
                        tempobj.name = "eul";
                        tempobj.transform.parent = walls.transform;

                    }

                    if (node.edRrender)
                    {
                        GameObject tempobj = Instantiate(mazeWallPrefab,
                           nodeTransform.position +
                            (node.transform.right * offset -nodeTransform.up * external_offset) +
                           nodeTransform.forward * height_offset, Quaternion.identity,nodeTransform);
                        tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 90);
                        tempobj.name = "edr";
                        tempobj.transform.parent = walls.transform;

                    }

                    if (node.edLrender)
                    {
                        GameObject tempobj = Instantiate(mazeWallPrefab,
                           nodeTransform.position +
                            (-node.transform.right * offset -nodeTransform.up * external_offset) +
                           nodeTransform.forward * height_offset, Quaternion.identity,nodeTransform);
                        tempobj.transform.localScale = new Vector3(h_size, c_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 90);
                        tempobj.name = "edl";
                        tempobj.transform.parent = walls.transform;
                    }

                    if (node.eerUrender)
                    {
                        GameObject tempobj = Instantiate(mazeWallPrefab,
                           nodeTransform.position +
                            (node.transform.right +nodeTransform.up) * external_offset +
                           nodeTransform.forward * height_offset, Quaternion.identity,nodeTransform);
                        tempobj.transform.localScale = new Vector3(h_size, h_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                        tempobj.name = "eeru";
                        tempobj.transform.parent = walls.transform;
                    }

                    if (node.eerDrender)
                    {
                        GameObject tempobj = Instantiate(mazeWallPrefab,
                           nodeTransform.position +
                            (node.transform.right -nodeTransform.up) * external_offset +
                           nodeTransform.forward * height_offset, Quaternion.identity,nodeTransform);
                        tempobj.transform.localScale = new Vector3(h_size, h_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                        tempobj.name = "eerd";
                        tempobj.transform.parent = walls.transform;
                    }

                    if (node.eelUrender)
                    {
                        GameObject tempobj = Instantiate(mazeWallPrefab,
                           nodeTransform.position +
                            (-node.transform.right +nodeTransform.up) * external_offset +
                           nodeTransform.forward * height_offset, Quaternion.identity,nodeTransform);
                        tempobj.transform.localScale = new Vector3(h_size, h_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                        tempobj.name = "eelu";
                        tempobj.transform.parent = walls.transform;
                    }

                    if (node.eelDrender)
                    {
                        GameObject tempobj = Instantiate(mazeWallPrefab,
                           nodeTransform.position +
                            (-node.transform.right -nodeTransform.up) * external_offset +
                           nodeTransform.forward * height_offset, Quaternion.identity,nodeTransform);
                        tempobj.transform.localScale = new Vector3(h_size, h_size, h_size);
                        tempobj.transform.localEulerAngles = new Vector3(0, 0, 0);
                        tempobj.name = "eeld";
                        tempobj.transform.parent = walls.transform;
                    }
                }
                Destroy(maze.GetChild(1).gameObject);
                
                /*
                 * combining all the cube meshes to mazeCubes
                 * combining all the wall meshes to mazeWalls
                 */
                CombineMeshes(cubes.gameObject);
                cubes.gameObject.GetComponent<Renderer>().material = mazeCubePrefab.GetComponent<Renderer>().sharedMaterial;
                CombineMeshes(walls.gameObject);
                walls.gameObject.GetComponent<Renderer>().material = mazeWallPrefab.GetComponent<Renderer>().sharedMaterial;

            }
            mazeHolder.transform.localScale = Vector3.one * 5; 
        }

        public void CombineMeshes(GameObject parentGameObject)
        {
            var meshFilters = parentGameObject.GetComponentsInChildren<MeshFilter>();
            var combine = new CombineInstance[meshFilters.Length];

            var i = 0;
            while (i < meshFilters.Length)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(false);
                i++;
            }
            parentGameObject.GetComponent<MeshFilter>().mesh = new Mesh();
            parentGameObject.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
            parentGameObject.SetActive(true);
            parentGameObject.GetComponent<MeshCollider>().sharedMesh = parentGameObject.GetComponent<MeshFilter>().mesh;
        }

        public SaveState LoadLevelDataFromFile(string levelName)
        {
            string directory = Application.streamingAssetsPath + "/Levels/" + levelName;
            SaveState state;

            string jsonString;
            if(Application.platform == RuntimePlatform.Android)
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

    [Serializable]
    public class SaveState
    {
        //list of all the maze cubes along with its list of attached nodes
        public List<MazeData> m;
    }

    [Serializable]
    public class MazeData
    {
        public List<MazeCubeData> c;
        public int x;
        public int y;
        public int z;

        //item data
        public List<Items.Intractable.Portal.SerializableItem> p;     //Portal
        public List<Items.Intractable.Gate.SerializableItem> g;       //Gate
        public List<Items.Intractable.Laser.SerializableItem> l;      //Laser
        public List<Items.Intractable.Spike.SerializableItem> s;      //Spike
        
        public List<Items.Activators.Button.SerializableItem> b;      //Button
        
        public List<Items.Enemies.Hammer.SerializableItem> h;         //Hammer
        public List<Items.Enemies.Guardian.SerializableItem> gr;      //Guardian
        public List<Items.Enemies.Blade.SerializableItem> bl;         //Blade
        public List<Items.Enemies.Knight.SerializableItem> k;         //Knight
        
        public List<Items.Path.Ice.SerializableItem> i;               //Ice
        public List<Items.Path.Fire.SerializableItem> f;              //Fire

        public List<Items.Collectables.Coin.SerializableItem> co;     //Coin
        public List<Items.Collectables.Diamond.SerializableItem> d;     //Coin
    }

    [Serializable]
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