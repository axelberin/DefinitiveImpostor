using System.Collections.Generic;

[System.Serializable]
public class WordData
{
    public string Word;
    public List<string> Hints = new();
    public string Description;
}