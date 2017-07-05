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
        if (GUILayout.Button("Generate Map"))
        {
            (target as MapData).GenerateMap();
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

    public GameObject PlanePrefab;
    public GameObject BoardPrefab;

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
            for (int j = 0; j < xLength; ++j)
            {
                if (j < xLength - 1 && ySheet[j].Equals(ySheet[j + 1]))
                {
                    continue;
                }
                else
                {
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

                    startIndex = new int[] { i, j + 1 };
                    preBoardData[i, j] = bd;
                    if (i > 0)
                    {
                        var prevYSheet = sheet[i - 1];
                        if (ySheet[j].Equals(prevYSheet[j]))
                        {
                            var data = preBoardData[i - 1, j];
                            if (data != null)
                            {
                                data.endIndex = new int[] { i, j };
                                preBoardData[i - 1, j] = null;
                                preBoardData[i, j] = data;
                            }
                        }
                    }
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

    public void GenerateMap()
    {
        if (PlanePrefab == null || BoardPrefab == null)
        {
            Debug.LogError("ERROR! Not Linked Prefab");
        }

        var plane = Instantiate(PlanePrefab, Vector3.zero, Quaternion.identity);
        plane.transform.localScale = StageScale / 1000;

        var xOffset = (StageScale.x - 2) / 2;
        var zOffset = (StageScale.z - 2) / 2;

        foreach (var data in boardDatas)
        {
            var board = Instantiate(BoardPrefab).transform;
            board.localScale = new Vector3(StageScale.x / 1000f, 1 / 1000f, StageScale.x / 1000f);
            board.localScale /= 2;
            board.SetParent( plane.transform );
            Vector3 boardLocalScale = board.localScale;
            if (data.startIndex[0] != data.endIndex[0])
            {
                boardLocalScale.z = Mathf.Abs((data.startIndex[0] - data.endIndex[0]));
            }
            if (data.startIndex[1] != data.endIndex[1])
            {
                boardLocalScale.x = Mathf.Abs((data.startIndex[1] - data.endIndex[1]));
            }
            board.localScale = boardLocalScale;
			
            Vector3 boardLocalPosition = Vector3.zero;

            // 0,0 좌상단으로 만들기.
            boardLocalPosition.x -= xOffset;
            boardLocalPosition.z += xOffset;

            // 그다음 EndIndex 따져서 끝으로 옮겨버리고 스케일 체크해서 다시 옮겨주면 얼추 맞을거 같은데?
            //boardLocalPosition.x = (StageScale.x - 2) - data.endIndex[1]
            //var width = data.endIndex[ 1 ];
            //var height = data.endIndex[ 0 ];

            //boardLocalPosition.x += width;
            //boardLocalPosition.z -= height;
            //boardLocalPosition.x -= boardLocalScale.x / 2;
            //boardLocalPosition.z += boardLocalScale.z / 2;

            board.localPosition = boardLocalPosition;
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
