using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class LanguageSelectorUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;

    private bool languageChangeInProgress;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
        if (label == null)
            label = GetComponentInChildren<TMP_Text>(true);

        button?.onClick.AddListener(HandleButtonPressed);
    }

    private void OnEnable()
    {
        GameLocalization.LanguageChanged += Refresh;

        if (GameLocalization.IsInitialized)
        {
            Refresh();
        }
        else
        {
            if (label != null)
                label.text = "EN";
            if (button != null)
                button.interactable = false;
        }
    }

    private void OnDisable()
    {
        GameLocalization.LanguageChanged -= Refresh;
    }

    private void OnDestroy()
    {
        button?.onClick.RemoveListener(HandleButtonPressed);
    }

    public void Setup(Button targetButton, TMP_Text targetLabel)
    {
        button = targetButton;
        label = targetLabel;
        if (Application.isPlaying)
            Refresh();
    }

    private void HandleButtonPressed()
    {
        if (languageChangeInProgress || !GameLocalization.IsReady)
            return;

        StartCoroutine(ChangeLanguage());
    }

    private IEnumerator ChangeLanguage()
    {
        languageChangeInProgress = true;
        if (button != null)
            button.interactable = false;

        yield return GameLocalization.ToggleLanguageAsync();

        languageChangeInProgress = false;
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

        if (button != null)
            button.interactable = GameLocalization.IsReady
                && !GameLocalization.IsChangingLanguage
                && !languageChangeInProgress;
    }
}
