using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayData : Initializable
{
    public override Type Type { get { return GetType(); } }

    public override object Instance { get { return this; } }

    public override void Initalize()
    {
        DontDestroyOnLoad(gameObject);
    }

	public int PlayerCount = 1;
	public GameMode gameMode;

	public enum GameMode{
		Single,
		OneVsOne,
		TwoVsTwo
	}
}
