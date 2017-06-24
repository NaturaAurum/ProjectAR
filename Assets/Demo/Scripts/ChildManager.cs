using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildManager : Initializable
{
    public override Type Type
    {
        get
        {
            return GetType();
        }
    }

    public override object Instance { get { return this; } }

    public struct HeadPositionData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public float Distance;

        public HeadPositionData(Vector3 pos, Quaternion rot, float dis )
        {
            Position = pos;
            Rotation = rot;
            Distance = dis;
        }
    }

    // 필요없을듯?
    //public struct FollowerData
    //{
    //    public Transform Transform;
    //}
    
    private List<HeadPositionData> headPositions = new List<HeadPositionData>();
    private List<Transform> followers = new List<Transform>();
    private List<Transform> family = new List<Transform>();

    private Transform head;
    private Head headData;
    private Vector3 nextPos;
    private Quaternion nextRot;

    public GameObject ChildPrefab;

    public Transform GetHead
    {
        get { return head; }
    }

    public Transform GetLastFamily
    {
        get { return family[ family.Count - 1 ]; }
    }

    public override void Initalize()
    {
        head = FindObjectOfType<Head>().transform;
        headData = head.GetComponent<Head>();
        family.Add( head );
    }

    public void AddChild( Transform transform )
    {
        followers.Add( transform );
        family.Add( transform );
        Debug.Log( followers.Count );
        Debug.Log( family.Count );
    }

    public void CreateChild()
    {
        var child = Instantiate( ChildPrefab, GetLastFamily.position + ( -GetLastFamily.forward ), Quaternion.identity );
        AddChild( child.transform );
    }

    private void SaveHeadPos()
    {
        if (headPositions.Count == 0)
        {
            headPositions.Insert( 0, new HeadPositionData( head.position, head.rotation, 0f ) );
        }
        else
        {
            Vector3 recentPos = headPositions[ 0 ].Position;
            Vector3 currentPos = head.position;
            if (recentPos != currentPos)
            {
                //Vector3 resizedPos = currentPos.normalized * 5f;
                float distance = ( recentPos - currentPos ).magnitude;
                headPositions.Insert( 0, new HeadPositionData( currentPos, head.rotation, distance ) );
            }
        }
    }

    private void Update()
    {
        SaveHeadPos();
        MoveFollowers();
    }

    private Vector3 moveVelocity;
    private void MoveFollowers()
    {
        int targetIdx = 0;
        float remainDist = 0.5f;

        for (int i = 0; i < followers.Count; ++i)
        {
            float leftDistance = 1.5f - remainDist;
            nextPos = Vector3.zero;
            HeadPositionData tPrev = new HeadPositionData( Vector3.zero, Quaternion.Euler( Vector3.zero ), 0f );
            for (int j = targetIdx; j < headPositions.Count; ++j)
            {
                float distance = tPrev.Distance;
                if (leftDistance >= distance)
                {
                    leftDistance -= distance;
                    tPrev = headPositions[ j ];
                }
                else
                {
                    nextPos = ( ( distance - leftDistance ) * tPrev.Position + leftDistance *
                        headPositions[ j ].Position ) / distance;
                    nextRot = Quaternion.Euler(
                        ( ( distance - leftDistance ) * tPrev.Rotation.eulerAngles +
                        leftDistance * headPositions[ j ].Rotation.eulerAngles ) / distance );
                    remainDist = distance - leftDistance;
                    targetIdx = j;
                    break;
                }
            }

            ////if(nextPos == Vector3.zero)
            ////{
            ////    continue;
            ////}

            if (nextPos != Vector3.zero)
            {
                followers[ i ].position = Vector3.Slerp( followers[ i ].position, nextPos, 0.05f );
                followers[ i ].rotation = Quaternion.Slerp( followers[ i ].rotation, family[ i ].rotation, 0.05f );
            }
            //followers[ i ].transform.rotation = nextRot;
            //if (nextPos != Vector3.zero)
            //{
            //    followers[ i ].position = Vector3.SmoothDamp(
            //        followers[ i ].position, nextPos, ref moveVelocity, 0.2f );
            //}
        }

        while (headPositions.Count > targetIdx + 50)
        {
            headPositions.RemoveAt( headPositions.Count - 1 );
        }
    }
}
