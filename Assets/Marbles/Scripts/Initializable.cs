using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Initializable : MonoBehaviour
{
    public virtual void Initalize() { }
    public abstract Type Type { get; }
    public abstract object Instance { get; }

    public virtual int GetOrder
    {
        get{
            return -1;
        }
    }
}
