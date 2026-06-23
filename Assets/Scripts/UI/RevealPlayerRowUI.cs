using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoleRevealPlayerRowUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button rowButton;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text emojiText;
    [SerializeField] private Image circleImage;
    [SerializeField] private GameObject revealedOverlay;
    [SerializeField] private TMP_Text statusText;

    [Header("Texts")]
    [SerializeField] private string revealedText = "Visto";

    private Action onPressed;
    private bool isRevealed;

    private void Awake()
    {
        if (rowButton == null)
            rowButton = GetComponent<Button>();
    }

    public void Setup(string playerName, Color circleColor, string emoji, bool startsRevealed, Action onPressed)
    {
        this.onPressed = onPressed;

        if (playerNameText != null)
            playerNameText.text = playerName;

        if (emojiText != null)
            emojiText.text = emoji;

        if (circleImage != null)
            circleImage.color = circleColor;

        if (rowButton != null)
        {
            rowButton.onClick.RemoveListener(HandlePressed);
            rowButton.onClick.AddListener(HandlePressed);
        }

        SetRevealed(startsRevealed);
    }

    public void SetRevealed(bool value)
    {
        isRevealed = value;

        if (rowButton != null)
            rowButton.interactable = !isRevealed;

        if (revealedOverlay != null)
            revealedOverlay.SetActive(isRevealed);

        if (statusText != null)
            statusText.text = isRevealed ? revealedText : "";
    }

    private void HandlePressed()
    {
        if (isRevealed)
            return;

        onPressed?.Invoke();
    }

    private void OnDestroy()
    {
        if (rowButton != null)
            rowButton.onClick.RemoveListener(HandlePressed);
    }
}
