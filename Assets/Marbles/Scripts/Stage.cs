using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using UnityEngine.UI;
using System.Linq;
using ProjectAR.Assets.Marbles.Scripts;

public class Stage : IScalable
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
        //for(int i = 0; i <transform.childCount; ++i){
        //    transform.GetChild(i).gameObject.SetActive(false);
        //}
        firstPlanePos = new Vector3(100f, 100f, 100f);
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

    public float WaitTimePerBoard = 0.05f;

    public IEnumerator StageLoadAnimation()
    {
        System.Random random = new System.Random((int)System.DateTime.Now.Ticks);
        int[] indexList = Enumerable.Range(0, transform.childCount).OrderBy(o => random.Next()).ToArray();
        for(int i = 0; i < indexList.Length; ++i){
            StartCoroutine(BlockLoadAnimation(transform.GetChild(i)));
            yield return new WaitForSeconds(WaitTimePerBoard);
        }
    }

    public AnimationCurve BoardAnimation;
    public float BoardAnimationEndTime = 0f;

    public IEnumerator BlockLoadAnimation(Transform block){
        var t = 0f;
        var startScale = new Vector3(0.2f, 0.0f, 0.2f);
        var targetScale = new Vector3(0.2f, 0.4f, 0.2f);
        while(t < 1f){
            yield return 0f;
            t = Mathf.Lerp(0, 1f, t + Time.deltaTime / BoardAnimationEndTime);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
        }
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

    public override void ApplyWolrdScale()
    {
        transform.localScale *= Config.WorldScale;
    }
}
