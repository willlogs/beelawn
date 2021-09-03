using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.Events;
using MoreMountains.NiceVibrations;
using Unity.Mathematics;

namespace PT.Garden
{
    public class FlowerJ : MonoBehaviour
    {
        public UnityEvent BeforeDestroy;

        [SerializeField] private float _beforeDestruction;

        public struct Data{
            public float3x3 rot;
            public float3 mp;
            public float shoudlDie;
            public float shoudlRot;
            // public void Update(){
            //     Vector3 look = tPos - myPos;
            //     if(look.magnitude < desMag){
            //         shoudlDie = true;
            //     }

            //     if(look.magnitude < 10){
            //         Vector3 axis = Vector3.Cross(look.normalized, Vector3.up);
            //         float angle = (10 - look.magnitude) / 10;
            //         angle = Mathf.Pow(angle, 2);
            //         quaternionOutput = Quaternion.AngleAxis(angle * 25, axis);
            //     }
            // }
        }

        public void GetDestroyed(){
            MMVibrationManager.Haptic (HapticTypes.SoftImpact);
            BeforeDestroy?.Invoke();
            Destroy(gameObject, _beforeDestruction);
        }
    }
}