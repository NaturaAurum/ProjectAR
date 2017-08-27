using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using ProjectAR.Assets.Marbles.Scripts;


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

        if (GUILayout.Button("Get Map Color Data"))
        {
            (target as MapData).GetMapColorData();
        }

        if (GUILayout.Button("Generate Map"))
        {
           (target as MapData).GenerateMap();
        }

        if(GUILayout.Button("Save Map Data")){

        }

        
    }
}

#endif

public class MapData : Initializable
{
    // 링크로만 공유되어있는 구글 스프레드시트 링크 ( 읽기 전용 )
    [Header("스프레드 시트 링크 ( 읽기전용 & 링크전용 )")]
    public const string SheetUrl = "https://docs.google.com/spreadsheets/d/1RQ8ZgNmPU7pobwaAgeW-KOIkJfRKRm2LXFzYk5yaW5s/edit?usp=sharing";
    private const string sheetNumber = "2076789463"; // sheetNumbers Data
    public string[][] sheetData;

    public GameObject PlanePrefab;
    public GameObject BoardPrefab;

    private Dictionary<string, string> sheetDatas = new Dictionary<string, string>();
    private Dictionary<string, string> mapColorDatas = new Dictionary<string, string>();

    public override Type Type { get { return GetType(); } }

    public override object Instance { get { return this; } }

    //public override void Initalize()
    //{
    //    GetGoogleSheet();
    //    //GetMapColorData();
    //}

    private void Awake()
    {
        GetGoogleSheet();
        GetMapColorData();
    }

    public void GetGoogleSheet()
    {
        sheetData = GoSheet.GetGoogleSheet(SheetUrl, sheetNumber);
        sheetDatas.Clear();
        for (int i = 0; i < sheetData.Length; ++i)
        {
            Debug.Log(sheetData[i][0] + "," + sheetData[i][1]);
            sheetDatas.Add(sheetData[i][0], sheetData[i][1]);
        }
    }

    public Color GetColorOfBoard(string key)
    {
        if(!mapColorDatas.ContainsKey(key)){
            Debug.Log(key + " is not exist on map color data dictionary");
            Debug.Log( "Dictionary info : " + mapColorDatas.Count);
        }
        return Extensions.ColorExtensions.HexToColor(mapColorDatas[key]);
    }

    public void GetMapColorData()
    {
        mapColorDatas.Clear();
        string[][] sheet = GoSheet.GetGoogleSheet(SheetUrl, sheetDatas["ColorCode"]);
        for (int i = 1; i < sheet.Length; ++i)
        {
            mapColorDatas.Add(sheet[i][0], sheet[i][1]);
        }
    }

    public void GenerateMap()
    {
        if (sheetDatas == null || sheetDatas.Count <= 0)
        {
            GetGoogleSheet();
        }

        string[][] sheet = GoSheet.GetGoogleSheet(SheetUrl, sheetDatas["Stage_0"]);
        // j 는 X축
        // i 는 Y축
        Transform boardParent = new GameObject("BoardParent").transform;
        boardParent.gameObject.AddComponent<AutoMapMerge>();
        for (int i = 0; i < sheet.Length; ++i)
        {
            Transform parent = new GameObject(i.ToString()).transform;
            parent.SetParent(boardParent);
            for (int j = 0; j < sheet[i].Length; ++j)
            {
                var boardObj = Instantiate(BoardPrefab, Vector3.zero, Quaternion.identity, parent).transform;
                var boardXZ = Vector3.zero;
                var boardScaleX = boardObj.localScale.x;
                boardXZ.x = (boardScaleX) * j;
                boardXZ.z = (-boardScaleX) * i;
                boardObj.position = boardXZ;

                GameBoard board = boardObj.GetComponentInChildren<GameBoard>();

                string boardScore = sheet[i][j];
                if (boardScore.Length > 2)
                {
                    board.Prefix = boardScore[0].ToString();
                    board.Score = int.Parse(boardScore[1].ToString());
                    board.colorCode = boardScore[2].ToString();
                }
                else
                {
                    board.Score = int.Parse(boardScore[0].ToString());
                    board.colorCode = boardScore[1].ToString();
                }
                board.mapData = this;
                board.SetScore();
                board.Index = i + "," + j;
                boardObj.tag = "Ground";
            }
        }
    }
}
