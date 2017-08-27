using System.Collections;
using System.Collections.Generic;
using ProjectAR.Assets.Marbles.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    public CustomButton singleButton;
    public CustomButton multiButton;
    public GameObject MultiOption;
    [SerializeField]
    private CustomButton[] multiButtons;
    private CustomButton backButton;

    private PlayData dataInstance;

    private void Awake()
    {
        var optionCount = MultiOption.transform.childCount;
        multiButtons = new CustomButton[optionCount - 1];
        for(int i = 0; i < optionCount - 1; ++i){
            multiButtons[i] = MultiOption.transform.GetChild(i).GetComponent<CustomButton>();
            multiButtons[i].AddListener(SelectMulti);
        }

        backButton = MultiOption.transform.GetChild(MultiOption.transform.childCount - 1).GetComponent<CustomButton>();
        backButton.AddListener(DoBack);

        singleButton.AddListener(DoSingle);
        multiButton.AddListener(DoMulti);

        MultiOption.SetActive(false);
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        dataInstance = Installer.GetInstance<PlayData>();
    }

    private void DoSingle(GameObject sender)
    {
        // TODO : Single Mode
        dataInstance.PlayerCount = 1;
        dataInstance.gameMode = PlayData.GameMode.Single;
        LoadIngameScene();
    }

    private void DoMulti(GameObject sender)
    {
        MultiOption.SetActive(true);
        singleButton.gameObject.SetActive(false);
        multiButton.gameObject.SetActive(false);
    }

    private void DoBack(GameObject sender){
        MultiOption.SetActive(false);
        singleButton.gameObject.SetActive(true);
        multiButton.gameObject.SetActive(true);
    }

    void LoadIngameScene(){
        //Fader.Instance.FadeIn(0.6f).LoadLevel("InGame").FadeOut(0.6f);
        Fader.Instance.FadeIn(Config.FadeInTime).LoadLevel("InGame").FadeOut(Config.FadeOutTime);
    }

    private void SelectMulti(GameObject sender)
    {
        var number = int.Parse( sender.name );
        dataInstance.Players = new List<PlayerData>();
        var datas = PlayerData.CreateDataWithCount( number );
        dataInstance.Players.AddRange( datas );
        //dataInstance.PlayerCount = number;
        if (number == 2)
        {
            dataInstance.gameMode = PlayData.GameMode.OneVsOne;
        }
        else if(number == 4)
        {
            dataInstance.gameMode = PlayData.GameMode.TwoVsTwo;
        }
        LoadIngameScene();
    }
}
