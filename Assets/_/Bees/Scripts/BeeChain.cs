using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        public float radius = 2, speed = 5f, capacity = 100, sensivity = 5, mgoal = 5;
        public Utils.MultiColorSlider _multiColorSlider;
        public Dictionary<string, Honey> honeys = new Dictionary<string, Honey>();

        [SerializeField] private Bee[] _bees;
        [SerializeField] private Rigidbody _masterBeeRB;
        [SerializeField] private float _movementThreshold = 0.005f;
        [SerializeField] private Image _jBase, _jStick;
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

            _jBase.enabled = false;
            _jStick.enabled = false;
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

                _jBase.enabled = false;
                _jStick.enabled = false;
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
            if (_hasLastGoal)
            {
                Vector3 newG = Input.mousePosition;
                Vector3 diff = newG - goal;                

                Vector3 wdiff = diff.normalized;
                wdiff.z = wdiff.y;
                wdiff.y = 0;

                if (_bees.Length > 0)
                {
                    _masterBeeRB.velocity = Vector3.Lerp(_masterBeeRB.velocity, wdiff * speed, Time.fixedDeltaTime * mgoal);
                    _bees[0].transform.forward = Vector3.Lerp(
                        _bees[0].transform.forward,
                        _masterBeeRB.velocity.normalized,
                        Time.fixedDeltaTime * 10
                    );
                }
                isActive = true;

                if(diff.magnitude > _movementThreshold){
                    diff = diff.normalized * _movementThreshold;
                }
                _jStick.transform.position = Vector3.Lerp(
                    _jStick.transform.position,
                    goal + diff,
                    Time.fixedDeltaTime * 10
                );
            }
            else
            {
                goal = Input.mousePosition;

                _hasLastGoal = true;
                _jBase.enabled = true;
                _jStick.enabled = true;

                _jBase.transform.position = goal;
                _jStick.transform.position = goal;
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