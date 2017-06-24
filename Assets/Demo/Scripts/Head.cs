using ProjectAR.Util.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : MonoBehaviour
{
    private Vector3 feedPositon;

    private bool receivedFeed = false;
    private float rotateTime = 0.0f;
    public float RotateEndTime = 0.0f;
    public float MoveSpeed = 5.0f;

    private Quaternion toRotation;
    private Quaternion fromRotation;

    private void Awake()
    {
        EventManager.Listen( EventMessage.Feed, ReceiveFeed );
    }

    private void ReceiveFeed( params object[] args )
    {
        receivedFeed = true;
        feedPositon = ( Vector3 )args[ 0 ];
        toRotation = Quaternion.FromToRotation( transform.forward,
                    ( feedPositon - transform.position ) );
        fromRotation = transform.rotation;
    }

    private void OnDrawGizmos()
    {
        if (receivedFeed)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine( transform.position, transform.position + transform.forward );
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine( transform.position, 
                transform.position + ( feedPositon - transform.position ) );
        }
    }

    private void Update()
    {
        if (receivedFeed)
        {
            if (rotateTime <= 1f)
            {
                rotateTime += Time.deltaTime / RotateEndTime;
                transform.rotation =
                    Quaternion.Lerp( fromRotation, toRotation, rotateTime );
            }
            else
            {
                transform.position = Vector3.MoveTowards( transform.position,
                    feedPositon, MoveSpeed * Time.deltaTime );

                if (( transform.position - feedPositon ).magnitude <= 0.005f)
                {
                    /// 먹이를 먹는 모션을 해야하지만 일단 이렇게.
                    EventManager.Send( EventMessage.Feeded );
                    receivedFeed = false;
                }
            }
        }
    }
}
