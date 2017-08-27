using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


//[DefaultExecutionOrder(-999)]
public class Installer : MonoBehaviour
{
    private static Installer instance = null;
    private Dictionary<Type, object> typeList;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        typeList = new Dictionary<Type, object>();

        // var initObjects = FindObjectsOfType<Initializable>();
        // for (int i = 0; i < initObjects.Length; ++i)
        // {
        //     initObjects[i].Initalize();
        //     typeList.Add(initObjects[i].Type, initObjects[i].Instance);
        // }
        DontDestroyOnLoad( gameObject );

        SceneManager.sceneLoaded += OnNewSceneLoaded;
        //SceneManager.activeSceneChanged += ActiveSceneChanged;
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    // void OnEnable()
    // {
    //     Load();
    // }

    private void ActiveSceneChanged( Scene args0, Scene args1 )
    {
        Load();
    }

    private void Load()
    {
        var initObjects = FindObjectsOfType<Initializable>();
        List<Initializable> sortedInitObjects = new List<Initializable>( initObjects );
        foreach (var initObject in initObjects)
        {
            if (typeList.ContainsKey( initObject.Type ))
            {
                sortedInitObjects.Remove( initObject );
                continue;
            }
            var order = initObject.GetOrder;
            if (order == -1)
            {
                order = sortedInitObjects.Count - 2;
            }
            sortedInitObjects.Remove( initObject );
            sortedInitObjects.Insert( order, initObject );
        }
        for (int i = 0; i < sortedInitObjects.Count; ++i)
        {
            if (!typeList.ContainsKey( sortedInitObjects[ i ].Type ))
            {
                sortedInitObjects[ i ].Initalize();
                typeList.Add( sortedInitObjects[ i ].Type, sortedInitObjects[ i ].Instance );
            }
        }
    }

    private void OnNewSceneLoaded( Scene args0, LoadSceneMode args1 )
    {
        Load();
    }

    private T GetInstanceWithType<T>() where T : Initializable
    {
        var key = typeof( T );
        if (!typeList.ContainsKey( key ))
        {
            return null;
        }
        return typeList[ key ] as T;
    }

    private IEnumerator GetInstanceWithType<T>( Action<T> callback ) where T : Initializable
    {
        while (!typeList.ContainsKey( typeof( T ) ))
        {
            yield return null;
        }
        callback( typeList[ typeof( T ) ] as T );
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

    public static void GetInstance<T>( MonoBehaviour owner, Action<T> callback ) where T : Initializable
    {
        owner.StartCoroutine( instance.GetInstanceWithType<T>( callback ) );
    }
}
