using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Rendering;

namespace PT.Garden
{
    public class FlowerManager : MonoBehaviour
    {
        public Transform targetT;

        [SerializeField] private ComputeShader _cs;
        [SerializeField] private float _radius = 0.5f;
        [SerializeField] private List<FlowerJ> _flowers;
        [SerializeField] private bool _hasTarget = false;
        [SerializeField] private float _desMag = 2.5f;
        [SerializeField] private Bees.Bee _mainBee;
        [SerializeField] private string _flowerID;
        [SerializeField] private Color _color;
        [SerializeField] private int _multiplier = 1;

        private FlowerJ.Data[] results;
        private ComputeBuffer _cb;
        private int stride = 0;

        Bees.Honey _honey;

        private void Awake()
        {
            _flowers = new List<FlowerJ>(GetComponentsInChildren<FlowerJ>());
            _honey = new Bees.Honey(_color);
            stride = sizeof(float) * 3 * 3 + sizeof(float) * 3 + sizeof(float) + sizeof(float);
            _cb = new ComputeBuffer(_flowers.Count, stride);
        }

        private void FixedUpdate()
        {
            if (_hasTarget)
            {
                // // set data
                // results = new FlowerJ.Data[_flowers.Count];
                // for (int i = 0; i < _flowers.Count; i++)
                // {
                //     results[i].mp = _flowers[i].transform.position;
                // }
                // float[] pos = { targetT.position.x, targetT.position.y, targetT.position.z };
                // _cs.SetFloats("tp", pos);
                // _cs.SetFloat("desMag", _desMag);
                // _cs.SetBuffer(0, "myData", _cb);
                // _cb.SetData(results);

                // // dispatch
                // int count = _flowers.Count / 4;
                // count = count > 1 ? count : 1;
                // _cs.Dispatch(0, count, count, 1);
                // _cb.GetData(results);
                
                // process results
                for (int i = _flowers.Count - 1; i >= 0; i--)
                {
                    Vector3 dis = _flowers[i].transform.position - targetT.position;
                    if (dis.magnitude < 10)
                    {
                        Vector3 axis = Vector3.Cross(dis.normalized, Vector3.up);
                        _flowers[i].transform.rotation = Quaternion.AngleAxis(
                            (10 - dis.magnitude) / 10 * -30,
                            axis
                        );
                    }

                    if (dis.magnitude < _desMag)
                    {
                        bool d = false;
                        d = _mainBee._chain.CatchHoney(_honey, _multiplier);
                        if(d){
                            _flowers[i].GetDestroyed();
                            _flowers.RemoveAt(i);
                            continue;
                        }
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other){
            Bees.Bee b = other.GetComponent<Bees.Bee>();
            if(b != null){
                if(b.isMain){
                    _hasTarget = true;
                    targetT = b.transform;
                    _mainBee = b;
                }
            }
        }

        private void OnTriggerExit(Collider other){
            Bees.Bee b = other.GetComponent<Bees.Bee>();
            if(b != null){
                if(b.isMain){
                    _hasTarget = false;
                }
            }
        }
    }
}