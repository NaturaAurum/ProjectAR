using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : Initializable
{

	public GameObject BallPrefab;

    public override Type Type { get { return GetType(); } }

    public override object Instance { get { return this; } }

	public override void Initalize(){

	}

	
}
