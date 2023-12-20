using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameEvents;

public class WordChecker : MonoBehaviour
{

    public GameData currentgameData;
    public GameLevelData gameLevelData;
    private string word;

    private int assignedPoints = 0;
    private int completedWords = 0;
    private Ray _rayUp , _rayDown , _rayLeft , _rayRight , _rayDiagonalLeftUp , _rayDiagonalRightUp , _rayDiagonalLeftDown , _rayDiagonalRightDown ;
    private Ray currentRay = new Ray();
    private Vector3 rayStartPosition;
    private List<int> correctSquareList = new List<int>();

    private void OnEnable()
    {
        GameEvents.OnCheckSquare += SquareSelected;
        GameEvents.OnClearSelection += ClearSelection;
        GameEvents.OnLoadNextLevel += LoadNextGameLevel;
    }


    private void OnDisable()
    {
        GameEvents.OnCheckSquare -= SquareSelected;
        GameEvents.OnClearSelection -= ClearSelection;
        GameEvents.OnLoadNextLevel -= LoadNextGameLevel;
    }

    private void LoadNextGameLevel()
    {
        SceneManager.LoadScene("GameScene");
    }

    // Start is called before the first frame update
    void Start()
    {
        assignedPoints = 0;
        completedWords = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(assignedPoints > 0 && Application.isEditor)
        {
            Debug.DrawRay(_rayUp.origin , _rayUp.direction * 4 );
            Debug.DrawRay(_rayDown.origin, _rayDown.direction * 4);
            Debug.DrawRay(_rayLeft.origin, _rayLeft.direction * 4);
            Debug.DrawRay(_rayRight.origin, _rayRight.direction * 4);
            Debug.DrawRay(_rayDiagonalLeftUp.origin, _rayDiagonalLeftUp.direction * 4);
            Debug.DrawRay(_rayDiagonalLeftDown.origin, _rayDiagonalLeftDown.direction * 4);
            Debug.DrawRay(_rayDiagonalRightUp.origin, _rayDiagonalRightUp.direction * 4);
            Debug.DrawRay(_rayDiagonalRightDown.origin, _rayDiagonalRightDown.direction * 4);
        }
    }

    private void SquareSelected(string letter, Vector3 squarePosition, int squareIndex)
    {
        if(assignedPoints == 0)
        {
            rayStartPosition = squarePosition;
            correctSquareList.Add(squareIndex);
            word += letter;
            _rayUp = new Ray(new Vector2(squarePosition.x , squarePosition.y) , new Vector2(0f , 1));
            _rayDown = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(0f, -1));
            _rayLeft = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(-1f,0f));
            _rayRight = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(1f, 0));
            _rayDiagonalLeftUp = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(-1, 1));
            _rayDiagonalLeftDown = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(-1, -1));
            _rayDiagonalRightUp = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(1, 1));
            _rayDiagonalRightDown = new Ray(new Vector2(squarePosition.x, squarePosition.y), new Vector2(1, -1));
        }
        else if(assignedPoints == 1)
        {
            correctSquareList.Add(squareIndex);
            currentRay = SelectRay(rayStartPosition, squarePosition);
            GameEvents.SelectSquareMethod(squarePosition);
            word += letter;
            CheckWord();
        }
        else
        {
            if(IsPointOnTheRay(currentRay , squarePosition) )
            {
                correctSquareList.Add(squareIndex);
                GameEvents.SelectSquareMethod(squarePosition);
                word += letter;
                CheckWord();
            }
        }

        assignedPoints++;
        
    }

    private void CheckWord()
    {
        foreach(var searchingWord  in currentgameData.selectedBoardData.SearchWords) {
            if(word == searchingWord.word)
            {
                GameEvents.CorrectWordMethod(word, correctSquareList);
                completedWords++;
                word = string.Empty;
                correctSquareList.Clear();
                CheckBoardCompleted();
                return;
            }
        }
    }

    private bool IsPointOnTheRay(Ray currentRay , Vector3 point)
    {
        var hits = Physics.RaycastAll(currentRay, 100.0f);
        for(int i = 0; i< hits.Length; i++)
        {
            if (hits[i].transform.position == point)
            {
                return true;
            }
        }
        return false;
    }

    private Ray SelectRay(Vector2 firstPosition , Vector2 secondPosition)
    {
        var direction = (secondPosition - firstPosition).normalized;
        float tolerance = 0.01f;
        if(Math.Abs(direction.x) < tolerance && Math.Abs(direction.y - 1f) < tolerance) {
            return _rayUp;
        }
        if (Math.Abs(direction.x) < tolerance && Math.Abs(direction.y - (-1f)) < tolerance)
        {
            return _rayDown;
        }
        if (Math.Abs(direction.x - (-1f)) < tolerance && Math.Abs(direction.y) < tolerance)
        {
            return _rayLeft;
        }
        if (Math.Abs(direction.x - 1f) < tolerance && Math.Abs(direction.y) < tolerance)
        {
            return _rayRight;
        }
        if (direction.x < 0f && direction.y > 0f)
        {
            return _rayDiagonalLeftUp;
        }
        if (direction.x < 0f && direction.y < 0f)
        {
            return _rayDiagonalLeftDown;
        }
        if(direction.x > 0f && direction.y > 0f)
        {
            return _rayDiagonalRightUp;
        }
        if (direction.x > 0f && direction.y < 0f)
        {
            return _rayDiagonalRightDown;
        }
        return _rayDown;
    }

    private void ClearSelection()
    {
        assignedPoints = 0;
        correctSquareList.Clear();
        word = string.Empty;
    }

    private void CheckBoardCompleted()
    {
        bool loadNextCategory = false;

        if(currentgameData.selectedBoardData.SearchWords.Count == completedWords)
        {
            var categoryName = currentgameData.selectedCategoryName;
            var currentBoardIndex = DataSaver.ReadCategoryCurrentIndexValues(categoryName);
            var nextBoardIndex = -1;
            var currentCategoryIndex = 0;
            bool readNextLevelName = false;

            for(int i = 0 ; i < gameLevelData.data.Count; i++) {
                if(readNextLevelName)
                {
                    nextBoardIndex = DataSaver.ReadCategoryCurrentIndexValues(gameLevelData.data[i].CategoryName);
                    readNextLevelName = false;
                }
                if (gameLevelData.data[i].CategoryName == categoryName)
                {
                    readNextLevelName = true;
                    currentCategoryIndex = i;
                }

            }
            var currentLevelSize = gameLevelData.data[currentCategoryIndex].boardData.Count;
            if(currentBoardIndex < currentLevelSize)
            {
                currentBoardIndex++;
            }

            DataSaver.SaveCategoryData(categoryName, currentBoardIndex);

            if(currentBoardIndex >= currentLevelSize)
            {
                currentCategoryIndex++;
                if(currentCategoryIndex < gameLevelData.data.Count)
                {
                    categoryName = gameLevelData.data[currentCategoryIndex].CategoryName;
                    currentBoardIndex = 0;
                    loadNextCategory = true;

                    if(nextBoardIndex <= 0)
                    {
                        DataSaver.SaveCategoryData(categoryName, currentBoardIndex);
                    }
                }
                else
                {
                    SceneManager.LoadScene("SelectCategory");
                }
            }
            else
            {
                GameEvents.BoardCompletedMethod();
            }
            if(loadNextCategory)
            {
                GameEvents.UnlockNextCategoryMethod();
            }
        }
    }
}
