using System.Collections.Generic;

[System.Serializable]
public class GameSettings
{
    public List<PlayerData> Players = new();
    public List<string> EnabledCategories = new();
    public int ImpostorCount;
    public bool HintsEnabled;
}