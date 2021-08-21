using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest : MonoBehaviour
{
    public Transform target;

    private void Update(){
        transform.rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
    }
}
