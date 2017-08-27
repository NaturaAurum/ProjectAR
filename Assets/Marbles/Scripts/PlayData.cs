using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData{
    public int Team;
	public string Name;
	public int Score;
	public int Turn;

    public static PlayerData[] CreateDataWithCount(int count )
    {
        List<PlayerData> list = new List<PlayerData>();
        for(int i = 0; i < count; ++i)
        {
            list.Add( new PlayerData() );
        }

        return list.ToArray();
    }
}

public class PlayData : Initializable
{
    public override Type Type { get { return GetType(); } }

    public override object Instance { get { return this; } }

	public override int GetOrder{
		get { return 0;}
	}

    public override void Initalize()
    {
        DontDestroyOnLoad(gameObject);
    }

	public List<PlayerData> Players;

	public int PlayerCount = 1;
	public GameMode gameMode;
	public GameType gameType;

	public enum GameMode{
		Single,
		OneVsOne,
		TwoVsTwo
	}

	public enum GameType{
		SINGLECHALLENGE,
		SUM,
		ATTACK,
		DAYLYCHALLENGE,
	}
}
