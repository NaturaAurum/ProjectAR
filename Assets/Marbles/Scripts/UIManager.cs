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

        [SerializeField]
        private HorizontalLayoutGroup horizontalLayoutGroup;

        public override Type Type { get { return GetType(); } }

        public override object Instance { get { return this; } }

        public bool MultipleUIMode = true;

        public override void Initalize()
        {

            retryBtn = GameObject.Find("RetryBtn").GetComponent<Button>();
            menuBtn = GameObject.Find("MenuBtn").GetComponent<Button>();

            retryBtn.onClick.AddListener(Retry);
            menuBtn.onClick.AddListener(GotoMenu);

            

            gameManager = Installer.GetInstance<GameManager>();

        }
        private void Awake()
        {
            EventManager.Listen(EventMessage.ChangeGameState, OnGameStateChanged);
            if (MultipleUIMode)
            {
                if (horizontalLayoutGroup == null)
                {
                    horizontalLayoutGroup = GetComponentInChildren<HorizontalLayoutGroup>();
                }
                List<Transform> playerInfos = new List<Transform>();
                foreach (Transform child in horizontalLayoutGroup.transform)
                {
                    child.gameObject.SetActive(false);
                    playerInfos.Add(child);
                }

                switch (Installer.GetInstance<PlayData>().gameMode)
                {
                    case PlayData.GameMode.Single:
                        playerInfos[0].gameObject.SetActive(true);
                        break;
                    case PlayData.GameMode.OneVsOne:
                        playerInfos[0].gameObject.SetActive(true);
                        playerInfos[1].gameObject.SetActive(true);
                        break;
                    case PlayData.GameMode.TwoVsTwo:
                        for (int i = 0; i < 4; ++i)
                        {
                            playerInfos[i].gameObject.SetActive(true);
                        }
                        break;
                }

                horizontalLayoutGroup.SetLayoutHorizontal();
            }
        }

        void Retry()
        {
            gameManager.ResetGame();
            gameManager.ChangeGameState(GameManager.State.Play);
        }

        void GotoMenu()
        {
            Fader.Instance.FadeIn(Config.FadeInTime).LoadLevel("Menu").FadeOut(Config.FadeOutTime);
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
