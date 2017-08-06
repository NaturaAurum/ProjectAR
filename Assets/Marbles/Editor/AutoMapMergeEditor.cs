#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(AutoMapMerge))]
public class AutoMapMergeEditor : Editor
{
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
                mergeTool.AddBoard(rowParent.GetChild(column).GetComponent<GameBoard>());
            }
        }
        //mergeTool.PrintBoard();
        mergeTool.ArrangementBoard();
    }
}

public class MergeTools
{
    private Dictionary<int, GameBoardList> boardData;
    private int boardCount = 0;

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

    public void ArrangementBoard()
    {
        foreach (var boards in boardData)
        {
            GameObject parent = new GameObject(boards.Key.ToString());
            foreach (var board in boards.Value.GetBoardList())
            {
                board.transform.SetParent(parent.transform);
            }
        }
    }

    public void Merge()
    {
        foreach(var boards in boardData)
        {
            var boardList = boards.Value.GetBoardList();
            var parent = boardList[0].transform.parent;
            
        }
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
            if ((_b.GetScore() == board.GetScore()) && this.IsNearBoard(_b.Index, board.Index))
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
        return (xDiff ^ yDiff) == 1; // 1아면 근접해있는 거다.
    }
}
#endif
