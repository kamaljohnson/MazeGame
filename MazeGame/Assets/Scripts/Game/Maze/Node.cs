using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Maze
{
    public class Node : MonoBehaviour
    {
        public Vector3 parentCubePos;
        public Vector3 nodeTransformPos;
        public Vector3 nodeTransformRot;

        public bool inactive;

        //path status
        public bool rightPath;
        public bool leftPath;
        public bool upPath;
        public bool downPath;

        //neighbour hood node status
        public Node rightNode;
        public Node leftNode;
        public Node upNode;
        public Node downNode;

        //rendering status
        //sides
        public bool rrender;
        public bool lrender;
        public bool urender;
        public bool drender;
        //corners
        public bool rUrender;
        public bool rDrender;
        public bool lUrender;
        public bool lDrender;

        public bool eRrender;
        public bool eLrender;
        public bool eUrender;
        public bool eDrender;

        public bool erUrender;
        public bool erDrender;
        public bool elUrender;
        public bool elDrender;

        public bool euRrender;
        public bool euLrender;
        public bool edRrender;
        public bool edLrender;

        public bool eerUrender;
        public bool eerDrender;
        public bool eelUrender;
        public bool eelDrender;

        public void SetNodeFromNode(Node node, Vector3 parentCubePos)
        {
            inactive = node.inactive;
            
            rightPath = node.rightPath;
            leftPath = node.leftPath;
            upPath = node.upPath;
            downPath = node.downPath;

            this.parentCubePos = parentCubePos;
            nodeTransformPos = transform.position;
            nodeTransformRot = transform.eulerAngles;
        }

        public void ReCalculateNeighbourInterations()
        {
            rightPath = rightNode != null;
            leftPath = leftNode != null;
            upPath = upNode != null;
            downPath = downNode != null;
        }

        public void CalculatePathDirection(Node other)
        {
            if (Vector3.Distance(other.transform.position, transform.position + transform.right) < 0.1f ||
                Vector3.Distance(other.transform.position, parentCubePos + transform.right * 0.5f) < 0.1f ||
                Vector3.Distance(other.transform.position, transform.position + (transform.forward + transform.right) * .5f) < 0.1f)
            {
                if (!other.inactive && !inactive)
                {
                    rightPath = !rightPath;
                }
                else
                {
                    rightPath = true;
                }

                if (rightPath)
                {
                    rightNode = other;
                }
                else
                {
                    rightNode = null;
                }
            }
            if (Vector3.Distance(other.transform.position, transform.position - transform.right) < 0.1f ||
                Vector3.Distance(other.transform.position, parentCubePos - transform.right * 0.5f) < 0.1f ||
                Vector3.Distance(other.transform.position, transform.position + (transform.forward - transform.right) * .5f) < 0.1f)
            {
                if (!other.inactive && !inactive)
                {
                    leftPath = !leftPath;
                }
                else
                {
                    leftPath = true;
                }
                
                if (leftPath)
                {
                    leftNode = other;
                }
                else
                {
                    leftNode = null;
                }
            }
            if (Vector3.Distance(other.transform.position, transform.position + transform.up) < 0.1f ||
                Vector3.Distance(other.transform.position, parentCubePos + transform.up * 0.5f) < 0.1f ||
                Vector3.Distance(other.transform.position, transform.position + (transform.forward + transform.up) * .5f) < 0.1f)
            {
                if (!other.inactive && !inactive)
                {
                    upPath = !upPath;
                }
                else
                {
                    upPath = true;
                }
                
                if (upPath)
                {
                    upNode = other;
                }
                else
                {
                    upNode = null;
                }
            }
            if (Vector3.Distance(other.transform.position, transform.position - transform.up) < 0.1f ||
                Vector3.Distance(other.transform.position, parentCubePos - transform.up * 0.5f) < 0.1f ||
                Vector3.Distance(other.transform.position, transform.position + (transform.forward - transform.up) * .5f) < 0.1f)
            {
                if (!other.inactive && !inactive)
                {
                    downPath = !downPath;
                }
                else
                {
                    downPath = true;
                }
                
                if (downPath)
                {
                    downNode = other;
                }
                else
                {
                    downNode = null;
                }
            }
        }

        public void CalculateRenderNodePath()
        {
            RaycastHit hit;

            rrender = false;
            lrender = false;
            urender = false;
            drender = false;

            rUrender = false;
            rDrender = false;
            lUrender = false;
            lDrender = false;

            eRrender = false;
            eLrender = false;
            eUrender = false;
            eDrender = false;

            erUrender = false;
            erDrender = false;
            elUrender = false;
            elDrender = false;

            euRrender = false;
            euLrender = false;
            edRrender = false;
            edLrender = false;

            eerUrender = false;
            eerDrender = false;
            eelUrender = false;
            eelDrender = false;

            if (!inactive)
            {
                rrender = !rightPath;
                lrender = !leftPath;
                urender = !upPath;
                drender = !downPath;

                rDrender = true;
                rUrender = true;
                lDrender = true;
                lUrender = true;
            }
            
            if (!Physics.Raycast(parentCubePos, transform.right * 0.6f, out hit, 0.5f))
            {
                Debug.DrawRay(parentCubePos, transform.right * 0.6f, Color.cyan, 200f);
                if (!inactive)
                {
                    erUrender = true;
                    erDrender = true;
                    eRrender = rrender;
                }
                else
                {
                    rUrender = true;
                    rDrender = true;
                    rrender = !rightPath;
                }
            }

            if (!Physics.Raycast(parentCubePos, -transform.right * 0.6f, out hit, 0.5f))
            {
                Debug.DrawRay(parentCubePos, -transform.right * 0.6f, Color.cyan, 200f);
                if (!inactive)
                {
                    elUrender = true;
                    elDrender = true;
                    eLrender = lrender;
                }
                else
                {
                    lUrender = true;
                    lDrender = true;
                    lrender = !leftPath;
                }
            }          

            if (!Physics.Raycast(parentCubePos, transform.up * 0.6f, out hit, 0.5f))
            {
                Debug.DrawRay(parentCubePos, transform.up * 0.6f, Color.cyan, 200f);
                if (!inactive)
                {
                    euRrender = true;
                    euLrender = true;
                    eUrender = urender;
                }
                else
                {
                    rUrender = true;
                    lUrender = true;
                    urender = !upPath;
                }
            }

            if (!Physics.Raycast(parentCubePos, -transform.up * 0.6f, out hit, 0.5f))
            {
                Debug.DrawRay(parentCubePos, -transform.up * 0.6f, Color.cyan, 200f);
                if (!inactive)
                {
                    edRrender = true;
                    edLrender = true;
                    eDrender = drender;
                }
                else
                {
                    rDrender = true;
                    lDrender = true;
                    drender = !downPath;
                }
            }

            if (erUrender && euRrender)
            {
                eerUrender = true;
            }
            if (elUrender && euLrender)
            {
                eelUrender = true;
            }
            if (erDrender && edRrender)
            {
                eerDrender = true;
            }
            if (elDrender && edLrender)
            {
                eelDrender = true;
            }
        }

        public void Reset()
        {
            inactive = false;

            rightPath = false;
            leftPath = false;
            upPath = false;
            downPath = false;

            rightNode = null;
            leftNode = null;
            upNode = null;
            downNode = null;


            rrender = false;
            lrender = false;
            urender = false;
            drender = false;

            rUrender = false;
            rDrender = false;
            lUrender = false;
            lDrender = false;

            eRrender = false;
            eLrender = false;
            eUrender = false;
            eDrender = false;

            erUrender = false;
            erDrender = false;
            elUrender = false;
            elDrender = false;

            euRrender = false;
            euLrender = false;
            edRrender = false;
            edLrender = false;

            eerUrender = false;
            eerDrender = false;
            eelUrender = false;
            eelDrender = false;
        }
    }


    [Serializable]
    public class SavableNode
    {
        //inactivity
        public int i;

        //node transform rotation
        public int u;
        public int v;
        public int w;

        //node path data
        public int p;

        public void ConvertToSavable(Node node)
        {
            i = node.inactive ? 1 : 0;

            var eulerAngles = node.transform.eulerAngles;
            u = (int)eulerAngles.x;
            v = (int)eulerAngles.y;
            w = (int)eulerAngles.z;

            string tempBin;
            
            tempBin = (node.rightPath ? "1" : "0") +
                (node.leftPath ? "1" : "0") +
                (node.upPath ? "1" : "0") +
                (node.downPath ? "1" : "0");    
            
            p = Convert.ToInt32(tempBin, 2);
        }

        public Node GetNode()
        {
            Node node = new Node();

            node.inactive = i == 1;

            node.nodeTransformRot = new Vector3(u, v, w);

            //converting p to binary and then to individual ints
            int  n, k;       
            int[] tempPathData = new int[4];
            for (int j = 0; j < 4; j++)
            {
                tempPathData[j] = 0;
            }
            n = p;     
            for(k=0; n>0; k++)      
            {      
                tempPathData[k]=n%2;      
                n = n/2;    
            }

            node.rightPath = tempPathData[3] == 1;
            node.leftPath = tempPathData[2] == 1;
            node.upPath = tempPathData[1] == 1;
            node.downPath = tempPathData[0] == 1;    

            return node;
        }
    }
}