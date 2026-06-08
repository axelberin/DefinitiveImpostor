using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CategoryToggleUI : MonoBehaviour
{
    [SerializeField] private TMP_Text categoryNameText;
    [SerializeField] private Toggle toggle;

    public string CategoryName { get; private set; }
    public bool IsOn => toggle.isOn;

    public void Setup(string categoryName)
    {
        CategoryName = categoryName;
        categoryNameText.text = categoryName;
        toggle.isOn = true;
    }
}