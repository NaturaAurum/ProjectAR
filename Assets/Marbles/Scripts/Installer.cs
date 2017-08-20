using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        DontDestroyOnLoad(gameObject);

        //SceneManager.sceneLoaded += OnNewSceneLoaded;
        //SceneManager.activeSceneChanged += ActiveSceneChanged;
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        Load();
    }

    private void ActiveSceneChanged(Scene args0, Scene args1)
    {
        Load();
    }

    private void Load()
    {
        var initObjects = FindObjectsOfType<Initializable>();
        for (int i = 0; i < initObjects.Length; ++i)
        {
            if (!typeList.ContainsKey(initObjects[i].Type)) 
            { 
                initObjects[i].Initalize();
                typeList.Add(initObjects[i].Type, initObjects[i].Instance); 
            }
        }
    }

    private void OnNewSceneLoaded(Scene args0, LoadSceneMode args1)
    {
        Load();
    }

    private T GetInstanceWithType<T>() where T : Initializable
    {
        return typeList[typeof(T)] as T;
    }

    public static T GetInstance<T>() where T : Initializable
    {
        try
        {
            return instance.GetInstanceWithType<T>();
        }
        catch (NullReferenceException e)
        {
            Debug.Log(e.Data);
            return null;
        }
    }
}
