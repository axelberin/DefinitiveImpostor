using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRowUI : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text playerNumText;
    [SerializeField] private Button removeButton;

    public void Setup(string playerName, int playerNumber, Action onRemove)
    {
        playerNameText.text = playerName;
        SetPlayerNumber(playerNumber);

        removeButton.onClick.RemoveAllListeners();
        removeButton.onClick.AddListener(() => onRemove?.Invoke());
    }

    public void SetPlayerNumber(int playerNumber)
    {
        playerNumText.text = playerNumber.ToString();
    }
}