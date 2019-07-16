using System;
using UnityEngine;

namespace Game.Items.Enemies.Knight
{
    public class Knight : MonoBehaviour
    {
    
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

        public int i;    //interactable id

        public void ConvertToSerializable(Knight knight)
        {
            var transform = knight.transform;
            var position = transform.position;
            x = (int)position.x;
            y = (int)position.y;
            z = (int)position.z;

            var eulerAngles = transform.eulerAngles;
            u = (int) eulerAngles.x;
            v = (int) eulerAngles.y;
            w = (int) eulerAngles.z;
        }
    }
}
