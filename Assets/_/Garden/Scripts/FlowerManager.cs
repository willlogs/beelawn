using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

namespace PT.Garden
{
    public class FlowerManager : MonoBehaviour
    {
        public Transform targetT;

        [SerializeField] private float _radius = 0.5f;
        [SerializeField] private List<FlowerJ> _flowers;
        [SerializeField] private bool _hasTarget = false;
        [SerializeField] private float _desMag = 2.5f;
        [SerializeField] private Bees.Bee _mainBee;
        [SerializeField] private string _flowerID;
        [SerializeField] private Color _color;
        [SerializeField] private int _multiplier = 1;

        Bees.Honey _honey;

        private void Awake()
        {
            _flowers = new List<FlowerJ>(GetComponentsInChildren<FlowerJ>());
            _honey = new Bees.Honey(_color);
        }

        private void Update()
        {
            if (_hasTarget)
            {
                NativeArray<FlowerJ.Data> flowerDataArray = new NativeArray<FlowerJ.Data>(_flowers.Count, Allocator.TempJob);

                for (int i = 0; i < _flowers.Count; i++)
                {
                    flowerDataArray[i] = new FlowerJ.Data(_flowers[i].transform.position, targetT.position, _desMag);
                }

                FlowerUpdateJob job = new FlowerUpdateJob
                {
                    flowerDataArray = flowerDataArray
                };

                JobHandle jobHandle = job.Schedule(_flowers.Count, 1);
                jobHandle.Complete();

                for (int i = _flowers.Count - 1; i >= 0; i--)
                {
                    _flowers[i].transform.rotation = flowerDataArray[i].quaternionOutput;
                    if (flowerDataArray[i].shoudlDie)
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

                flowerDataArray.Dispose();
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