using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameBoard : MonoBehaviour
{
	private TMP_Text textMesh;

	public int Score = 0;
	public string Prefix = "";

	public Color BoardColor = Color.white;

	public int GetScore(){
		int score = 0;

		switch(Prefix){
			case "-":
			score = -Score;
			break;
			case "+":
			score = Score;
			break;
			case "*":
			score = Installer.GetInstance<GameManager>().CurrentScore * Score;
			break;
		}
		return score;
	}

	void Awake()
	{
		textMesh = GetComponentInChildren<TMP_Text>();

		textMesh.text = Prefix + Score;
	}
}
