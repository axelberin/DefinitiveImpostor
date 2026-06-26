using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoleRevealPlayerRowUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button rowButton;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private Image emojiImage;
    [SerializeField] private Image circleImage;
    [SerializeField] private GameObject revealedOverlay;
    [SerializeField] private Image checkImage;

    private Action onPressed;
    private bool isRevealed;

    private void Awake()
    {
        if (rowButton == null)
            rowButton = GetComponent<Button>();

        if (revealedOverlay != null)
            checkImage = revealedOverlay.GetComponent<Image>();

        if (playerNameText != null)
            playerNameText.raycastTarget = false;
    }

    public void Setup(string playerName, Color circleColor, Sprite emoji, bool startsRevealed, Action onPressed)
    {
        this.onPressed = onPressed;

        if (playerNameText != null)
            playerNameText.text = playerName;

        if (emojiImage != null)
            emojiImage.sprite = emoji;

        if (circleImage != null)
            circleImage.color = circleColor;

        if (checkImage != null)
            checkImage.color = circleColor;

        if (rowButton != null)
        {
            rowButton.onClick.RemoveAllListeners();
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
