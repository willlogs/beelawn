using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PT.Bee
{
    public class BeeChain : MonoBehaviour
    {
        public bool isActive = false;
        public Vector3 center;
        public Vector3 goal;
        public float radius = 2, speed = 5f;

        [SerializeField] private Bee[] _bees;
        private bool _isActive = true;
        private Vector3 _lastGoal = Vector3.zero, _lastDiff;
        private bool _hasLastGoal;

        private void Start()
        {
            if (_bees.Length > 0)
            {
                _bees[0].SetChain(this);
            }

            for (int i = 1; i < _bees.Length; i++)
            {
                _bees[i].SetParent(_bees[i - 1], this);
            }
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                goal = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                if (_hasLastGoal)
                {
                    Vector3 diff = goal - _lastGoal;
                    diff = diff.normalized;

                    if (diff.magnitude == 0)
                    {
                        diff = _lastDiff;
                    }

                    isActive = true;

                    if (_bees.Length > 0)
                    {
                        _bees[0].transform.position = Vector3.Lerp(
                            _bees[0].transform.position,
                            diff * speed + _bees[0].transform.position,
                            Time.deltaTime * 5f
                        );
                    }

                    _lastDiff = diff;
                }
                else
                {
                    _hasLastGoal = true;
                }
                _lastGoal = goal;
            }
            else
            {
                isActive = false;
                _hasLastGoal = false;
            }

            if (isActive != _isActive)
            {
                _isActive = isActive;

                if (!_isActive && _bees.Length > 0)
                {
                    center = _bees[0].transform.position;
                }

                Follow(_isActive);
            }
        }

        private void Follow(bool f)
        {
            foreach (Bee b in _bees)
            {
                b.Follow(f);
            }
        }
    }
}