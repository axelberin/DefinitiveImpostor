using System;
using System.Collections.Generic;

[Serializable]
public class HintData
{
    public string HintId;
    public string TextKey;

    [NonSerialized] public string RuntimeText;

    public string GetLocalizedText()
    {
        return !string.IsNullOrWhiteSpace(RuntimeText)
            ? RuntimeText
            : GameLocalization.GetContent(TextKey);
    }
}

[Serializable]
public class WordData
{
    public string WordId;
    public string WordKey;
    public string DescriptionKey;
    public List<HintData> Hints = new();

    [NonSerialized] public string RuntimeWord;
    [NonSerialized] public string RuntimeDescription;
    [NonSerialized] public object[] DescriptionArguments;

    public string GetLocalizedWord()
    {
        return !string.IsNullOrWhiteSpace(RuntimeWord)
            ? RuntimeWord
            : GameLocalization.GetContent(WordKey);
    }

    public string GetLocalizedDescription()
    {
        if (!string.IsNullOrWhiteSpace(RuntimeDescription))
            return RuntimeDescription;

        return GameLocalization.GetContent(DescriptionKey, DescriptionArguments);
    }
}
