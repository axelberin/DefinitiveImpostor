using System.Collections.Generic;

[System.Serializable]
public class WordCategory
{
    public string CategoryName;
    public CategoryType CategoryType;
    public List<WordData> Words = new();
}

public enum CategoryType
{
    Normal,
    PlayerNames
}