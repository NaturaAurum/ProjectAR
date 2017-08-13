using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using TMPro;
using ProjectAR.Util.Event;

///AR 과 Game괸련으로.
public class GameManager : Initializable
{
    public override Type Type { get { return GetType(); } }

    public override object Instance { get { return this; } }

    private UnityARSessionNativeInterface arSession = null;

    public enum State
    {
        Play,
        GameOver,
    }

    private State currentGameState;
    private State prevGameState;

    public void ChangeGameState( State s )
    {
        currentGameState = s;
        prevGameState = s;
        EventManager.Send( EventMessage.ChangeGameState, currentGameState );
    }

    public bool IsState( State s )
    {
        return currentGameState.Equals( s );
    }

    public int MaxTurn = 10;
    public int TargetScore = 30;

    private int currentTurn = 1;
    private int currentScore = 0;
    public int CurrentScore
    {
        get { return currentScore; }
        set { currentScore = value; }
    }

    public TMP_Text TurnText;
    public TMP_Text ScoreText;

    public Transform SampleGameMap;

    private BallManager ballManager;

    public override void Initalize()
    {
        arSession = UnityARSessionNativeInterface.GetARSessionNativeInterface();
        //UnityARSessionNativeInterface.ARAnchorAddedEvent += AR_AnchorAdded;

        //ChangeGameState( State.Play );
    }

    void AR_AnchorAdded( ARPlaneAnchor anchor )
    {
        SampleGameMap.position = UnityARMatrixOps.GetPosition( anchor.transform );
        UnityARSessionNativeInterface.ARAnchorAddedEvent -= AR_AnchorAdded;
    }

    private void Awake()
    {
        ballManager = Installer.GetInstance<BallManager>();
    }

    void Start()
    {
        //Installer.GetInstance<FeedManager>().CreateFeed();
        Installer.GetInstance<BallManager>().CreateBall();
        ChangeGameState( State.Play );
    }

    public void SetTurn()
    {
        if (IsState( State.Play ))
        {
            SetTurn( currentTurn + 1 );
        }
    }

    public void ResetGame()
    {
        SetTurn( 1 );
        currentScore = 0;
        SetScore( 0 );
        ballManager.CreateBall();
    }

    public void SetTurn( int turn )
    {
        this.currentTurn = turn;
        if (turn > MaxTurn)
        {
            return;
        }
        TurnText.text = string.Format( "Turn {0} / {1}", currentTurn, MaxTurn );
    }

    public bool CanCreateBall()
    {
        return currentTurn != MaxTurn && currentScore < TargetScore;
    }

    public void SetScore( int score )
    {
        this.currentScore += score;
        ScoreText.text = string.Format( "Score {0} / {1}", currentScore, TargetScore );
    }

    void Update()
    {
        if (currentScore >= TargetScore || currentTurn > MaxTurn)
        {
            // Game Over
            ChangeGameState( State.GameOver );
        }
    }
}
