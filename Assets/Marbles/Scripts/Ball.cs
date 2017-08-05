using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    private Rigidbody ballRig;

    public float GravityScale = -0.05f;

    private int bounceCount = 0;
    private bool bounced = false;

    private void Awake()
    {
        ballRig = GetComponent<Rigidbody>();
        ballRig.isKinematic = true;
    }


    public void Throw( Vector3 velocity )
    {
        var calculatedVelocity = velocity + transform.forward;
        ballRig.velocity = calculatedVelocity * 0.45f;
        ballRig.isKinematic = false;
        transform.SetParent( null );
    }

    // private void FixedUpdate()
    // {
    //     var gravity = new Vector3( 0, GravityScale, 0 );
    //     ballRig.AddForce( gravity * ballRig.mass );
    // }

    private void Update()
    {
        var gravity = new Vector3( 0, GravityScale, 0 );
        ballRig.AddForce( gravity * ballRig.mass );

        // 멈춘걸로 본다.
        if (ballRig.velocity.magnitude <= 0.005f)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast( transform.position, Vector3.down, out hitInfo ))
            {
                if (hitInfo.collider.name.Contains( "GameBoard" ))
                {
                    var board = hitInfo.transform.GetComponent<GameBoard>();
                    Installer.GetInstance<GameManager>().SetScore( board.Score );

                    Installer.GetInstance<BallManager>().CreateBall();
                    Installer.GetInstance<GameManager>().SetTurn();
                    Destroy( gameObject );
                }
            }
        }

        if (ballRig.velocity.magnitude <= 0.07f)
        {
            ballRig.AddForce( ballRig.mass * -ballRig.velocity );
        }

        //if (ballRig.velocity.magnitude >= 0.3f)
        //{
        //    RaycastHit boardHitInfo;
        //    if (Physics.Raycast( transform.position, Vector3.down, out boardHitInfo, transform.localScale.x ))
        //    {
        //        //bounceCount++;

        //        var reflect = Vector3.Reflect( ballRig.velocity, boardHitInfo.normal );
        //        ballRig.velocity = reflect.normalized * ( ballRig.velocity.magnitude );
        //        GravityScale *= 1.5f;
        //    }
        //}

        //Debug.Log( ballRig.velocity.magnitude );
        Debug.Log( ballRig.velocity + " magnitude : " + ballRig.velocity.magnitude );
    }

    //private void OnDrawGizmos()
    //{
    //    if (ballRig != null)
    //    {
    //        Gizmos.color = Color.blue * Color.cyan;
    //        Gizmos.DrawLine( transform.position, transform.position + ballRig.velocity.normalized * 0.1f );
    //    }
    //}

    private void OnCollisionEnter( Collision collision )
    {
        if (collision.gameObject.name.Contains( "GameBoard" ) && !bounced
            && ballRig.velocity.magnitude >= 0.3f )
        {
            bounced = true;
            bounceCount++;
            var reflect = Vector3.Reflect( ballRig.velocity, Vector3.up );
            //var power = ballRig.mass * ( (bounceCount + 1) / 10f );
            //Debug.Log( power );
            ballRig.velocity = reflect.normalized * ballRig.velocity.magnitude * 0.4f;
            //GravityScale *= 1.5f;
            //Debug.DrawRay( collision.contacts[ 0 ].point, collision.contacts[ 0 ].normal, Color.red, 0.1f );
        }
        if (collision.gameObject.name.Contains( "Destroy" ))
        {
            Installer.GetInstance<BallManager>().CreateBall();
            Installer.GetInstance<GameManager>().SetTurn();
            Destroy( gameObject );
        }
    }

    private void OnCollisionExit( Collision collision )
    {
        if (collision.gameObject.name.Contains( "GameBoard" ) && bounced)
        {
            bounced = false;
        }
    }
}
