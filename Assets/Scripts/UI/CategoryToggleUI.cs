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

    private WordCategory category;

    public string CategoryId => category != null ? category.CategoryId : string.Empty;
    public bool IsOn => toggle != null && toggle.isOn;

    private void Awake()
    {
        if (categoryNameText != null)
            categoryNameText.raycastTarget = false;
        if (categoryDescriptionText != null)
            categoryDescriptionText.raycastTarget = false;

        toggle?.onValueChanged.AddListener(HandleToggleValueChanged);
    }

    private void OnEnable()
    {
        GameLocalization.LanguageChanged += RefreshLocalizedText;
    }

    private void OnDisable()
    {
        GameLocalization.LanguageChanged -= RefreshLocalizedText;
    }

    private void OnDestroy()
    {
        toggle?.onValueChanged.RemoveListener(HandleToggleValueChanged);
    }

    public void Setup(WordCategory value)
    {
        category = value;
        RefreshLocalizedText();

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

    public void RefreshLocalizedText()
    {
        if (category == null)
            return;

        if (categoryNameText != null)
            categoryNameText.text = category.GetLocalizedName();
        if (categoryDescriptionText != null)
            categoryDescriptionText.text = category.GetLocalizedSubtitle();
    }

    private void HandleToggleValueChanged(bool value)
    {
        RefreshVisualState(value);
        OnValueChanged?.Invoke();
    }

    private void RefreshVisualState(bool active)
    {
        if (checkImage != null)
            checkImage.gameObject.SetActive(active);
    }
}
