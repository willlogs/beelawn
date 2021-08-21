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

        private void Awake()
        {
            
        }

        private void Update()
        {
            NativeArray<FlowerJ.Data> flowerDataArray = new NativeArray<FlowerJ.Data>(_flowers.Count, Allocator.TempJob);

            for (int i = 0; i < _flowers.Count; i++)
            {
                flowerDataArray[i] = new FlowerJ.Data(_flowers[i].transform.position, targetT.position);
            }

            FlowerUpdateJob job = new FlowerUpdateJob
            {
                flowerDataArray = flowerDataArray
            };

            JobHandle jobHandle = job.Schedule(_flowers.Count, 1);
            jobHandle.Complete();

            for (int i = 0; i < _flowers.Count; i++)
            {
                _flowers[i].transform.rotation = flowerDataArray[i].quaternionOutput;
            }

            flowerDataArray.Dispose();
        }
    }
}