#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(AutoMapMerge))]
public class AutoMapMergeEditor : Editor
{
    void OnEnable()
    {
        AssetNameAttribute.ConnectMemberAssets(target);
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Merge"))
        {
            MergeGameBoard();
        }
    }

    private void MergeGameBoard()
    {
        MergeTools mergeTool = new MergeTools();
        mergeTool.BoardPrefab = (target as AutoMapMerge).GameBoardPrefab;
        var boardParent = (target as AutoMapMerge).transform;
        var rowCount = boardParent.childCount;
        for (int row = 0; row < rowCount; ++row)
        { // 먼저 행을 돌고 열을 돌도록 하자.
            var columnCount = boardParent.GetChild(row).childCount;
            var rowParent = boardParent.GetChild(row);
            for (int column = 0; column < columnCount; ++column)
            //for (int column = columnCount - 1; column >= 0; --column)
            {
                /*if (columnCount == 1) {
					
				} else {
					var currentBoard = rowParent.GetChild (column).GetComponent<GameBoard> ();
					var nextBoard = rowParent.GetChild (column + 1).GetComponent<GameBoard> ();

					if (currentBoard.GetScore () == nextBoard.GetScore ()) {
						mergeTool.AddBoard (currentBoard);
						mergeTool.AddBoard (nextBoard);
					} else {
						
					}
				}*/
                mergeTool.AddBoard(rowParent.GetChild(column).GetComponentInChildren<GameBoard>());
            }
        }
        //mergeTool.PrintBoard();
        mergeTool.ArrangementBoard();
        mergeTool.Merge();

        Object.DestroyImmediate( ( target as AutoMapMerge ).gameObject );
    }
}

public class MergeTools
{
    private Dictionary<int, GameBoardList> boardData;
    private int boardCount = 0;

    public GameObject BoardPrefab = null;

    public MergeTools()
    {
        boardData = new Dictionary<int, GameBoardList>();
    }

    public void AddBoard(GameBoard board)
    {
        var key = boardData.Where(p => p.Value.Exists(board)).Select(p => p.Key);
        var keyList = key.ToList();
        if (keyList.Count > 0)
        {
            var bList = boardData[keyList[0]];
            bList.Add(board);
        }
        else
        {
            GameBoardList newBoards = new GameBoardList();
            newBoards.Add(board);
            boardData.Add(boardCount, newBoards);
            boardCount++;
        }

        /*if (key.Count > 0) {
		} else {
			GameBoardList newBoards = new GameBoardList();
			newBoards.Add (board);
			boardData.Add (boardCount, newBoards);
			boardCount++;
		}*/
    }

    public void PrintBoard()
    {
        string log = "";
        foreach (var boards in boardData)
        {
            var num = boards.Key;
            log = num.ToString();
            log += "{ ";
            foreach (var board in boards.Value.GetBoardList())
            {
                log += " " + board.GetScore();
            }
            log += " }";
            Debug.Log(log);
        }
    }

    private Vector3 GetCenterOfBoard(params GameBoard[] boards)
    {
        var center = Vector3.zero;

        if (boards.Length == 1)
        {
            center = boards[0].transform.position;
        }
        else
        {
            var bounds = new Bounds(boards[0].transform.position,
                                        Vector3.zero);
            for (int i = 1; i < boards.Length; ++i)
            {
                bounds.Encapsulate(boards[i].transform.position);
            }
            center = bounds.center;
        }
        return center;
    }


    public void ArrangementBoard()
    {
        foreach (var boards in boardData)
        {
            GameObject parent = new GameObject(boards.Key.ToString());
            var parentCenter = this.GetCenterOfBoard(boards.Value.GetBoardList().ToArray());
            parent.transform.position = parentCenter;
            foreach (var board in boards.Value.GetBoardList())
            {
                board.transform.parent.SetParent(parent.transform);
            }
        }
    }

    public void Merge()
    {
        var newParent = new GameObject( "Stage" ).transform;
        Vector3 parentPos = Vector3.zero;
        List<GameBoard> gameBoards = new List<GameBoard>();
        foreach(var boards in boardData)
        {
            gameBoards.AddRange( boards.Value.GetBoardList() );
        }

        parentPos = GetCenterOfBoard( gameBoards.ToArray() );
        newParent.position = parentPos;
        foreach (var boards in boardData)
        {
            var boardList = boards.Value.GetBoardList();
            var parent = boardList[ 0 ].transform.parent.parent;
            var mergedBoard = ( GameObject.Instantiate<GameObject>( BoardPrefab, parent.position, Quaternion.identity, newParent ) ).GetComponentInChildren<GameBoard>();
            mergedBoard.CopyBoard(boardList[0]);
            // xDiff, yDiff를 돌면서 체크해보면? xDiffCount올리다가 yDiffCount올라가면 xDiffCount는 0으로 초기화 시카면 결국 크기가 구해지지 않을까?
            int xDiffCount = 1;
            int yDiffCount = 1;
            if(boardList.Count == 1){
                boardList[0].transform.parent.gameObject.SetActive(false);
            }
            for (int i = 0; i < boardList.Count - 1; ++i)
            {
                var board = boardList[i];
                var nextBoard = boardList[i + 1];
                if (IsNearX(board.Index, nextBoard.Index))
                {
                    xDiffCount++;
                }
                if (IsNearY(board.Index, nextBoard.Index))
                {
                    xDiffCount = 1;
                    yDiffCount++;
                }
                board.transform.parent.gameObject.SetActive(false);
                nextBoard.transform.parent.gameObject.SetActive(false);
            }

            var boardScale = mergedBoard.transform.localScale;
            boardScale.x *= xDiffCount;
            boardScale.z *= yDiffCount;
            mergedBoard.transform.localScale = boardScale;
            mergedBoard.SetScore();
            mergedBoard.SetColor();
            Object.DestroyImmediate( parent.gameObject );
            //mergedBoard.SetTextUIFixScale();
        }
    }

    private bool IsNearX(string index1, string index2)
    {
        var splited_0 = index1.Split(',');
        var splited_1 = index2.Split(',');

        int x1 = int.Parse(splited_0[1]);
        int x2 = int.Parse(splited_1[1]);
        return Mathf.Abs(x1 - x2) == 1;
    }

    private bool IsNearY(string index1, string index2)
    {
        var splited_0 = index1.Split(',');
        var splited_1 = index2.Split(',');
        int y1 = int.Parse(splited_0[0]);
        int y2 = int.Parse(splited_1[0]);
        return Mathf.Abs(y1 - y2) == 1;
    }
}

public class GameBoardList
{
    private List<GameBoard> boardList = new List<GameBoard>();

    public void Add(GameBoard board)
    {
        boardList.Add(board);
    }

    public void Remove(GameBoard board)
    {
        boardList.Remove(board);
    }

    public bool Exists(GameBoard board)
    {
        foreach (var _b in boardList)
        {
            if ((_b.GetScore() == board.GetScore()) && this.IsNearBoard(_b.Index, board.Index) &&
                    this.IsSameColor(_b.colorCode, board.colorCode))
            {
                return true;
            }
        }
        return false;
    }

    public List<GameBoard> GetBoardList()
    {
        return boardList;
    }

    private bool IsSameColor(string colorCode0, string colorCode1)
    {
        return colorCode0.Equals(colorCode1);
    }

    private bool IsNearBoard(string index1, string index2)
    {
        var splitIndex1 = index1.Split(',');
        var splitIndex2 = index2.Split(',');

        int x1 = int.Parse(splitIndex1[0]);
        int x2 = int.Parse(splitIndex2[0]);
        int y1 = int.Parse(splitIndex1[1]);
        int y2 = int.Parse(splitIndex2[1]);

        int xDiff = Mathf.Abs(x1 - x2);
        int yDiff = Mathf.Abs(y1 - y2);

        if (xDiff > 1 || yDiff > 1)
        {
            return false;
        }
        return (xDiff ^ yDiff) == 1; // 1이면 근접해있는 거다.
    }
}
#endif
