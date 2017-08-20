using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ProjectAR.Assets.Marbles.Scripts;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(GameBoard))]
public class GameBoardEditor : Editor
{
    private void OnEnable()
    {
        if (!Application.isPlaying)
        {
            (target as GameBoard).SetScore();
            (target as GameBoard).SetColor();
        }
    }
}
#endif
[DefaultExecutionOrder(999)]
public class GameBoard : MonoBehaviour
{
    private TMP_Text textMesh;


    [SerializeField]
    private MeshRenderer meshRenderer;
    private MaterialPropertyBlock materialProperty;

    public MapData mapData;

    //[SerializeField]
    public int _Score = 0;
    public string Prefix = "";

    public string Index = "";

    public string colorCode = "";

    public void CopyBoard(GameBoard board)
    {
        // 합쳐져 있는 애들은 Index필요 없다. 쓸일이 있을까?
        Prefix = board.Prefix;
        colorCode = board.colorCode;
        _Score = board._Score;
        mapData = board.mapData;
    }

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
                case "*":
                    score = Installer.GetInstance<GameManager>().CurrentScore * _Score;
                    break;
                default:
                    score = _Score;
                    break;
            }
            return score;
        }

        set { _Score = value; }
    }
    public void SetScore()
    {
        if (!textMesh)
        {
            textMesh = transform.parent.GetComponentInChildren<TMP_Text>();
        }

        textMesh.text = Prefix + _Score;
    }

    public string GetScore()
    {
        return Prefix + _Score;
    }

    public void SetColor()
    {
        if (materialProperty == null)
        {
            materialProperty = new MaterialPropertyBlock();
            meshRenderer = GetComponent<MeshRenderer>();
        }

        meshRenderer.GetPropertyBlock(materialProperty);
        materialProperty.SetColor("_Color", (mapData.GetColorOfBoard(colorCode)));
        meshRenderer.SetPropertyBlock(materialProperty);
    }

    void Awake()
    {
        SetScore();
        SetColor();
    }
}
