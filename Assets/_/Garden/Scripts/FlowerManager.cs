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

        private void Awake()
        {
            _flowers = new List<FlowerJ>(GetComponentsInChildren<FlowerJ>());
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
                    if (flowerDataArray[i].shoudlDie)
                    {
                        _flowers[i].GetDestroyed();
                        _flowers.RemoveAt(i);
                        continue;
                    }
                    _flowers[i].transform.rotation = flowerDataArray[i].quaternionOutput;
                }

                flowerDataArray.Dispose();
            }
        }

        private void OnTriggerEnter(Collider other){
            Bees.Bee b = other.GetComponent<Bees.Bee>();
            if(b != null){
                if(b.isMain){
                    print("found");
                    _hasTarget = true;
                    targetT = b.transform;
                }
            }
        }

        private void OnTriggerExit(Collider other){
            Bees.Bee b = other.GetComponent<Bees.Bee>();
            if(b != null){
                if(b.isMain){
                    print("exit");
                    _hasTarget = false;
                }
            }
        }
    }
}