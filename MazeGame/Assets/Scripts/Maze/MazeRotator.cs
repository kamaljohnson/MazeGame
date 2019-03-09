using System.Collections;
using UnityEngine;

namespace Game.Maze
{
    public class MazeRotator : MonoBehaviour
    {
        public bool IsRotating;
        private int angle;
        private float speed;
        private Vector3 direction;
        private Vector3 prevRotation;
        private int step;

        public void Start()
        {
            IsRotating = false;
            angle = 0;
        }

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
            yield return new WaitForSeconds(seconds: 90 / step);
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