using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using TMPro;

///AR 과 Game괸련으로.
public class GameManager : Initializable
{
    public override Type Type { get { return GetType(); } }

    public override object Instance { get { return this; } }

    private UnityARSessionNativeInterface arSession = null;

    public int MaxTurn = 10;
    public int TargetScore = 30;

    private int currentTurn = 1;
    private int currentScore = 0;

    public TMP_Text TurnText;
    public TMP_Text ScoreText;

	public Transform SampleGameMap;

    public override void Initalize()
    {
        arSession = UnityARSessionNativeInterface.GetARSessionNativeInterface();
		UnityARSessionNativeInterface.ARAnchorAddedEvent += AR_AnchorAdded;
    }

	void AR_AnchorAdded(ARPlaneAnchor anchor){
		SampleGameMap.position = UnityARMatrixOps.GetPosition(anchor.transform);
		UnityARSessionNativeInterface.ARAnchorAddedEvent -= AR_AnchorAdded;
	}

	void Start(){
		Installer.GetInstance<FeedManager>().CreateFeed();
	}

    public void SetTurn(){
        SetTurn(currentTurn + 1);
    }

    public void SetTurn(int currentTurn)
    {
        this.currentTurn = currentTurn;
        TurnText.text = string.Format("Turn {0} / {1}", currentTurn, MaxTurn);
    }

    public void SetScore(int currentScore)
    {
        this.currentScore = currentScore;
        ScoreText.text = string.Format("Score {0} / {1}", currentScore, TargetScore);
    }

    void Update()
    {
        if (currentScore >= TargetScore || currentTurn >= MaxTurn)
        {
			// Game Over
        }
    }
}
