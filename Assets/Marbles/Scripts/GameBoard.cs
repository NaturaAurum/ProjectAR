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

	void Awake()
	{
		textMesh = GetComponentInChildren<TMP_Text>();

		textMesh.text = Prefix + Score;
	}
}
