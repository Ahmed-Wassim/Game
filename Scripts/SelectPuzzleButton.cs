using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{

    public GameData gameData;
    public GameLevelData levelData;
    public Text categoryText;
    public Image progressBarFilling;

    private string gameSceneName = "GameScene";

    private bool levelLocked;
    // Start is called before the first frame update
    void Start()
    {
        levelLocked = false;
        var button = GetComponent<Button>();
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
        UpdateButtonInf();
        if (levelLocked)
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnButtonClick()
    {
        gameData.selectedCategoryName = gameObject.name;
        SceneManager.LoadScene(gameSceneName);
    }

    private void UpdateButtonInf()
    {
        var currentIndex = -1;
        var totalBoards = 0;

        foreach(var data in levelData.data)
        {
            if(data.CategoryName == gameObject.name)
            {
                currentIndex = DataSaver.ReadCategoryCurrentIndexValues(gameObject.name);
                totalBoards = data.boardData.Count;

                if (levelData.data[0].CategoryName == gameObject.name && currentIndex < 0)
                {
                    DataSaver.SaveCategoryData(levelData.data[0].CategoryName, 0);
                    currentIndex = DataSaver.ReadCategoryCurrentIndexValues(gameObject.name);
                    totalBoards = data.boardData.Count;
                }
            }
        }

        if(currentIndex == -1)
        {
            levelLocked = true;
        }

        categoryText.text = levelLocked ? string.Empty : (currentIndex.ToString() + "/" + totalBoards.ToString());

        progressBarFilling.fillAmount = (currentIndex > 0 && totalBoards > 0) ? ((float) currentIndex / (float) totalBoards) : 0f;
    }
}
