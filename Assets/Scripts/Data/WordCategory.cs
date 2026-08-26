using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WordCategory
{
    public string CategoryId;
    public string NameKey;
    public string SubtitleKey;
    public CategoryType CategoryType;
    public List<WordData> Words = new();

    [HideInInspector] public Color CategoryColor = Color.white;
    [HideInInspector] public Sprite CategoryImage;

    public string GetLocalizedName() => GameLocalization.GetContent(NameKey);
    public string GetLocalizedSubtitle() => GameLocalization.GetContent(SubtitleKey);
}

public enum CategoryType
{
    Normal,
    PlayerNames
}
