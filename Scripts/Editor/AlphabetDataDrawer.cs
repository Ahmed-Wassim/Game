using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


[CustomEditor(typeof(AlphabetData))]
[CanEditMultipleObjects]
[System.Serializable]


public class AlphabetDataDrawer : Editor
{
    private ReorderableList AlphabetPlainlist;
    private ReorderableList AlphabetNormallist;
    private ReorderableList AlphabetHighlightedlist;
    private ReorderableList AlphabetWronglist;

    private void OnEnable()
    {
        InitalizeReorderableList(ref AlphabetPlainlist , "AlphabetPlain", "Alphabet Plain");
        InitalizeReorderableList(ref AlphabetNormallist, "AlphabetNormal", "Alphabet Normal");
        InitalizeReorderableList(ref AlphabetHighlightedlist, "AlphabetHighlighted", "Alphabet Highlighted");
        InitalizeReorderableList(ref AlphabetWronglist, "AlphabetWrong", "Alphabet Wrong");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        AlphabetPlainlist.DoLayoutList();
        AlphabetNormallist.DoLayoutList();
        AlphabetHighlightedlist.DoLayoutList();
        AlphabetWronglist.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

    private void InitalizeReorderableList(ref ReorderableList list , string propertyName, string listLabel)
    {
        list = new ReorderableList(serializedObject ,serializedObject.FindProperty(propertyName) , true, true, true, true);
        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, listLabel);
        };

        var l = list;

        list.drawElementCallback = (Rect rect,int index , bool isActive, bool isFocused) =>
        {
            var element = l.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("letter"), GUIContent.none);

            EditorGUI.PropertyField(new Rect(rect.x + 70 , rect.y , rect.width - 60 -30 , EditorGUIUtility.singleLineHeight) , 
                element.FindPropertyRelative("image") , GUIContent.none);
        };
    }
}
