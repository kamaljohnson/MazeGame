using System.Collections;
using UnityEngine;

namespace Game.Maze
{
    public class MazeRotator : MonoBehaviour
    {
        public bool isRotating = false;
        private int _angle = 0;
        private float _speed;
        private Vector3 _direction;
        private Vector3 _prevRotation;
        private int _step;

        public void Update()
        {
            if (isRotating)
            {
                StartCoroutine(nameof(Rotate));
            }
        }

        public IEnumerator Rotate()
        {
            transform.RotateAround(transform.position, _direction, _step);
            _angle += _step;
            if (_angle == 90)
            {
                isRotating = false;
                _angle = 0;
            }
            yield return new WaitForSeconds(90 / _step);
        }

        public void Rotate(Vector3 direction, float speed)
        {
            _direction = direction;
            _step = 5;
            _prevRotation = transform.eulerAngles;
            isRotating = true;
        }
    }
}