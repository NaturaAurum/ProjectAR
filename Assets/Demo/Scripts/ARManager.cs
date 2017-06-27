using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class ARManager : Initializable
{
    public override Type Type { get { return GetType(); } }

    public override object Instance { get { return this; } }


    private UnityARSessionNativeInterface arNativeInterfaceInst;
    private UnityARAnchorManager unityARAnchorManager;

    public GameObject planePrefab;

    public Transform TestObject;


    private bool initalized = false;
    public override void Initalize()
    {
        arNativeInterfaceInst = UnityARSessionNativeInterface.GetARSessionNativeInterface();
        //unityARAnchorManager = new UnityARAnchorManager();
        UnityARUtility.InitializePlanePrefab(planePrefab);

        UnityARSessionNativeInterface.ARAnchorAddedEvent += AnchorAdded;
        UnityARSessionNativeInterface.ARAnchorRemovedEvent += AnchorRemoved;
        UnityARSessionNativeInterface.ARFrameUpdatedEvent += FrameUpdate;
    }

    // void OnEnabled()
    // {
    //     UnityARSessionNativeInterface.ARAnchorAddedEvent += AnchorAdded;
    //     UnityARSessionNativeInterface.ARAnchorRemovedEvent += AnchorRemoved;
    //     UnityARSessionNativeInterface.ARFrameUpdatedEvent += FrameUpdate;
    // }

    // void OnDisabled()
    // {
    //     UnityARSessionNativeInterface.ARAnchorAddedEvent -= AnchorAdded;
    //     UnityARSessionNativeInterface.ARAnchorRemovedEvent -= AnchorRemoved;
    //     UnityARSessionNativeInterface.ARFrameUpdatedEvent -= FrameUpdate;
    // }

    private void AnchorAdded(ARPlaneAnchor anchorData)
    {
        if (!initalized)
        {
            Installer.GetInstance<ChildManager>().GetHead.position = UnityARMatrixOps.GetPosition(anchorData.transform);
            initalized = true;
        }

        TestObject.position = anchorData.extent;
    }

    private void AnchorRemoved(ARPlaneAnchor anchorData)
    {

    }

    private void FrameUpdate(UnityARCamera arCamera)
    {
        
    }

    void OnDestory()
    {
        //unityARAnchorManager.Destroy();
    }

    // private IEnumerator Start()
    // {
    // 	List<ARPlaneAnchorGameObject> arpags = null;
    //     while (true)
    //     {
    //         yield return null;
    //         arpags = unityARAnchorManager.GetCurrentPlaneAnchors();
    //         if (arpags.Count >= 1)
    //         {
    //             break;
    //         }
    // 		Debug.Log("Finding");
    //     }

    //     // Vector3 hitPos = new Vector3();
    //     // ARPoint arPoint = new ARPoint
    //     // {
    //     //     x = Screen.width / 2,
    //     //     y = Screen.height / 2
    //     // };

    //     // ARHitTestResultType[] resultTypes = {
    //     //     ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent,
    //     //     ARHitTestResultType.ARHitTestResultTypeHorizontalPlane,
    //     //     ARHitTestResultType.ARHitTestResultTypeVerticalPlane,
    //     //     ARHitTestResultType.ARHitTestResultTypeFeaturePoint,
    //     // };

    //     // foreach (var resultType in resultTypes)
    //     // {
    //     //     if (HitTestWithResultType(arPoint, resultType, ref hitPos))
    //     //     {
    //     //         Installer.GetInstance<ChildManager>().GetHead.position = hitPos;
    // 	// 		Debug.Log("Success");
    //     //     }
    // 	// 	Debug.Log("Failed");
    //     // }
    // 	Installer.GetInstance<ChildManager>().GetHead.position = UnityARMatrixOps.GetPosition(arpags[0].planeAnchor.transform);
    //     yield break;
    // }

    void Start()
    {
        //var childManager = Installer.GetInstance<ChildManager>();
        // var camPos = arNativeInterfaceInst.GetCameraPose();
        // //hildManager.GetHead.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 10f));
        // var realPos = UnityARMatrixOps.GetPosition(camPos);
        // var realRot = UnityARMatrixOps.GetRotation(camPos);

        //childManager.GetHead.position = realPos + ((realRot * Vector3.forward).normalized * 5f);
    }

    void Update()
    {
        //var arpags = unityARAnchorManager.GetCurrentPlaneAnchors();
        // var camPos = arNativeInterfaceInst.GetCameraPose();
        // var realPos = UnityARMatrixOps.GetPosition(camPos);
        // var realRot = UnityARMatrixOps.GetRotation(camPos);

        // TestObject.position = realPos + ((realRot * Vector3.forward).normalized * 5f);
        // TestObject.LookAt(realPos);
    }

    private bool HitTestWithResultType(ARPoint point, ARHitTestResultType resultTypes, ref Vector3 hitPosition)
    {
        List<ARHitTestResult> hitResults = arNativeInterfaceInst.HitTest(point, resultTypes);
        if (hitResults.Count > 0)
        {
            foreach (var result in hitResults)
            {
                hitPosition = UnityARMatrixOps.GetPosition(result.worldTransform);
                return true;
            }
        }
        return false;
    }
}
