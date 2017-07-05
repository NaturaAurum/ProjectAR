using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameBoard : MonoBehaviour
{
    private TMP_Text textMesh;

    [SerializeField]
    private int _Score = 0;
    public string Prefix = "";

    public Color BoardColor = Color.white;

    public int Score
    {
        get
        {
            int score = 0;

            switch (Prefix)
            {
                case "-":
                    score = -_Score;
                    break;
                case "+":
                    score = _Score;
                    break;
                case "*":
                    score = Installer.GetInstance<GameManager>().CurrentScore * _Score;
                    break;
            }
            return score;
        }
    }

    void Awake()
    {
        textMesh = GetComponentInChildren<TMP_Text>();

        textMesh.text = Prefix + Score;
    }
}
