using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Text.RegularExpressions;
using Unity.VisualScripting;

[CustomEditor(typeof(BoardData), false)]
[CanEditMultipleObjects]
[System.Serializable]
public class BoardDataDrawer : Editor
{
    private BoardData gameDataInstance => target as BoardData;
    private ReorderableList dataList;
    private void OnEnable()
    {
        InitializeReorderList(ref dataList, "SearchWords", "Searching Words");
    }

    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        serializedObject.Update();
        DrawColsRowsInputs();
        EditorGUILayout.Space();
        ConvertToUpper();

        if (gameDataInstance.cols > 0 && gameDataInstance.rows > 0)
        {
            DrawBoardTable();
        }

        GUILayout.BeginHorizontal();

        ClearBoardButton();
        FillRandom();

        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
        dataList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(gameDataInstance);
        }
    }

    private void DrawColsRowsInputs()
    {
        var columnsTemp = gameDataInstance.cols;
        var rowsTemp = gameDataInstance.rows;

        gameDataInstance.cols = EditorGUILayout.IntField("Columns", gameDataInstance.cols);
        gameDataInstance.rows = EditorGUILayout.IntField("Rows", gameDataInstance.rows);

        if ((gameDataInstance.cols != columnsTemp || gameDataInstance.rows != rowsTemp) && gameDataInstance.cols > 0 && gameDataInstance.rows > 0)
        {
            gameDataInstance.CreateNewBoard();
        }
    }

    private void DrawBoardTable()
    {
        var tableStyle = new GUIStyle("box");
        tableStyle.padding = new RectOffset(10, 10, 10, 10);
        tableStyle.margin.left = 32;

        var headerColStyle = new GUIStyle();
        headerColStyle.fixedWidth = 35;

        var colStyle = new GUIStyle();
        colStyle.fixedWidth = 50;

        var rowStyle = new GUIStyle();
        rowStyle.fixedHeight = 25;
        rowStyle.fixedWidth = 40;
        rowStyle.alignment = TextAnchor.MiddleCenter;

        var textFieldStyle = new GUIStyle();

        textFieldStyle.normal.background = Texture2D.grayTexture;
        textFieldStyle.normal.textColor = Color.white;
        textFieldStyle.fontStyle = FontStyle.Bold;
        textFieldStyle.alignment = TextAnchor.MiddleCenter;

        EditorGUILayout.BeginHorizontal(tableStyle);

        for (var x = 0; x < gameDataInstance.cols; x++)
        {
            EditorGUILayout.BeginVertical(x == -1 ? headerColStyle : colStyle);
            for (var y = 0; y < gameDataInstance.rows; y++)
            {
                if (x >= 0 && y >= 0)
                {
                    EditorGUILayout.BeginHorizontal(rowStyle);
                    var character = (string)EditorGUILayout.TextArea(gameDataInstance.Board[x].row[y], textFieldStyle);
                    if (gameDataInstance.Board[x].row[y].Length > 1)
                    {
                        character = gameDataInstance.Board[x].row[y].Substring(0, 1);
                    }

                    gameDataInstance.Board[x].row[y] = character;
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void InitializeReorderList(ref ReorderableList list, string propertyName, string listLabel)
    {
        list = new ReorderableList(serializedObject, serializedObject.FindProperty(propertyName), true, true, true, true);

        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, listLabel);
        };

        var l = list;

        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = l.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight)
                , element.FindPropertyRelative("word"), GUIContent.none);
        };
    }

    private void ConvertToUpper()
    {
        if (GUILayout.Button("To Upper"))
        {
            for (var i = 0; i < gameDataInstance.cols; i++)
            {
                for (var j = 0; j < gameDataInstance.rows; j++)
                {
                    var errorCounter = Regex.Matches(gameDataInstance.Board[i].row[j], @"[a-z]").Count;

                    if (errorCounter > 0)
                    {
                        gameDataInstance.Board[i].row[j] = gameDataInstance.Board[i].row[j].ToUpper();
                    }
                }
            }

            foreach (var searchWord in gameDataInstance.SearchWords)
            {
                var errorCounter = Regex.Matches(searchWord.word, @"[a-z]").Count;

                if (errorCounter > 0)
                {
                    searchWord.word = searchWord.word.ToUpper();
                }
            }
        }
    }

    private void ClearBoardButton()
    {
        if(GUILayout.Button("Clear Button"))
        {
            for(int i = 0; i < gameDataInstance.cols; i++)
            {
                for(int j = 0; j < gameDataInstance.rows; j++)
                {
                    gameDataInstance.Board[i].row[j] = " ";
                }
            }
        }
    }

    private void FillRandom()
    {
        if(GUILayout.Button("Fill Random"))
        {
            for (int i = 0; i < gameDataInstance.cols; i++)
            {
                for (int j = 0; j < gameDataInstance.rows; j++)
                {
                    int errorCounter = Regex.Matches(gameDataInstance.Board[i].row[j], @"[a-zA-Z]").Count;
                    string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    int index = Random.Range(0, letters.Length);

                    if(errorCounter == 0)
                    {
                        gameDataInstance.Board[i].row[j] = letters[index].ToString();
                    }
                }
            }
        }   
    }
}
