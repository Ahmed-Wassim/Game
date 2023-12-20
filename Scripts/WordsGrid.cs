using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UIElements;

public class WordsGrid : MonoBehaviour
{

    public GameData currrentGameData;
    public GameObject gridSquarePrefab;
    public AlphabetData alphabetData;

    public float squareOffset = 0.0f;
    public float topPosition;
     
    private List<GameObject> squareList = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        SpawnGridSquares();
        SetSquaresPosition();
    }

    private void SetSquaresPosition()
    {
        var squareRect = squareList[0].GetComponent<SpriteRenderer>().sprite.rect;
        var squareTransform = squareList[0].GetComponent<Transform>();

        var offset = new Vector2
        {
            x = (squareRect.width * squareTransform.localScale.x + squareOffset) * 0.01f,
            y = (squareRect.height * squareTransform.localScale.y + squareOffset) * 0.01f
        };

        var startPosition = GetFirstSquarePosition();
        int columnNumber = 0;
        int rowNumber = 0;

        foreach(var square in squareList )
        {
            if(rowNumber + 1 > currrentGameData.selectedBoardData.rows)
            {
                columnNumber++;
                rowNumber = 0;
            }

            var positionX = startPosition.x + offset.x * columnNumber;
            var positionY = startPosition.y - offset.y * rowNumber;

            square.GetComponent<Transform>().position = new Vector2 (positionX, positionY);
            rowNumber++;
        }
    }

    private Vector2 GetFirstSquarePosition()
    {
        var startPosition = new Vector2(0f, transform.position.y);
        var squareRect = squareList[0].GetComponent<SpriteRenderer>().sprite.rect;

        var squareTransform = squareList[0].GetComponent <Transform>();
        var squareSize = new Vector2(0f, 0f);

        squareSize.x = squareRect.width * squareTransform.localScale.x;
        squareSize.y = squareRect.height * squareTransform.localScale.y;

        var midWidthPosition = (((currrentGameData.selectedBoardData.cols - 1) * squareSize.x) / 2) * 0.01f;
        var midWidthHeight = (((currrentGameData.selectedBoardData.cols - 1) * squareSize.x) / 2) * 0.01f;

        startPosition.x = (midWidthPosition != 0) ? midWidthPosition * -1 : midWidthPosition;
        startPosition.y = midWidthHeight;

        return startPosition;
    }

        // Update is called once per frame
        void SpawnGridSquares()
    {
        if(currrentGameData != null)
        {
            var squareScale = GetSquareScale(new Vector3(1.5f, 1.5f, 0.1f));

            foreach(var squares in currrentGameData.selectedBoardData.Board)
            {
                foreach(var square in squares.row)
                {
                    var normalLetterData = alphabetData.AlphabetNormal.Find(data => data.letter == square);
                    var selectedLetterData = alphabetData.AlphabetHighlighted.Find(data => data.letter == square);
                    var correctLetterData = alphabetData.AlphabetWrong.Find(data => data.letter == square);

                    if(normalLetterData.image == null || selectedLetterData.image == null)
                    {
                        Debug.LogError("there is an error");

                        if (UnityEditor.EditorApplication.isPlaying)
                        {
                            UnityEditor.EditorApplication.isPlaying = false;
                        }
                    }
                    else
                    {
                        squareList.Add(Instantiate(gridSquarePrefab));
                        squareList[squareList.Count - 1].GetComponent<GridSquare>().setSprite(normalLetterData, correctLetterData, selectedLetterData);
                        squareList[squareList.Count -1 ].transform.SetParent(this.transform);
                        squareList[squareList.Count - 1].GetComponent<Transform>().position = new Vector3(0f, 0f, 0f);
                        squareList[squareList.Count - 1].transform.localScale = squareScale;
                        squareList[squareList.Count - 1].GetComponent<GridSquare>().setIndex(squareList.Count - 1);
                    }
                }
            }
        }
        
    }

    private Vector3 GetSquareScale(Vector3 defaultScale)
    {
        var finalScale = defaultScale;
        var adjustment = 0.01f; // how much will scale down or up specific square

        while (ShouldScaleDown(finalScale))
        {
            finalScale.x -= adjustment;
            finalScale.y -= adjustment;

            if(finalScale.x <= 0 ||finalScale.y <= 0)
            {
                finalScale.x = adjustment;
                finalScale.y = adjustment;
                return finalScale;
            } 
        }

        return finalScale;
    }

    private bool ShouldScaleDown(Vector3 targetScale)
    {
        var squareRect = gridSquarePrefab.GetComponent<SpriteRenderer>().sprite.rect;
        var squareSize = new Vector2(0.0f, 0.0f);
        var startPosition = new Vector2(0f, 0f);

        squareSize.x = (squareRect.width * targetScale.x) + squareOffset;
        squareSize.y = (squareRect.height * targetScale.y) + squareOffset;

        var midWidthPosition = ((currrentGameData.selectedBoardData.cols * squareSize.x) / 2) * 0.01f;
        var midWidthHeight = ((currrentGameData.selectedBoardData.rows * squareSize.y) / 2) * 0.01f;

        startPosition.x = (midWidthPosition != 0) ? midWidthPosition * -1 : midWidthPosition;
        startPosition.y = midWidthHeight;

        return (startPosition.x < GetHalfScreenWidth() * -1 || startPosition.y > topPosition);

    }

    private float GetHalfScreenWidth()
    {
        float height = Camera.main.orthographicSize * 2;
        float width = (1.7f * height) * Screen.width / Screen.height;
        return width / 2;
    }
}
