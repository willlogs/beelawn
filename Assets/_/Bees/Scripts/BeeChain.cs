using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PT.Bees
{
    public class BeeChain : MonoBehaviour
    {
        public bool isActive = false;
        public Vector3 center;
        public Vector3 goal;
        public float radius = 2, speed = 5f;

        [SerializeField] private Bee[] _bees;
        [SerializeField] private Rigidbody _masterBeeRB;
        private bool _isActive = true;
        private Vector3 _lastGoal = Vector3.zero, _lastDiff;
        private bool _hasLastGoal;

        private void Awake()
        {
            if (_bees.Length > 0)
            {
                _bees[0].SetChain(this);
            }

            for (int i = 1; i < _bees.Length; i++)
            {
                _bees[i].SetParent(_bees[i - 1], this);
            }

            center = _bees[0].transform.position;
        }

        private void Start()
        {
            Follow(_isActive);
        }

        private void FixedUpdate()
        {
            if (Input.GetMouseButton(0))
            {
                OnInput();
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

        private void OnInput()
        {
            goal = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            if (_hasLastGoal)
            {
                Vector3 diff = goal - _lastGoal;

                if (diff.magnitude == 0)
                {
                    diff = _lastDiff;
                }
                else
                {
                    diff.z = diff.y;
                    diff.y = 0;
                }

                diff = diff.normalized;
                isActive = true;

                if (_bees.Length > 0)
                {
                    _bees[0].transform.forward = Vector3.Lerp(
                        _bees[0].transform.forward,
                        diff.normalized,
                        Time.fixedDeltaTime * 10
                    );

                    // _bees[0].transform.position = Vector3.Lerp(
                    //     _bees[0].transform.position,
                    //     diff * speed + _bees[0].transform.position,
                    //     Time.deltaTime * 5f
                    // );
                    _masterBeeRB.velocity = diff * speed;
                }

                _lastDiff = diff;
            }
            else
            {
                _hasLastGoal = true;
            }
            _lastGoal = goal;
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