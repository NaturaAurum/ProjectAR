#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor( typeof( AutoMapMerge ) )]
public class AutoMapMergeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button( "Merge" ))
        {
            MergeGameBoard();
        }
    }

    private void MergeGameBoard()
    {
        var transform = ( target as AutoMapMerge ).transform;
        var childCount = transform.childCount;
        Transform parent = null;
        for (int i = 0; i < childCount; ++i)
        {
            var boardCount = transform.GetChild( i ).childCount;
            for (int j = 0; j < boardCount; ++j)
            {
                //if (j - 1 < 0 || i - 1 < 0)
                //{
                //}
                parent = new GameObject( "_" + i * j ).transform;

                var board = transform.GetChild( i ).GetChild( j );
                var gameBoard = board.GetComponent<GameBoard>();
                if (!( i - 1 < 0 ))
                {
                    var upBoard = transform.GetChild( i - 1 ).GetChild( j );
                    var upGameBoard = upBoard.GetComponent<GameBoard>();
                    // 현재 검사중인 보드와 위쪽 보드가 같을 경우
                    if (upGameBoard.GetScore().Equals( gameBoard.GetScore() ))
                    {
                        board.SetParent( upBoard.parent );
                        continue;
                    }
                }

                if (!( j - 1 < 0 ))
                {
                    var prevBoard = transform.GetChild( i ).GetChild( j - 1 );
                    var prevGameBoard = prevBoard.GetComponent<GameBoard>();
                    // 현재 검사중인 보드와 이전 보드가 같을 경우
                    if (prevGameBoard.GetScore().Equals( gameBoard.GetScore() ))
                    {
                        board.SetParent( prevBoard.parent );
                        continue;
                    }
                }

                board.SetParent( parent );
                //if (!( j - 1 < 0 ))
                //{
                //    //var prevBoard = 
                //}
            }
        }
    }
}
#endif
