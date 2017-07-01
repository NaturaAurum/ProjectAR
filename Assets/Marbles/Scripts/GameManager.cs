using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

///AR 과 Game괸련으로.
public class GameManager : Initializable
{
    public override Type Type { get { return GetType(); } }

    public override object Instance { get { return this; } }
	
	private UnityARSessionNativeInterface arSession = null;

	public int MaxTurn = 10;
	public int TargetScore = 30;

	public override void Initalize()
    {
		arSession = UnityARSessionNativeInterface.GetARSessionNativeInterface();
    }
}
