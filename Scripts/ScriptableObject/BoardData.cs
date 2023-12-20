using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu]
public class BoardData : ScriptableObject
{

    //public float timeInSeconds;
    public int rows = 0;
    public int cols = 0;
    public BoardRow[] Board;
    public List<SearchingWord> SearchWords = new List<SearchingWord>();

    public void ClearWithEmptyString()
    {
        for(int i = 0; i < cols; i++)
        {
            Board[i].ClearRow();
        }
    }

    public void CreateNewBoard()
    {
        Board = new BoardRow[cols];
        for(int i = 0; i < cols; i++)
        {
            Board[i] = new BoardRow(rows);
        }
    }

    [System.Serializable]
    public class SearchingWord
    {
        public string word;
    }

    [System.Serializable]
    public class BoardRow
    {
        public int size;
        public string[] row;

        public BoardRow(int size)
        {
            CreateRow(size);
        }

        public void CreateRow(int size)
        {
            this.size = size;
            row = new string[this.size];
            ClearRow();
        }

        public void ClearRow()
        {
            for(int i = 0 ; i < this.size; i++) {
                row[i] = " ";
            }
        }
    }
}
