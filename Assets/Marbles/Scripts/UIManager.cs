using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ProjectAR.Util.Event;

namespace Assets.Marbles.Scripts
{
    public class UIManager : MonoBehaviour
    {

        private Button retryBtn;
        private Button menuBtn;

        private GameObject gameOverObjectParent;

        private void Awake()
        {
            retryBtn = GameObject.Find( "RetryBtn" ).GetComponent<Button>();
            menuBtn = GameObject.Find( "MenuBtn" ).GetComponent<Button>();

            retryBtn.onClick.AddListener( Retry );
            menuBtn.onClick.AddListener( GotoMenu );

            EventManager.Listen( EventMessage.ChangeGameState, OnGameStateChanged );

            gameOverObjectParent = GameObject.Find( "GameOver" );
        }

        void Retry()
        {
            SceneManager.LoadScene( "InGame" );
        }

        void GotoMenu()
        {

        }

        private void UI_On()
        {
            gameOverObjectParent.SetActive( true );
        }

        private void UI_Off()
        {
            gameOverObjectParent.SetActive( false );
        }

        private void OnGameStateChanged(params object[] args)
        {
            var state = ( GameManager.State )args[ 0 ];
            switch (state)
            {
                case GameManager.State.GameOver:
                    UI_On();
                    break;
                case GameManager.State.Play:
                    UI_Off();
                    break;
            }
        }
    }
}
