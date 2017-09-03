using System;
using System.Collections;
using System.Collections.Generic;
using ProjectAR.Util.Event;
using UnityEngine;
using UnityEngine.XR.iOS;

[DefaultExecutionOrder(+1000)]
public class BallManager : Initializable
{

    public GameObject BallPrefab;

    public override Type Type { get { return GetType(); } }

    public override object Instance { get { return this; } }

    public override int GetOrder
    {
        get { return 1; }
    }

    public override void Initalize()
    {
        //EventManager.Listen(EventMessage.Thorwed,)
        var arTouch = Installer.GetInstance<ARTouch>();
        arTouch.OnTouchDown += OnTouchDown;
        arTouch.OnTouching += OnTouching;
        arTouch.OnTouchUp += OnTouchUp;

        Installer.GetInstance<GameManager>(this, GetGameManager);
    }

    private void GetGameManager(GameManager mgr)
    {
        if (mgr != null)
        {
            gameManager = mgr;
        }
    }

    private bool touched = false;

    private Vector3 onTouchedPosition;
    private Vector3 onTouchUpPosition;
    private Vector3 prevTouchPosition;
    private Vector3 currentTouchPosition;

    private Vector3 touchVelocity;

    private Transform createdBall;
    private Ball ball;

    private GameManager gameManager;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdate;
        //UnityARSessionNativeInterface.ARAnchorAddedEvent += ARAnchorAdded;
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        UnityARSessionNativeInterface.ARFrameUpdatedEvent -= ARFrameUpdate;
        //UnityARSessionNativeInterface.ARAnchorAddedEvent -= ARAnchorAdded;
    }

    private Vector3 _ballPosition;
    private void ARFrameUpdate(UnityARCamera arCamera)
    {
        if (createdBall != null)
        {
            var arMatrix = arCamera.worldTransform;
            Matrix4x4 matrix = new Matrix4x4(arMatrix.column0, arMatrix.column1, arMatrix.column2, arMatrix.column3);
            var unityPosition = UnityARMatrixOps.GetPosition(matrix);
            var unityRotation = UnityARMatrixOps.GetRotation(matrix);
            var cameraForward = unityRotation * Vector3.forward;
            var ballPosition = unityPosition + (cameraForward.normalized * Config.MinimumRenderingDistance);
            _ballPosition = ballPosition;
            if (touched)
            {
                var touchWorldPoint = Camera.main.ScreenToWorldPoint(currentTouchPosition);
                var result = touchWorldPoint + (cameraForward.normalized * Config.MinimumRenderingDistance);
                touchVelocity = ((result - prevTouchPosition)) / Time.deltaTime;
                prevTouchPosition = result;
                createdBall.position = Vector3.MoveTowards(createdBall.position, result, 0.65f * Time.deltaTime);
            }
            else{
                createdBall.position = ballPosition + onTouchUpPosition;
            }
            //createdBall.position = ballPosition;
        }
    }

    private void ARAnchorAdded(ARPlaneAnchor achor)
    {
        if (createdBall != null)
        {
            var logFormat = string.Format("Camera Position : {0}, Current Ball Position : {1}, calculated ball position {2}", Camera.main.transform.position, createdBall.position, _ballPosition);
            Debug.Log(logFormat);
        }
    }

    private void OnTouchDown(Vector3 touchPosition)
    {
        if (createdBall == null)
        {
            return;
        }
        touchPosition.z = Config.MinimumRenderingDistance;
        var worldTouchPosition = Camera.main.ScreenToWorldPoint(touchPosition);
        var dis = (worldTouchPosition - createdBall.position).magnitude;
        if (dis <= createdBall.localScale.x)
        {
            touched = true;
            onTouchedPosition = touchPosition;
            touchPosition.z = Config.MinimumRenderingDistance;
            prevTouchPosition = Camera.main.ScreenToWorldPoint(touchPosition);
        }
    }

    private void OnTouching(Vector3 touchPosition)
    {
        currentTouchPosition = touchPosition;
    }

    private void OnTouchUp(Vector3 touchPosition)
    {
        // Throw
        if (createdBall != null && touched)
        {
            ball.Throw(touchVelocity);
            createdBall = null;
            ball = null;
        }
        onTouchUpPosition = touchPosition;
        touched = false;
    }
#if UNTY_EDITOR
    private void Update()
    {
        if (touched && createdBall != null)
        {
            currentTouchPosition.z = Config.MinimumRenderingDistance;
            var result = Camera.main.ScreenToWorldPoint(currentTouchPosition);

            touchVelocity = ((result - prevTouchPosition)) / Time.deltaTime;
            prevTouchPosition = result;
            createdBall.position = Vector3.MoveTowards(createdBall.position, result, 0.65f * Time.deltaTime);
        }
    }
#endif

    public void CreateBall()
    {
        if (createdBall != null || (gameManager != null && !gameManager.CanCreateBall()))
        {
            return;
        }

        var position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, 0, Config.MinimumRenderingDistance)); // AR Kit 업데이트 하면서 카메라가 바뀐듯 하다?
        createdBall = Instantiate(BallPrefab, position, Quaternion.identity).transform;
        createdBall.position = _ballPosition;
        ball = createdBall.GetComponent<Ball>();
    }
}
