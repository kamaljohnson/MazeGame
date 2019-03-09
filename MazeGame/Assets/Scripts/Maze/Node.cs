using System;
using UnityEngine;

namespace Game.Maze
{
    public class Node : MonoBehaviour
    {
        public Vector3 parentCube_pos;
        public Vector3 nodeTransform_pos;
        public Vector3 nodeTransform_rot;

        public bool inactive;

        //path status
        public bool right_path;
        public bool left_path;
        public bool up_path;
        public bool down_path;

        //neighbour hood node status
        public Node right_node;
        public Node left_node;
        public Node up_node;
        public Node down_node;

        //rendering status
        //sides
        public bool r_render;
        public bool l_render;
        public bool u_render;
        public bool d_render;
        //corners
        public bool ru_render;
        public bool rd_render;
        public bool lu_render;
        public bool ld_render;

        public bool er_render;
        public bool el_render;
        public bool eu_render;
        public bool ed_render;

        public bool ir_render;
        public bool il_render;
        public bool iu_render;
        public bool id_render;

        public bool eru_render;
        public bool erd_render;
        public bool elu_render;
        public bool eld_render;

        public bool eur_render;
        public bool eul_render;
        public bool edr_render;
        public bool edl_render;

        public bool eeru_render;
        public bool eerd_render;
        public bool eelu_render;
        public bool eeld_render;

        public void SetNodeFromNode(Node node, Vector3 parentCube_pos)
        {
            right_path = node.right_path;
            left_path = node.left_path;
            up_path = node.up_path;
            down_path = node.down_path;

            this.parentCube_pos = parentCube_pos;
            nodeTransform_pos = transform.position;
            nodeTransform_rot = transform.eulerAngles;
        }

        public void Start()
        {
            parentCube_pos = -transform.forward;
        }

        public void ReCalculateNeighbourInterations()
        {
            right_path = (right_node != null);
            left_path = (left_node != null);
            up_path = (up_node != null);
            down_path = (down_node != null);
        }

        public void CalculatePathDirection(Node other)
        {

            if (Vector3.Distance(other.transform.position, transform.position + transform.right) < 0.01f ||
                Vector3.Distance(other.transform.position, parentCube_pos + transform.right * 0.5f) < 0.01f ||
                Vector3.Distance(other.transform.position, transform.position + (transform.forward + transform.right) * .5f) < 0.01f)
            {
                if (!other.inactive && !inactive)
                {
                    right_path = !right_path;
                }
                else
                {
                    right_path = true;
                }
                right_node = other;
            }
            if (Vector3.Distance(other.transform.position, transform.position - transform.right) < 0.01f ||
                Vector3.Distance(other.transform.position, parentCube_pos - transform.right * 0.5f) < 0.01f ||
                Vector3.Distance(other.transform.position, transform.position + (transform.forward - transform.right) * .5f) < 0.01f)
            {

                if (!other.inactive && !inactive)
                {
                    left_path = !left_path;
                }
                else
                {
                    left_path = true;
                }
                left_node = other;
            }
            if (Vector3.Distance(other.transform.position, transform.position + transform.up) < 0.01f ||
                Vector3.Distance(other.transform.position, parentCube_pos + transform.up * 0.5f) < 0.01f ||
                Vector3.Distance(other.transform.position, transform.position + (transform.forward + transform.up) * .5f) < 0.01f)
            {
                if (!other.inactive && !inactive)
                {
                    up_path = !up_path;
                }
                else
                {
                    up_path = true;
                }
                up_node = other;
            }
            if (Vector3.Distance(other.transform.position, transform.position - transform.up) < 0.01f ||
                Vector3.Distance(other.transform.position, parentCube_pos - transform.up * 0.5f) < 0.01f ||
                Vector3.Distance(other.transform.position, transform.position + (transform.forward - transform.up) * .5f) < 0.01f)
            {
                if (!other.inactive && !inactive)
                {
                    down_path = !down_path;
                }
                else
                {
                    down_path = true;
                }
                down_node = other;
            }
        }

        public void CalculateRenderNodePath()
        {
            RaycastHit hit;

            r_render = false;
            l_render = false;
            u_render = false;
            d_render = false;

            ru_render = false;
            rd_render = false;
            lu_render = false;
            ld_render = false;

            er_render = false;
            el_render = false;
            eu_render = false;
            ed_render = false;

            ir_render = false;
            il_render = false;
            iu_render = false;
            id_render = false;

            eru_render = false;
            erd_render = false;
            elu_render = false;
            eld_render = false;

            eur_render = false;
            eul_render = false;
            edr_render = false;
            edl_render = false;

            eeru_render = false;
            eerd_render = false;
            eelu_render = false;
            eeld_render = false;


            if (!inactive)
            {
                r_render = !right_path;
                l_render = !left_path;
                u_render = !up_path;
                d_render = !down_path;

                rd_render = true;
                ru_render = true;
                ld_render = true;
                lu_render = true;
            }
            else
            {
                if (!Physics.Raycast(parentCube_pos, transform.right, out hit, 1))
                {
                    if (r_render)
                    {
                        ru_render = true;
                        rd_render = true;
                        er_render = true;
                    }
                }
                if (!Physics.Raycast(parentCube_pos, -transform.right, out hit, 1))
                {
                    if (l_render)
                    {
                        lu_render = true;
                        ld_render = true;
                        el_render = true;
                    }
                }
                if (!Physics.Raycast(parentCube_pos, transform.up, out hit, 1))
                {
                    if (u_render)
                    {
                        ru_render = true;
                        lu_render = true;
                        eu_render = true;
                    }
                }
                if (!Physics.Raycast(parentCube_pos, -transform.up, out hit, 1))
                {
                    if (d_render)
                    {
                        rd_render = true;
                        ld_render = true;
                        ed_render = true;
                    }
                }
            }

            if (!Physics.Raycast(parentCube_pos, transform.right * 0.6f, out hit, 0.5f))
            {
                eru_render = true;
                erd_render = true;
                if (r_render)
                {
                    er_render = true;
                }
            }
            if (Physics.Raycast(transform.position + transform.forward * 0.1f, transform.right * 0.6f, out hit, 0.9f))
            {
                if (r_render)
                {
                    ir_render = true;
                }
            }

            if (!Physics.Raycast(parentCube_pos, -transform.right * 0.6f, out hit, 0.5f))
            {
                elu_render = true;
                eld_render = true;
                if (l_render)
                {
                    el_render = true;
                }
            }
            if (Physics.Raycast(transform.position + transform.forward * 0.1f, -transform.right * 0.6f, out hit, 0.9f))
            {
                if (l_render)
                {
                    il_render = true;
                }
            }

            if (!Physics.Raycast(parentCube_pos, transform.up * 0.6f, out hit, 0.5f))
            {
                eur_render = true;
                eul_render = true;
                if (u_render)
                {
                    eu_render = true;
                }
            }
            if (Physics.Raycast(transform.position + transform.forward * 0.1f, transform.up * 0.6f, out hit, 0.9f))
            {
                if (u_render)
                {
                    iu_render = true;
                }
            }

            if (!Physics.Raycast(parentCube_pos, -transform.up * 0.6f, out hit, 0.5f))
            {
                edr_render = true;
                edl_render = true;
                if (d_render)
                {
                    ed_render = true;
                }
            }
            if (Physics.Raycast(transform.position + transform.forward * 0.1f, -transform.up * 0.6f, out hit, 0.9f))
            {
                if (d_render)
                {
                    id_render = true;
                }
            }

            if (eru_render && eur_render)
            {
                eeru_render = true;
            }
            if (elu_render && eul_render)
            {
                eelu_render = true;
            }
            if (erd_render && edr_render)
            {
                eerd_render = true;
            }
            if (eld_render && edl_render)
            {
                eeld_render = true;
            }
        }

        public void Reset()
        {
            inactive = false;

            right_path = false;
            left_path = false;
            up_path = false;
            down_path = false;

            right_node = null;
            left_node = null;
            up_node = null;
            down_node = null;


            r_render = false;
            l_render = false;
            u_render = false;
            d_render = false;

            ru_render = false;
            rd_render = false;
            lu_render = false;
            ld_render = false;

            er_render = false;
            el_render = false;
            eu_render = false;
            ed_render = false;

            ir_render = false;
            il_render = false;
            iu_render = false;
            id_render = false;

            eru_render = false;
            erd_render = false;
            elu_render = false;
            eld_render = false;

            eur_render = false;
            eul_render = false;
            edr_render = false;
            edl_render = false;

            eeru_render = false;
            eerd_render = false;
            eelu_render = false;
            eeld_render = false;
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

            r = node.right_path ? 1 : 0;
            l = node.left_path ? 1 : 0;
            f = node.up_path ? 1 : 0;
            b = node.down_path ? 1 : 0;
        }

        public Node GetNode()
        {
            Node node = new Node();

            node.inactive = (i == 1);

            node.nodeTransform_rot = new Vector3(u, v, w);

            node.right_path = (r == 1);
            node.left_path = (l == 1);
            node.up_path = (f == 1);
            node.down_path = (b == 1);

            return node;
        }
    }
}