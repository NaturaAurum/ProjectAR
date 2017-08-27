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

    public override int GetOrder{
        get{return 2;}
    }

    private UnityARSessionNativeInterface arSession = null;

    private string targetScoreFormat = "Score {0} / {1}";
    private string scoreFormat = "Score {0}";

    public enum State
    {
        Play,
        Clear,
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
        ballManager = Installer.GetInstance<BallManager>();        
        Installer.GetInstance<BallManager>().CreateBall();
    }

    void AR_AnchorAdded( ARPlaneAnchor anchor )
    {
        SampleGameMap.position = UnityARMatrixOps.GetPosition( anchor.transform );
        UnityARSessionNativeInterface.ARAnchorAddedEvent -= AR_AnchorAdded;
    }

    // private void Awake()
    // {
        
    // }

    void Start()
    {
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

        #if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.M)){
            Fader.Instance.FadeIn(0.6f).LoadLevel("Menu").FadeOut(0.6f);
        }
        #endif
    }
}
