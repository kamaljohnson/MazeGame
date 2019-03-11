using System;
using UnityEngine;

namespace Game.Maze
{
    public class Node : MonoBehaviour
    {
        public Vector3 ParentCubePos;
        public Vector3 NodeTransformPos;
        public Vector3 NodeTransformRot;

        public bool inactive;

        //path status
        public bool RightPath;
        public bool LeftPath;
        public bool UpPath;
        public bool DownPath;

        //neighbour hood node status
        public Node RightNode;
        public Node LeftNode;
        public Node UpNode;
        public Node DownNode;

        //rendering status
        //sides
        public bool Rrender;
        public bool Lrender;
        public bool Urender;
        public bool Drender;
        //corners
        public bool RUrender;
        public bool RDrender;
        public bool LUrender;
        public bool LDrender;

        public bool ERrender;
        public bool ELrender;
        public bool EUrender;
        public bool EDrender;

        public bool IRrender;
        public bool ILrender;
        public bool IUrender;
        public bool IDrender;

        public bool ERUrender;
        public bool ERDrender;
        public bool ELUrender;
        public bool ELDrender;

        public bool EURrender;
        public bool EULrender;
        public bool EDRrender;
        public bool EDLrender;

        public bool EERUrender;
        public bool EERDrender;
        public bool EELUrender;
        public bool EELDrender;

        public void SetNodeFromNode(Node node, Vector3 parentCubePos)
        {
            RightPath = node.RightPath;
            LeftPath = node.LeftPath;
            UpPath = node.UpPath;
            DownPath = node.DownPath;

            ParentCubePos = parentCubePos;
            NodeTransformPos = transform.position;
            NodeTransformRot = transform.eulerAngles;
        }

        public void Start()
        {
            ParentCubePos = -transform.forward;
        }

        public void ReCalculateNeighbourInterations()
        {
            RightPath = (RightNode != null);
            LeftPath = (LeftNode != null);
            UpPath = (UpNode != null);
            DownPath = (DownNode != null);
        }

        public void CalculatePathDirection(Node other)
        {

            if (Vector3.Distance(other.transform.position, transform.position + transform.right) < 0.01f ||
                Vector3.Distance(other.transform.position, ParentCubePos + transform.right * 0.5f) < 0.01f ||
                Vector3.Distance(other.transform.position, transform.position + (transform.forward + transform.right) * .5f) < 0.01f)
            {
                if (!other.inactive && !inactive)
                {
                    RightPath = !RightPath;
                }
                else
                {
                    RightPath = true;
                }
                RightNode = other;
            }
            if (Vector3.Distance(other.transform.position, transform.position - transform.right) < 0.01f ||
                Vector3.Distance(other.transform.position, ParentCubePos - transform.right * 0.5f) < 0.01f ||
                Vector3.Distance(other.transform.position, transform.position + (transform.forward - transform.right) * .5f) < 0.01f)
            {

                if (!other.inactive && !inactive)
                {
                    LeftPath = !LeftPath;
                }
                else
                {
                    LeftPath = true;
                }
                LeftNode = other;
            }
            if (Vector3.Distance(other.transform.position, transform.position + transform.up) < 0.01f ||
                Vector3.Distance(other.transform.position, ParentCubePos + transform.up * 0.5f) < 0.01f ||
                Vector3.Distance(other.transform.position, transform.position + (transform.forward + transform.up) * .5f) < 0.01f)
            {
                if (!other.inactive && !inactive)
                {
                    UpPath = !UpPath;
                }
                else
                {
                    UpPath = true;
                }
                UpNode = other;
            }
            if (Vector3.Distance(other.transform.position, transform.position - transform.up) < 0.01f ||
                Vector3.Distance(other.transform.position, ParentCubePos - transform.up * 0.5f) < 0.01f ||
                Vector3.Distance(other.transform.position, transform.position + (transform.forward - transform.up) * .5f) < 0.01f)
            {
                if (!other.inactive && !inactive)
                {
                    DownPath = !DownPath;
                }
                else
                {
                    DownPath = true;
                }
                DownNode = other;
            }
        }

        public void CalculateRenderNodePath()
        {
            RaycastHit hit;

            Rrender = false;
            Lrender = false;
            Urender = false;
            Drender = false;

            RUrender = false;
            RDrender = false;
            LUrender = false;
            LDrender = false;

            ERrender = false;
            ELrender = false;
            EUrender = false;
            EDrender = false;

            IRrender = false;
            ILrender = false;
            IUrender = false;
            IDrender = false;

            ERUrender = false;
            ERDrender = false;
            ELUrender = false;
            ELDrender = false;

            EURrender = false;
            EULrender = false;
            EDRrender = false;
            EDLrender = false;

            EERUrender = false;
            EERDrender = false;
            EELUrender = false;
            EELDrender = false;


            if (!inactive)
            {
                Rrender = !RightPath;
                Lrender = !LeftPath;
                Urender = !UpPath;
                Drender = !DownPath;

                RDrender = true;
                RUrender = true;
                LDrender = true;
                LUrender = true;
            }
            else
            {
                if (!Physics.Raycast(ParentCubePos, transform.right, out hit, 1))
                {
                    if (Rrender)
                    {
                        RUrender = true;
                        RDrender = true;
                        ERrender = true;
                    }
                }
                if (!Physics.Raycast(ParentCubePos, -transform.right, out hit, 1))
                {
                    if (Lrender)
                    {
                        LUrender = true;
                        LDrender = true;
                        ELrender = true;
                    }
                }
                if (!Physics.Raycast(ParentCubePos, transform.up, out hit, 1))
                {
                    if (Urender)
                    {
                        RUrender = true;
                        LUrender = true;
                        EUrender = true;
                    }
                }
                if (!Physics.Raycast(ParentCubePos, -transform.up, out hit, 1))
                {
                    if (Drender)
                    {
                        RDrender = true;
                        LDrender = true;
                        EDrender = true;
                    }
                }
            }

            if (!Physics.Raycast(ParentCubePos, transform.right * 0.6f, out hit, 0.5f))
            {
                ERUrender = true;
                ERDrender = true;
                if (Rrender)
                {
                    ERrender = true;
                }
            }
            if (Physics.Raycast(transform.position + transform.forward * 0.1f, transform.right * 0.6f, out hit, 0.9f))
            {
                if (Rrender)
                {
                    IRrender = true;
                }
            }

            if (!Physics.Raycast(ParentCubePos, -transform.right * 0.6f, out hit, 0.5f))
            {
                ELUrender = true;
                ELDrender = true;
                if (Lrender)
                {
                    ELrender = true;
                }
            }
            if (Physics.Raycast(transform.position + transform.forward * 0.1f, -transform.right * 0.6f, out hit, 0.9f))
            {
                if (Lrender)
                {
                    ILrender = true;
                }
            }

            if (!Physics.Raycast(ParentCubePos, transform.up * 0.6f, out hit, 0.5f))
            {
                EURrender = true;
                EULrender = true;
                if (Urender)
                {
                    EUrender = true;
                }
            }
            if (Physics.Raycast(transform.position + transform.forward * 0.1f, transform.up * 0.6f, out hit, 0.9f))
            {
                if (Urender)
                {
                    IUrender = true;
                }
            }

            if (!Physics.Raycast(ParentCubePos, -transform.up * 0.6f, out hit, 0.5f))
            {
                EDRrender = true;
                EDLrender = true;
                if (Drender)
                {
                    EDrender = true;
                }
            }
            if (Physics.Raycast(transform.position + transform.forward * 0.1f, -transform.up * 0.6f, out hit, 0.9f))
            {
                if (Drender)
                {
                    IDrender = true;
                }
            }

            if (ERUrender && EURrender)
            {
                EERUrender = true;
            }
            if (ELUrender && EULrender)
            {
                EELUrender = true;
            }
            if (ERDrender && EDRrender)
            {
                EERDrender = true;
            }
            if (ELDrender && EDLrender)
            {
                EELDrender = true;
            }
        }

        public void Reset()
        {
            inactive = false;

            RightPath = false;
            LeftPath = false;
            UpPath = false;
            DownPath = false;

            RightNode = null;
            LeftNode = null;
            UpNode = null;
            DownNode = null;


            Rrender = false;
            Lrender = false;
            Urender = false;
            Drender = false;

            RUrender = false;
            RDrender = false;
            LUrender = false;
            LDrender = false;

            ERrender = false;
            ELrender = false;
            EUrender = false;
            EDrender = false;

            IRrender = false;
            ILrender = false;
            IUrender = false;
            IDrender = false;

            ERUrender = false;
            ERDrender = false;
            ELUrender = false;
            ELDrender = false;

            EURrender = false;
            EULrender = false;
            EDRrender = false;
            EDLrender = false;

            EERUrender = false;
            EERDrender = false;
            EELUrender = false;
            EELDrender = false;
        }
    }


    [System.Serializable]
    public class SavableNode
    {
        //inactivity
        public int i;

        //node transfomr rotation
        public int u;
        public int v;
        public int w;

        //node path data
        public int r;
        public int l;
        public int f;
        public int b;

        public void ConvertToSavable(Node node)
        {
            i = node.inactive ? 1 : 0;

            u = (int)(node.transform.eulerAngles.x);
            v = (int)(node.transform.eulerAngles.y);
            w = (int)(node.transform.eulerAngles.z);

            r = node.RightPath ? 1 : 0;
            l = node.LeftPath ? 1 : 0;
            f = node.UpPath ? 1 : 0;
            b = node.DownPath ? 1 : 0;
        }

        public Node GetNode()
        {
            Node node = new Node();

            node.inactive = (i == 1);

            node.NodeTransformRot = new Vector3(u, v, w);

            node.RightPath = (r == 1);
            node.LeftPath = (l == 1);
            node.UpPath = (f == 1);
            node.DownPath = (b == 1);

            return node;
        }
    }
}