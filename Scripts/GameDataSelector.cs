using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataSelector : MonoBehaviour
{

    public GameData currentGameData;
    public GameLevelData levelData;

    // Start is called before the first frame update
    void Awake()
    {
        SelectSequentalBoardData();
    }

    private void SelectSequentalBoardData()
    {
        foreach(var data in levelData.data)
        {
            if(data.CategoryName == currentGameData.selectedCategoryName)
            {
                var BoardIndex = DataSaver.ReadCategoryCurrentIndexValues(currentGameData.selectedCategoryName);

                if(BoardIndex < data.boardData.Count)
                {
                    currentGameData.selectedBoardData = data.boardData[BoardIndex];
                }
                else
                {
                    var randomIndex = Random.Range(0, data.boardData.Count);
                    currentGameData.selectedBoardData = data.boardData[randomIndex];
                }
            }
        }
    }
}
