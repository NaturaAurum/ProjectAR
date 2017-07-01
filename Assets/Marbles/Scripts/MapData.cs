using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(MapData))]
public class MapDataEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Get Google Sheet Data"))
        {
            (target as MapData).GetGoogleSheet();
        }
        if (GUILayout.Button("Generate Map Data"))
        {
            (target as MapData).GenerateMapData();
        }
    }
}

#endif

public class MapData : Initializable
{
    // 링크로만 공유되어있는 구글 스프레드시트 링크 ( 읽기 전용 )
    [Header("스프레드 시트 링크 ( 읽기전용 & 링크전용 )")]
    public string SheetUrl;
    private const string sheetNumber = "2076789463";
    public string[][] sheetData;

    [Header("게임 판 크기")]
    public Vector3 StageScale;

    private List<string> sheetNumbers = new List<string>();

    [SerializeField]
    private List<BoardData> boardDatas = new List<BoardData>();

    //private Dictionary<string, BoardData> preBoardData = new Dictionary<string, BoardData>();
    private BoardData[,] preBoardData;

    public override Type Type { get { return GetType(); } }

    public override object Instance { get { return this; } }

    public override void Initalize()
    {
        if (sheetNumbers == null || sheetNumbers.Count <= 0)
        {
            GetGoogleSheet();
        }
    }

    public void GetGoogleSheet()
    {
        sheetData = GoSheet.GetGoogleSheet(SheetUrl, sheetNumber);
        for (int i = 0; i < sheetData.Length; ++i)
        {
            for (int j = 0; j < sheetData[i].Length; ++j)
            {
                sheetNumbers.Add(sheetData[i][j]);
            }
        }

        GenerateMapData();
    }

    /// 우선 시트에 있는 데이터가 하나 뿐이니 하나만 가져와서 테스트
    public void GenerateMapData()
    {
        if (sheetNumbers == null || sheetNumbers.Count <= 0)
        {
            GetGoogleSheet();
        }
        string[][] sheet = GoSheet.GetGoogleSheet(SheetUrl, sheetNumbers[1]);
        StageScale = new Vector3(sheet[0].Length, 1, sheet.Length);
        boardDatas.Clear();
        ParseMapData(sheet);
    }

    private void ParseMapData(string[][] sheet)
    {
        int xLength = sheet[0].Length;
        int yLength = sheet.Length;

        int[] startIndex = new int[] { 0, 0 };
        int[] endIndex = new int[] { 0, 0 };

        preBoardData = new BoardData[yLength, xLength];

        for (int i = 0; i < yLength; ++i)
        {
            string[] ySheet = sheet[i];
            for (int j = 0; j < xLength - 1; ++j)
            {
                if (ySheet[j].Equals(ySheet[j + 1]))
                {
                    continue;
                }
                else
                {
                    if (i > 0)
                    {
                        var prevYSheet = sheet[i - 1];
                        if (ySheet[j].Equals(prevYSheet[j]))
                        {
                            var data = preBoardData[i - 1, j];
                            data.endIndex = new int[] { i, j };
                            preBoardData[i - 1, j] = null;
                            preBoardData[i, j] = data;
                        }
                    }
                    endIndex = new int[] { i, j };
                    BoardData bd = new BoardData();
                    bd.startIndex = startIndex;
                    bd.endIndex = endIndex;
                    if (ySheet[j].Length > 1)
                    {
                        bd.prefix = ySheet[j][0].ToString();
                        bd.score = int.Parse(ySheet[j][1].ToString());
                    }
                    else
                    {
                        bd.score = int.Parse(ySheet[j]);
                    }

					Debug.Log(bd.score);

                    startIndex = new int[] { i, j + 1 };
                    preBoardData[i, j] = bd;
                }
            }
        }

        for (int i = 0; i < yLength; ++i)
        {
            for (int j = 0; j < xLength; ++j)
            {
                if (preBoardData[i, j] != null)
                {
                    boardDatas.Add(preBoardData[i, j]);
                }
            }
        }
    }
}

[System.Serializable]
public class BoardData
{
    public int[] startIndex;
    public int[] endIndex;

    public int score;
    public string prefix;
}
