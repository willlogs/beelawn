using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PT.Bees
{
    [Serializable]
    public class Honey
    {
        private static string glyphs = "abcdefghijklmnopqrstuvwxyz0123456789";

        public static string GenerateRandomString(int charAmount)
        {
            string myString = "";
            for (int i = 0; i < charAmount; i++)
            {
                myString += glyphs[UnityEngine.Random.Range(0, glyphs.Length)];
            }
            return myString;
        }

        public Color color;
        public string ID;
        public float amount = 0;
        public int jarIndex = 0;

        public Honey(Honey h)
        {
            this.ID = h.ID;
            this.color = new Color(h.color.r, h.color.g, h.color.b);
        }

        public Honey(Color c)
        {
            this.ID = GenerateRandomString(5);
            this.color = new Color(c.r, c.g, c.b);
        }
    }

    public class BeeChain : MonoBehaviour
    {
        public bool isActive = false;
        public Vector3 center;
        public Vector3 goal;
        public float radius = 2, speed = 5f, capacity = 100;
        public Utils.MultiColorSlider _multiColorSlider;
        public Dictionary<string, Honey> honeys = new Dictionary<string, Honey>();

        [SerializeField] private Bee[] _bees;
        [SerializeField] private Rigidbody _masterBeeRB;
        [SerializeField] private float _movementThreshold = 0.005f;
        private bool _isActive = true;
        private Vector3 _lastGoal = Vector3.zero, _lastDiff;
        private bool _hasLastGoal, _hasSlider;
        private float _honeyAmount = 0;

        public bool CatchHoney(Honey h, int amount = 1)
        {
            if (_honeyAmount < capacity)
            {
                Honey honeyRef;
                if (honeys.ContainsKey(h.ID))
                {
                    honeys.TryGetValue(h.ID, out honeyRef);
                }
                else
                {
                    honeyRef = new Honey(h);
                    honeys.Add(h.ID, honeyRef);
                    honeyRef.amount = 0;
                }

                honeyRef.amount += amount;
                _honeyAmount += amount;

                UpdateSlider();

                return true;
            }

            return false;
        }

        public Honey[] GetHoneys()
        {
            _honeyAmount = 0;
            Honey[] h = new Honey[honeys.Count];
            honeys.Values.CopyTo(h, 0);

            if (_hasSlider)
                _multiColorSlider.ResetValues();

            honeys = new Dictionary<string, Honey>();
            return h;
        }

        private void UpdateSlider()
        {
            if (_hasSlider)
            {
                int i = 0;
                foreach (Honey h in honeys.Values)
                {
                    _multiColorSlider.SetValue(i, h.amount / capacity, h.color);
                    i++;
                }
            }
        }

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
            _hasSlider = _multiColorSlider != null;
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
            goal = Input.mousePosition;
            if (_hasLastGoal)
            {
                Vector3 diff = goal - _lastGoal;

                if (diff.magnitude < _movementThreshold)
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

                    _masterBeeRB.velocity = Vector3.Lerp(_masterBeeRB.velocity, diff * speed, Time.fixedDeltaTime * 10);
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