using System.Collections.Generic;

[System.Serializable]
public class WordCategory
{
    public string CategoryName;
    public List<WordData> Words = new();
}