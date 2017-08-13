using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using UnityEngine.UI;

public class Stage : MonoBehaviour
{

    public float HeightMax = 2.0f;
    public Slider uiSlider = null;
    private Vector3 firstPlanePos = Vector3.zero;

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
        var planeRot = UnityARMatrixOps.GetRotation(anchor.transform);
        firstPlanePos = planePos;
        planePos.y += 0.15f;
        transform.position = planePos;
        transform.rotation = planeRot;
        Debug.Log("Camera to first plane : " + (Camera.main.transform.position - planePos).magnitude);
        UnityARSessionNativeInterface.ARAnchorAddedEvent -= ARAnchorAdded;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (uiSlider)
        {
            var stagePos = firstPlanePos;
            stagePos.y += Mathf.Lerp(0, HeightMax, uiSlider.value);
            transform.position = stagePos;
        }
    }
}
