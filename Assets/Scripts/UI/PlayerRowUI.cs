using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRowUI : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private Button removeButton;

    public void Setup(string playerName, Action onRemove)
    {
        playerNameText.text = playerName;

        removeButton.onClick.RemoveAllListeners();
        removeButton.onClick.AddListener(() => onRemove?.Invoke());
    }
}