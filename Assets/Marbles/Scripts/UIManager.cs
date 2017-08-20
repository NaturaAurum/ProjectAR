using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ProjectAR.Util.Event;
using System;

namespace Assets.Marbles.Scripts
{
    [DefaultExecutionOrder(+9999)]
    public class UIManager : Initializable
    {

        private Button retryBtn;
        private Button menuBtn;

        [SerializeField]
        private GameObject gameOverObjectParent;

        private GameManager gameManager;

        public override Type Type { get { return GetType(); } }

        public override object Instance { get { return this; } }

        public override void Initalize()
        {

            retryBtn = GameObject.Find("RetryBtn").GetComponent<Button>();
            menuBtn = GameObject.Find("MenuBtn").GetComponent<Button>();

            retryBtn.onClick.AddListener(Retry);
            menuBtn.onClick.AddListener(GotoMenu);

            EventManager.Listen(EventMessage.ChangeGameState, OnGameStateChanged);

            gameManager = Installer.GetInstance<GameManager>();

        }
        private void Awake()
        {

        }

        void Retry()
        {
            gameManager.ResetGame();
            gameManager.ChangeGameState(GameManager.State.Play);
        }

        void GotoMenu()
        {

        }

        private void UI_On()
        {
            gameOverObjectParent.SetActive(true);
        }

        private void UI_Off()
        {
            gameOverObjectParent.SetActive(false);
        }

        private void OnGameStateChanged(params object[] args)
        {
            var state = (GameManager.State)args[0];
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
