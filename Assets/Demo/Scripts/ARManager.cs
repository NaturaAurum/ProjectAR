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

    public override void Initalize()
    {
        arNativeInterfaceInst = UnityARSessionNativeInterface.GetARSessionNativeInterface();
        unityARAnchorManager = new UnityARAnchorManager();
		UnityARUtility.InitializePlanePrefab(planePrefab);
    }

	void OnDestory(){
		unityARAnchorManager.Destroy();
	}

    private IEnumerator Start()
    {
		List<ARPlaneAnchorGameObject> arpags = null;
        while (true)
        {
            yield return null;
            arpags = unityARAnchorManager.GetCurrentPlaneAnchors();
            if (arpags.Count >= 1)
            {
                break;
            }
			Debug.Log("Finding");
        }

        // Vector3 hitPos = new Vector3();
        // ARPoint arPoint = new ARPoint
        // {
        //     x = Screen.width / 2,
        //     y = Screen.height / 2
        // };

        // ARHitTestResultType[] resultTypes = {
        //     ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent,
        //     ARHitTestResultType.ARHitTestResultTypeHorizontalPlane,
        //     ARHitTestResultType.ARHitTestResultTypeVerticalPlane,
        //     ARHitTestResultType.ARHitTestResultTypeFeaturePoint,
        // };

        // foreach (var resultType in resultTypes)
        // {
        //     if (HitTestWithResultType(arPoint, resultType, ref hitPos))
        //     {
        //         Installer.GetInstance<ChildManager>().GetHead.position = hitPos;
		// 		Debug.Log("Success");
        //     }
		// 	Debug.Log("Failed");
        // }
		Installer.GetInstance<ChildManager>().GetHead.position = arpags[0].gameObject.transform.position;
        yield break;
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
