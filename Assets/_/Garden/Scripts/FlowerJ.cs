using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.Events;
using MoreMountains.NiceVibrations;

namespace PT.Garden
{
    public struct FlowerUpdateJob : IJobParallelFor
    {
        public NativeArray<FlowerJ.Data> flowerDataArray;
        
        public void Execute(int index)
        {
            FlowerJ.Data data = flowerDataArray[index];
            data.Update();
            flowerDataArray[index] = data;
        }
    }    

    public class FlowerJ : MonoBehaviour
    {
        public UnityEvent BeforeDestroy;

        [SerializeField] private float _beforeDestruction;

        public struct Data{
            public Quaternion quaternionOutput;
            public Vector3 myPos, tPos;
            public bool shoudlDie;
            public float desMag;

            public Data(Vector3 mp, Vector3 tp, float dm){
                quaternionOutput = new Quaternion();
                myPos = mp;
                tPos = tp;
                shoudlDie = false;
                desMag = dm;
            }

            public void Update(){
                Vector3 look = tPos - myPos;
                if(look.magnitude < desMag){
                    shoudlDie = true;
                }
                
                if(look.magnitude < 10){
                    Vector3 axis = Vector3.Cross(look.normalized, Vector3.up);
                    float angle = (10 - look.magnitude) / 10;
                    angle = Mathf.Pow(angle, 2);
                    quaternionOutput = Quaternion.AngleAxis(angle * 25, axis);
                }
            }
        }

        public void GetDestroyed(){
            MMVibrationManager.Haptic (HapticTypes.SoftImpact);
            BeforeDestroy?.Invoke();
            Destroy(gameObject, _beforeDestruction);
        }
    }
}