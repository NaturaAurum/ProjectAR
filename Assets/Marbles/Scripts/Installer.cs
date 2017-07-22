using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-999)]
public class Installer : MonoBehaviour
{
    private static Installer instance = null;
    private Dictionary<Type, object> typeList;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        typeList = new Dictionary<Type, object>();

        var initObjects = FindObjectsOfType<Initializable>();
        for(int i = 0; i < initObjects.Length; ++i)
        {
            initObjects[ i ].Initalize();
            typeList.Add( initObjects[ i ].Type, initObjects[ i ].Instance );
        }
    }

    private T GetInstanceWithType<T>() where T : Initializable
    {
        return typeList[ typeof( T ) ] as T;
    }

    public static T GetInstance<T>() where T : Initializable
    {
        try
        {
            return instance.GetInstanceWithType<T>();
        }
        catch (NullReferenceException e)
        {
            Debug.Log( e.Data );
            return null;
        }
    }
}
