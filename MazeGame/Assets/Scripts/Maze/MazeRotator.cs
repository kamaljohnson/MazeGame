using System.Collections;
using UnityEngine;

namespace Game.Maze
{
    public class MazeRotator : MonoBehaviour
    {
        public bool IsRotating = false;
        private int angle = 0;
        private float speed;
        private Vector3 direction;
        private Vector3 prevRotation;
        private int step;

        public void Update()
        {
            if (IsRotating)
            {
                StartCoroutine(nameof(rotate));
            }
        }

        public IEnumerator rotate()
        {
            transform.RotateAround(transform.position, direction, step);
            angle += step;
            if (angle == 90)
            {
                IsRotating = false;
                angle = 0;
            }
            yield return new WaitForSeconds(90 / step);
        }

        public void Rotate(Vector3 direction, float speed)
        {
            this.direction = direction;
            step = 5;
            prevRotation = transform.eulerAngles;
            IsRotating = true;
        }
    }
}