using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

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
        public struct Data{
            public Quaternion quaternionOutput;
            public Vector3 myPos, tPos;

            public Data(Vector3 mp, Vector3 tp){
                quaternionOutput = new Quaternion();
                myPos = mp;
                tPos = tp;
            }

            public void Update(){
                quaternionOutput = Quaternion.LookRotation(tPos - myPos, Vector3.up);
            }
        }

        private void Update(){

        }
    }
}