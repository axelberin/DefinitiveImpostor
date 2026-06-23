using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WordCategory
{
    public string CategoryName;

    [TextArea]
    [HideInInspector] public string Description;
    [HideInInspector] public Color CategoryColor = Color.white;
    [HideInInspector] public Sprite CategoryImage;

    public CategoryType CategoryType;
    public List<WordData> Words = new();
}

public enum CategoryType
{
    Normal,
    PlayerNames
}