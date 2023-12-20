using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEvents;

public class GridSquare : MonoBehaviour
{

    public int squareIndex {  get; set; }

    private AlphabetData.LetterData normalLettterData;
    private AlphabetData.LetterData selectedLetterData;
    private AlphabetData.LetterData correctLetterData;

    private SpriteRenderer displayedImage;

    private bool _selected;
    private bool _clicked;
    private int index = -1;

    private bool _correct;

    public void setIndex(int index)
    {
        this.index = index;
    }

    public int getIndex()
    {
        return this.index;
    }


    // Start is called before the first frame update
    void Start()
    {
        _selected = false;
        _clicked = false;
        _correct = false;
        displayedImage = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        GameEvents.OnEnableSquareSelection += OnEnableSquareSelection;
        GameEvents.OnDisableSquareSelection += OnDisableSquareSelection;
        GameEvents.OnSelectSquare += SelectSquare;
        GameEvents.OnCorrectWord += CorrectWord;
    }

    private void OnDisable()
    {
        GameEvents.OnEnableSquareSelection -= OnEnableSquareSelection;
        GameEvents.OnDisableSquareSelection -= OnDisableSquareSelection;
        GameEvents.OnSelectSquare -= SelectSquare;
        GameEvents.OnCorrectWord -= CorrectWord;
    }

    private void CorrectWord(string word , List<int> squareIndexes)
    {
        if (_selected && squareIndexes.Contains(index))
        {
            _correct = true;
            displayedImage.sprite = correctLetterData.image;
        }

        _selected = false;
        _clicked = false;
    }

    public void OnEnableSquareSelection()
    {
        _selected = false;
        _clicked = true;
    }

    public void OnDisableSquareSelection()
    {
        _selected = false;
        _clicked = false;
        if(_correct == true)
        {
            displayedImage.sprite = correctLetterData.image;
        }
        else
        {
            displayedImage.sprite = normalLettterData.image;
        }
    }

    private void SelectSquare(Vector3 position)
    {
        if(this.gameObject.transform.position == position) {
            displayedImage.sprite = selectedLetterData.image;
        }
    }

    public void setSprite(AlphabetData.LetterData normalLettterData, AlphabetData.LetterData selectedLetterData, AlphabetData.LetterData correctLetterData)
    {
        this.normalLettterData = normalLettterData;
        this.selectedLetterData = selectedLetterData;
        this.correctLetterData = correctLetterData;

        GetComponent<SpriteRenderer>().sprite = this.normalLettterData.image;
    }

    private void OnMouseDown()
    {
        OnEnableSquareSelection();
        GameEvents.EnableSquareSelectionMethod();
        CheckSquare();
        displayedImage.sprite = selectedLetterData.image;
    }

    private void OnMouseEnter()
    {
        CheckSquare();
    }

    private void OnMouseUp()
    {
        GameEvents.ClearSelectionMethod();
        GameEvents.DisableSquareSelectionMethod();
    }

    public void CheckSquare()
    {
        if(_selected == false && _clicked == true) {
            _selected = true;
            GameEvents.CheckSquareMethod(normalLettterData.letter , gameObject.transform.position , index);
        }
    }
}
