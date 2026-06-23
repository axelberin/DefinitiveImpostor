using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CategoryToggleUI : MonoBehaviour
{
    public Action OnValueChanged;

    [SerializeField] private TMP_Text categoryNameText;
    [SerializeField] private Toggle toggle;
    [SerializeField] private Image icon;
    [SerializeField] private Color toggleColor = Color.white;

    public string CategoryName { get; private set; }
    public bool IsOn => toggle.isOn;

    private void Awake()
    {
        toggle.onValueChanged.AddListener(_ =>
        {
            OnValueChanged?.Invoke();
        });
    }

    public void Setup(string categoryName)
    {
        CategoryName = categoryName;
        categoryNameText.text = categoryName;
        toggle.isOn = true;
    }
}