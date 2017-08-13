using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class Stage : MonoBehaviour
{
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        UnityARSessionNativeInterface.ARAnchorAddedEvent += ARAnchorAdded;
    }

    void ARAnchorAdded(ARPlaneAnchor anchor)
    {
		var planePos = UnityARMatrixOps.GetPosition(anchor.transform);
		Debug.Log("Camera to first plane : " + (Camera.main.transform.position - planePos).magnitude);
		UnityARSessionNativeInterface.ARAnchorAddedEvent -= ARAnchorAdded;	
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    // void Update()
    // {

    // }
}
