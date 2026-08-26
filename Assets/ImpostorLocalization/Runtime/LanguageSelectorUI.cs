using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class LanguageSelectorUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
        if (label == null)
            label = GetComponentInChildren<TMP_Text>(true);

        button?.onClick.AddListener(GameLocalization.ToggleLanguage);
    }

    private void OnEnable()
    {
        GameLocalization.LanguageChanged += Refresh;
        StartCoroutine(InitializeAndRefresh());
    }

    private void OnDisable()
    {
        GameLocalization.LanguageChanged -= Refresh;
    }

    private void OnDestroy()
    {
        button?.onClick.RemoveListener(GameLocalization.ToggleLanguage);
    }

    public void Setup(Button targetButton, TMP_Text targetLabel)
    {
        button = targetButton;
        label = targetLabel;
        if (Application.isPlaying)
            Refresh();
    }

    private IEnumerator InitializeAndRefresh()
    {
        yield return GameLocalization.Initialize();
        Refresh();
    }

    private void Refresh()
    {
        if (label == null)
            return;

        label.text = GameLocalization.CurrentLanguageCode.StartsWith(
            "es",
            System.StringComparison.OrdinalIgnoreCase)
            ? "ES"
            : "EN";
    }
}
