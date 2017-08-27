using System;
using System.Collections;
using System.Collections.Generic;
using ProjectAR.Util.Event;
using UnityEngine;

[DefaultExecutionOrder( +1000 )]
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

        Installer.GetInstance<GameManager>( this, GetGameManager );
    }

    private void GetGameManager( GameManager mgr )
    {
        if (mgr != null)
        {
            gameManager = mgr;
        }
    }

    private bool touched = false;

    private Vector3 onTouchedPosition;
    private Vector3 prevTouchPosition;
    private Vector3 currentTouchPosition;

    private Vector3 touchVelocity;

    private Transform createdBall;
    private Ball ball;

    private GameManager gameManager;

    private void Awake()
    {

    }

    private void OnTouchDown( Vector3 touchPosition )
    {
        if (createdBall == null)
        {
            return;
        }
        touchPosition.z = Config.MinimumRenderingDistance;
        var worldTouchPosition = Camera.main.ScreenToWorldPoint( touchPosition );
        Debug.Log( worldTouchPosition );
        var dis = ( worldTouchPosition - createdBall.position ).magnitude;
        Debug.Log( dis );
        if (dis <= createdBall.localScale.x)
        {
            touched = true;
            onTouchedPosition = touchPosition;
            touchPosition.z = Config.MinimumRenderingDistance;
            prevTouchPosition = Camera.main.ScreenToWorldPoint( touchPosition );
        }
    }

    private void OnTouching( Vector3 touchPosition )
    {
        currentTouchPosition = touchPosition;
    }

    private void OnTouchUp( Vector3 touchPosition )
    {
        // Throw
        if (createdBall != null && touched)
        {
            ball.Throw( touchVelocity );
            createdBall = null;
            ball = null;
        }
        touched = false;
    }

    private void Update()
    {
        if (touched && createdBall != null)
        {
            currentTouchPosition.z = Config.MinimumRenderingDistance;
            var result = Camera.main.ScreenToWorldPoint( currentTouchPosition );

            touchVelocity = ( ( result - prevTouchPosition ) ) / Time.deltaTime;
            prevTouchPosition = result;
            createdBall.position = Vector3.MoveTowards( createdBall.position, result, 0.65f * Time.deltaTime );
        }
    }

    public void CreateBall()
    {
        if (createdBall != null || (gameManager != null && !gameManager.CanCreateBall()))
        {
            return;
        }

        var position = Camera.main.ScreenToWorldPoint( new Vector3( Screen.width / 2, 0, Config.MinimumRenderingDistance ) ); // AR Kit 업데이트 하면서 카메라가 바뀐듯 하다?
        createdBall = Instantiate( BallPrefab, position, Quaternion.identity ).transform;
        createdBall.SetParent( Camera.main.transform );
        createdBall.localRotation = Quaternion.Euler( Vector3.zero );
        ball = createdBall.GetComponent<Ball>();
    }
}
