using System;
using System.Collections;
using System.Collections.Generic;
using ProjectAR.Util.Event;
using UnityEngine;

public class BallManager : Initializable
{

	public GameObject BallPrefab;

    public override Type Type { get { return GetType(); } }

    public override object Instance { get { return this; } }

	public override void Initalize(){
		//EventManager.Listen(EventMessage.Thorwed,)
	}

	private bool touched = false;

	private Vector3 onTouchedPosition;
	private Vector3 prevTouchPosition;
	private Vector3 currentTouchPosition;

	private Transform createdBall;

	private void Awake(){
		var arTouch = Installer.GetInstance<ARTouch>();
		arTouch.OnTouchDown += OnTouchDown;
		arTouch.OnTouching += OnTouching;
		arTouch.OnTouchUp += OnTouchUp;
	}

	private void OnTouchDown(Vector3 touchPosition){
		touched = true;
	}

	private void OnTouching(Vector3 touchPosition){

	}

	private void OnTouchUp(Vector3 touchPosition){
		// Throw
		touched = false;
	}

	private void Update(){
		if(touched && createdBall != null)
		{
			currentTouchPosition.z = Camera.main.nearClipPlane + 0.03f;
			var result = Camera.main.ScreenToWorldPoint(currentTouchPosition);
			prevTouchPosition = result;
			createdBall.position = result;
		}
	}

	public void CreateBall(){
		if(createdBall != null){
			return;
		}

		var position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, 100, Camera.main.nearClipPlane + 0.03f));
		createdBall = Instantiate(BallPrefab, position, Quaternion.identity).transform;
		createdBall.SetParent(Camera.main.transform);
		createdBall.localRotation = Quaternion.Euler(Vector3.zero);
	}
}
