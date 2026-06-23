using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CategoryToggleUI : MonoBehaviour
{
    public Action OnValueChanged;

    [Header("UI")]
    [SerializeField] private TMP_Text categoryNameText;
    [SerializeField] private TMP_Text categoryDescriptionText;
    [SerializeField] private Image categoryImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image checkImage;
    [SerializeField] private Toggle toggle;

    public string CategoryName { get; private set; }
    public bool IsOn => toggle.isOn;

    private void Awake()
    {
        categoryNameText.raycastTarget = false;
        categoryDescriptionText.raycastTarget = false;

        toggle.onValueChanged.AddListener(_ =>
        {
            RefreshVisualState(toggle.isOn);
            OnValueChanged?.Invoke();
        });
    }

    public void Setup(WordCategory category)
    {
        CategoryName = category.CategoryName;

        categoryNameText.text = category.CategoryName;

        if (categoryDescriptionText != null)
            categoryDescriptionText.text = category.Description;

        if (backgroundImage != null)
            backgroundImage.color = category.CategoryColor;

        if (checkImage != null)
            checkImage.color = category.CategoryColor;

        if (categoryImage != null)
        {
            categoryImage.sprite = category.CategoryImage;
            categoryImage.enabled = category.CategoryImage != null;
        }

        SetSelected(true, false);
    }

    public void SetSelected(bool isSelected, bool notify = true)
    {
        if (toggle == null)
            return;

        if (notify)
            toggle.isOn = isSelected;
        else
            toggle.SetIsOnWithoutNotify(isSelected);

        RefreshVisualState(toggle.isOn);
    }

    private void RefreshVisualState(bool active)
    {
        if (checkImage != null)
            checkImage.gameObject.SetActive(active);
    }
}