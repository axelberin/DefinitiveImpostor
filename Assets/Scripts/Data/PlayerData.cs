[System.Serializable]
public class PlayerData
{
    public string PlayerName;
    public bool IsImpostor;

    public PlayerData(string playerName)
    {
        PlayerName = playerName;
        IsImpostor = false;
    }
}