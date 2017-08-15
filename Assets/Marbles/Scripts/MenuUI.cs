using System.Collections;
using System.Collections.Generic;
using ProjectAR.Assets.Marbles.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{

    public CustomButton singleButton;
    public CustomButton multiButton;
    public GameObject MultiOption;
    private CustomButton[] multiButtons;

    private void Awake()
    {
        MultiOption.SetActive(false);
        var optionCount = MultiOption.transform.childCount;
        multiButtons = new CustomButton[optionCount];
        for(int i = 0; i < optionCount; ++i){
            multiButtons[i] = MultiOption.transform.GetChild(i).GetComponent<CustomButton>();
            multiButtons[i].OnClick.AddListener(SelectMulti);
        }

        singleButton.OnClick.AddListener(DoSingle);
        multiButton.OnClick.AddListener(DoMulti);
    }

    private void DoSingle()
    {
        
    }

    private void DoMulti()
    {

    }

    private void SelectMulti(GameObject sender)
    {

    }
}
