using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(GameBoard))]
public class GameBoardEditor : Editor
{
    private void OnEnable()
    {
        ( target as GameBoard ).SetScore();
    }
}
#endif

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
    public void SetScore()
    {
        if (!textMesh)
        {
            textMesh = GetComponentInChildren<TMP_Text>();
        }

        textMesh.text = Prefix + Score;
    }

    public string GetScore()
    {
        return Prefix + Score;
    }

	void Awake()
	{
        SetScore();
	}
}
