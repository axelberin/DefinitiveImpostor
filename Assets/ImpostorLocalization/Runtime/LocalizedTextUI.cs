using System.Collections;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(TMP_Text))]
public class LocalizedTextUI : MonoBehaviour
{
    [SerializeField] private string tableName = GameLocalization.UiTable;
    [SerializeField] private string entryKey;
    [SerializeField] private TMP_Text target;

    private void Reset()
    {
        target = GetComponent<TMP_Text>();
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

    public void Setup(string table, string key)
    {
        tableName = table;
        entryKey = key;
        if (target == null)
            target = GetComponent<TMP_Text>();
        if (Application.isPlaying)
            Refresh();
    }

    public void Refresh()
    {
        if (!isActiveAndEnabled || string.IsNullOrWhiteSpace(entryKey))
            return;

        if (target == null)
            target = GetComponent<TMP_Text>();

        target.text = tableName == GameLocalization.ContentTable
            ? GameLocalization.GetContent(entryKey)
            : GameLocalization.GetUi(entryKey);
    }

    private IEnumerator InitializeAndRefresh()
    {
        yield return GameLocalization.Initialize();
        Refresh();
    }
}
