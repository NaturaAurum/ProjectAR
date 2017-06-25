using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feed : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Rigidbody>().isKinematic = true;
    }
}
