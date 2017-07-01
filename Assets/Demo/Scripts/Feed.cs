using System.Collections;
using System.Collections.Generic;
using ProjectAR.Util.Event;
using UnityEngine;

public class Feed : MonoBehaviour
{

    private Rigidbody feedRig;

    public float GravityScale = -9.8f;

    private void Awake()
    {
        feedRig = GetComponent<Rigidbody>();
        feedRig.isKinematic = true;
    }

    public void Throw(Vector3 velocity)
    {
        var calculatedVelocity = velocity + transform.forward;
        feedRig.velocity = calculatedVelocity / 3;
        feedRig.isKinematic = false;
        transform.SetParent(null);
    }

    void FixedUpdate()
    {
        var garvity = new Vector3(0, GravityScale, 0);
        feedRig.AddForce(garvity * feedRig.mass);
    }

    void OnCollisionEnter(Collision collision)
    {
       
        if (collision.gameObject.tag == "AR_GeneratedPlane")
        {
            Debug.Log("Feed On Plane");
            feedRig.isKinematic = true;
            EventManager.Send(EventMessage.Feed, transform.position);
        }
        // Debug.Log(collision.gameObject.tag);
        // if (collision.gameObject.tag == "Destroy")
        // {
           
        // }
        Destroy(gameObject);
        Installer.GetInstance<FeedManager>().CreateFeed();
    }
}
