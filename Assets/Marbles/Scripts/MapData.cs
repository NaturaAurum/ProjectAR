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
        //if (GUILayout.Button("Generate Map Data"))
        //{
        //    (target as MapData).GenerateMapData();
        //}
        //if (GUILayout.Button("Generate Map"))
        //{
        //    (target as MapData).GenerateMap();
        //}
    }
}

#endif

public class MapData : Initializable
{
    // 링크로만 공유되어있는 구글 스프레드시트 링크 ( 읽기 전용 )
    [Header("스프레드 시트 링크 ( 읽기전용 & 링크전용 )")]
    public string SheetUrl;
    private const string sheetNumber = "2076789463"; // sheetNumbers Data
    public string[][] sheetData;

    public GameObject PlanePrefab;
    public GameObject BoardPrefab;

    //[Header( "게임 판 크기" )]
    //public Vector3 StageScale;

    private List<string> sheetNumbers = new List<string>();

    private Dictionary<string, string> sheetDatas = new Dictionary<string, string>();
    private Dictionary<string, string> mapColorDatas = new Dictionary<string, string>();

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
            //GetGoogleSheet();
        }
    }

    public void GetGoogleSheet()
    {
        sheetData = GoSheet.GetGoogleSheet(SheetUrl, sheetNumber);
        sheetDatas.Clear();
        for (int i = 0; i < sheetData.Length; ++i)
        {
            //sheetNumbers.Add( sheetData[ i ][ j ] );]
            sheetDatas.Add(sheetData[i][0], sheetData[i][1]);
        }

        GenerateMap();
        GetMapColorData();
    }

    public Color GetColorOfBoard(string key){
        return Extensions.ColorExtensions.HexToColor(mapColorDatas[key]);
    }

    private void GetMapColorData(){
        mapColorDatas.Clear();
        string[][] sheet = GoSheet.GetGoogleSheet(SheetUrl, sheetDatas["ColorCode"]);
        for(int i = 1; i < sheet.Length; ++i){
            mapColorDatas.Add(sheet[i][0], sheet[i][1]);
        }
    }

    private Dictionary<int, List<Transform>> boardList = new Dictionary<int, List<Transform>>();

    private void GenerateMap()
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

        //ParseMapData( sheet );
    }

    /// 우선 시트에 있는 데이터가 하나 뿐이니 하나만 가져와서 테스트
    //public void GenerateMapData()
    //{
    //    if (sheetNumbers == null || sheetNumbers.Count <= 0)
    //    {
    //        GetGoogleSheet();
    //    }
    //    string[][] sheet = GoSheet.GetGoogleSheet(SheetUrl, sheetNumbers[1]);
    //    //StageScale = new Vector3(sheet[0].Length, 1, sheet.Length);
    //    //boardDatas.Clear();
    //    //ParseMapData(sheet);
    //}

    private void ParseMapData(string[][] sheet)
    {
        int xLength = sheet[0].Length;
        int yLength = sheet.Length;

        //int[] startIndex = new int[] { 0, 0 };
        //int[] endIndex = new int[] { 0, 0 };

        //preBoardData = new BoardData[ yLength, xLength ];

        Transform boardParent = new GameObject("BoardParent").transform;
        int blockCount = 0;
        List<Transform> boards = new List<Transform>();
        //Dictionary<int, List<Transform>> boardList = new Dictionary<int, List<Transform>>();
        /// Key는 Board 오브젝트, Value는 부모로 쓰일 번호
        //Transform[] prevBoards = new Transform[ xLength ];
        List<Transform> prevBoards = new List<Transform>();
        Dictionary<Transform, Transform> boardList = new Dictionary<Transform, Transform>();
        for (int i = 0; i < yLength; ++i)
        {
            string[] ySheet = sheet[i];
            for (int j = 0; j < xLength; ++j)
            {
                if (j < xLength - 1 && ySheet[j].Equals(ySheet[j + 1]))
                {
                    var boardObj = Instantiate(BoardPrefab, Vector3.zero, Quaternion.identity, boardParent).transform;
                    var boardXZ = Vector3.zero;
                    var boardScaleX = boardObj.localScale.x;
                    boardXZ.x = (boardScaleX) * j;
                    boardXZ.z = (-boardScaleX) * i;
                    boardObj.position = boardXZ;

                    GameBoard board = boardObj.GetComponent<GameBoard>();

                    string boardScore = sheet[i][j];
                    if (boardScore.Length > 1)
                    {
                        board.Prefix = boardScore[0].ToString();
                        board.Score = int.Parse(boardScore[1].ToString());
                    }
                    else
                    {
                        board.Score = int.Parse(boardScore[0].ToString());
                    }
                    board.mapData = this;
                    boards.Add(boardObj);
                }
                else
                {

                    if (i > 0)
                    {
                        var prevYSheet = sheet[i - 1];
                        Transform parent = null;
                        if (ySheet[j].Equals(prevYSheet[j]))
                        {
                            parent = boardList[prevBoards[j * i]];
                        }

                        if (parent)
                        {
                            foreach (var board in boards)
                            {
                                boardList.Add(board, parent);
                            }
                            continue;
                        }
                    }
                    blockCount++;

                    var block = new GameObject(blockCount.ToString());
                    block.transform.SetParent(boardParent);

                    foreach (var board in boards)
                    {
                        boardList.Add(board, block.transform);
                    }

                    prevBoards.AddRange(boards.ToArray());
                    boards = new List<Transform>();
                }
            }

            //prevBoards = new List<Transform>();
        }

        foreach (var b in boardList)
        {
            b.Key.SetParent(b.Value);
        }
    }

    //public void GenerateMap()
    //{
    //    if (PlanePrefab == null || BoardPrefab == null)
    //    {
    //        Debug.LogError("ERROR! Not Linked Prefab");
    //    }

    //    var plane = Instantiate(PlanePrefab, Vector3.zero, Quaternion.identity);
    //    plane.transform.localScale = StageScale / 1000;

    //    var xOffset = (StageScale.x - 2) / 2;
    //    var zOffset = (StageScale.z - 2) / 2;

    //    foreach (var data in boardDatas)
    //    {
    //        var board = Instantiate(BoardPrefab).transform;
    //        board.localScale = new Vector3(StageScale.x / 1000f, 1 / 1000f, StageScale.x / 1000f);
    //        board.localScale /= 2;
    //        board.SetParent( plane.transform );
    //        Vector3 boardLocalScale = board.localScale;
    //        if (data.startIndex[0] != data.endIndex[0])
    //        {
    //            boardLocalScale.z = Mathf.Abs((data.startIndex[0] - data.endIndex[0]));
    //        }
    //        if (data.startIndex[1] != data.endIndex[1])
    //        {
    //            boardLocalScale.x = Mathf.Abs((data.startIndex[1] - data.endIndex[1]));
    //        }
    //        board.localScale = boardLocalScale;

    //        Vector3 boardLocalPosition = Vector3.zero;

    //        // 0,0 좌상단으로 만들기.
    //        boardLocalPosition.x -= xOffset;
    //        boardLocalPosition.z += xOffset;

    //        // 그다음 EndIndex 따져서 끝으로 옮겨버리고 스케일 체크해서 다시 옮겨주면 얼추 맞을거 같은데?
    //        //boardLocalPosition.x = (StageScale.x - 2) - data.endIndex[1]
    //        //var width = data.endIndex[ 1 ];
    //        //var height = data.endIndex[ 0 ];

    //        //boardLocalPosition.x += width;
    //        //boardLocalPosition.z -= height;
    //        //boardLocalPosition.x -= boardLocalScale.x / 2;
    //        //boardLocalPosition.z += boardLocalScale.z / 2;

    //        board.localPosition = boardLocalPosition;
    //    }
    //}
}

[System.Serializable]
public class BoardData
{
    public int[] startIndex;
    public int[] endIndex;

    public int score;
    public string prefix;
}
