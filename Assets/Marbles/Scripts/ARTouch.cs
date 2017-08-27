using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ARTouch : Initializable
{
    public override Type Type
    {
        get { return GetType(); }
    }

    public override object Instance
    {
        get { return this; }
    }

    public override int GetOrder
    {
        get { return 0; }
    }

    public delegate void TouchEvent(Vector3 touchPosition);

    public event TouchEvent OnTouchDown;
    public event TouchEvent OnTouching;
    public event TouchEvent OnTouchUp;

    public Vector3 GetPosition(Vector3 touchPosition)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return GetWorldPositionOnPlane(touchPosition, Camera.main.nearClipPlane + 0.05f);
#else
        var pos = new Vector3(touchPosition.x, touchPosition.y, Camera.main.nearClipPlane + 0.05f);
        return pos;
#endif
    }

    private Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane plane = new Plane(Camera.main.transform.forward, Camera.main.transform.forward.normalized * z);
        float distance;
        plane.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject)
        {
            return;
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        // Mouse Input
        if (Input.GetMouseButtonDown(0))
        {
            if (OnTouchDown != null)
            {
                OnTouchDown(Input.mousePosition);
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (OnTouching != null)
            {
                OnTouching(Input.mousePosition);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (OnTouchUp != null)
            {
                OnTouchUp(Input.mousePosition);
            }
        }
#else
        // Touch
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch( 0 );
            if (touch.phase == TouchPhase.Began)
            {
                if (OnTouchDown != null)
                {
                    OnTouchDown( touch.position );
                }
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (OnTouching != null)
                {
                    OnTouching( touch.position );
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (OnTouchUp != null)
                {
                    OnTouchUp( touch.position );
                }
            }
        }
#endif
    }
}
