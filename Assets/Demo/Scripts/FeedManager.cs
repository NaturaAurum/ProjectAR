using ProjectAR.Util.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 먹이를 던진다!
/// 먹이 관련해서 짜둔다.
/// </summary>

public class FeedManager : Initializable
{

    // 하나씩 뽑을 테니 List보단 Queue가 편할까?
    // 성능은 나중에 보자.
    //private List<Vector3> feedPositions = new List<Vector3>();
    private Queue<Vector3> feedPositions = new Queue<Vector3>();
    private Vector3 prevFeedPosition; // 전에 먹었던 먹이 포지션 혹시 모르니 있는게 좋잖아?

    private Vector3 onTouchedPosition;
    private Vector3 currentTouchPosition;

    public GameObject FeedPrefab;

    public Transform CreatedFeed = null;

    public override Type Type
    {
        get
        {
            return GetType();
        }
    }

    public override object Instance
    {
        get { return this; }
    }

    private bool touched = false;

    public override void Initalize()
    {
        EventManager.Listen( EventMessage.Feeded, OnFeedDone );
    }

    private void Awake()
    {
        var artouch = Installer.GetInstance<ARTouch>();
        artouch.OnTouchDown += FeedManager_OnTouchDown;
        artouch.OnTouchUp += FeedManager_OnTouchUp;
        artouch.OnTouching += FeedManager_OnTouching;
    }

    private void FeedManager_OnTouching( Vector3 touchPosition )
    {
        currentTouchPosition = touchPosition;
    }

    private void FeedManager_OnTouchUp( Vector3 touchPosition )
    {
        touched = false;
        onTouchedPosition = Vector3.zero;
    }

    private void FeedManager_OnTouchDown( Vector3 touchPosition )
    {
        touched = true;
        onTouchedPosition = touchPosition;
        EventManager.Send( EventMessage.Feed, new Vector3( 7f, 0.5f, 11f ) );
        //Debug.Log( onTouchedPosition );
    }

    private void Update()
    {
        if (touched)
        {
            var distnace = ( currentTouchPosition - onTouchedPosition ).magnitude;
        }
    }

    public void CreateFeed()
    {
        var position = Camera.main.ScreenToWorldPoint( new Vector3( Screen.width / 2, 0 , 0.5f ) );
        CreatedFeed = Instantiate( FeedPrefab, position, Quaternion.identity ).transform;
    }

    // 먹이는 이쪽에서 던졌다고 메세지를 쏘는게 좋을거 같다.
    // POS 인페르노 슈터처럼 해볼까?
    //private void OnReceiveFeed( params object[] args )
    //{
    //    feedPositions.Enqueue( ( Vector3 )args[ 0 ] );
    //}

    private void OnFeedDone( params object[] args )
    {
        if (feedPositions != null && feedPositions.Count > 0)
        {
            feedPositions.Dequeue();

            if (feedPositions.Count > 0)
            {
                EventManager.Send( EventMessage.Feed, feedPositions.Peek() );
            }
        }
    }
}
