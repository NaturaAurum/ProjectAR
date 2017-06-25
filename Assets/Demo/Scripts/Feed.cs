using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feed : MonoBehaviour
{

    private Rigidbody feedRig;

    private void Awake()
    {
        feedRig = GetComponent<Rigidbody>();
        feedRig.isKinematic = true;
    }

    public void Throw(Vector3 velocity)
    {
        feedRig.velocity = velocity/3;
        feedRig.isKinematic = false;
    }
}
