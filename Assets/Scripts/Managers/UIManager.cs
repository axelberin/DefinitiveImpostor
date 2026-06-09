using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GameRoundManager gameRoundManager;

    [Header("Screens")]
    [SerializeField] private GameObject initialScreen;
    [SerializeField] private GameObject setupScreen;
    [SerializeField] private GameObject revealScreen;
    [SerializeField] private GameObject roleHiddenScreen;
    [SerializeField] private GameObject roleVisibleScreen;
    [SerializeField] private GameObject discussionScreen;
    [SerializeField] private GameObject resultsScreen;
    [SerializeField] private GameObject errorScreen;

    [SerializeField] private Button initialButton;

    [Header("Setup - Players")]
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private Transform playersContent;
    [SerializeField] private PlayerRowUI playerRowPrefab;

    [Header("Setup - Impostors")]
    [SerializeField] private TMP_Text impostorCountText;
    [SerializeField] private Button decreaseImpostorButton;
    [SerializeField] private Button increaseImpostorButton;

    private int impostorCount = 1;

    [Header("Setup - Categories")]
    [SerializeField] private Transform categoriesContent;
    [SerializeField] private CategoryToggleUI categoryTogglePrefab;
    [SerializeField] private WordDatabase wordDatabase;

    [Header("Setup - Options")]
    [SerializeField] private Toggle hintsToggle;
    [SerializeField] private Button startGameButton;

    [Header("Reveal UI")]
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text playerCounterText;
    [SerializeField] private TMP_Text roleText;
    [SerializeField] private Button revealRoleButton;
    [SerializeField] private Button nextPlayerButton;

    [Header("Discussion UI")]
    [SerializeField] private TMP_Text discussionTitleText;
    [SerializeField] private Button revealResultsButton;

    [Header("Results UI")]
    [SerializeField] private TMP_Text impostorsResultText;
    [SerializeField] private TMP_Text wordResultText;
    [SerializeField] private Button restartButton;

    [Header("Error UI")]
    [SerializeField] private TMP_Text errorText;
    [SerializeField] private Button closeErrorButton;

    private readonly List<PlayerData> players = new();
    private readonly List<CategoryToggleUI> categoryToggles = new();

    private void Awake()
    {
        initialButton.onClick.AddListener(ShowSetupScreen);
        startGameButton.onClick.AddListener(StartGameButton);

        revealRoleButton.onClick.AddListener(RevealRoleButton);
        nextPlayerButton.onClick.AddListener(NextPlayerButton);
        decreaseImpostorButton.onClick.AddListener(DecreaseImpostorCount);
        increaseImpostorButton.onClick.AddListener(IncreaseImpostorCount);
        revealResultsButton.onClick.AddListener(ShowResultsScreen);
        restartButton.onClick.AddListener(ShowSetupScreen);
        closeErrorButton.onClick.AddListener(CloseErrorButton);

        playerNameInput.onEndEdit.AddListener(AddPlayerFromInput);

        RefreshImpostorCountUI();
        BuildCategoryToggles();
    }

    private void OnEnable()
    {
        gameRoundManager.OnPlayerChanged += HandlePlayerChanged;
        gameRoundManager.OnRoleRevealed += HandleRoleRevealed;
        gameRoundManager.OnRoleHidden += HandleRoleHidden;
        gameRoundManager.OnRevealFinished += HandleRevealFinished;
        gameRoundManager.OnError += HandleError;
    }

    private void OnDisable()
    {
        gameRoundManager.OnPlayerChanged -= HandlePlayerChanged;
        gameRoundManager.OnRoleRevealed -= HandleRoleRevealed;
        gameRoundManager.OnRoleHidden -= HandleRoleHidden;
        gameRoundManager.OnRevealFinished -= HandleRevealFinished;
        gameRoundManager.OnError -= HandleError;
    }

    private void Start()
    {
        ShowInitialScreen();
    }

    private void AddPlayerFromInput(string playerName)
    {
        if (string.IsNullOrWhiteSpace(playerName))
        {
            HandleError("Escribí un nombre para agregar jugador.");
            return;
        }

        AddPlayer(playerName);
        playerNameInput.text = "";
        playerNameInput.ActivateInputField();
    }

    private void AddPlayer(string playerName)
    {
        PlayerData player = new PlayerData(playerName);
        players.Add(player);

        PlayerRowUI row = Instantiate(playerRowPrefab, playersContent);
        row.Setup(playerName, () =>
        {
            players.Remove(player);
            Destroy(row.gameObject);
        });
    }

    private void BuildCategoryToggles()
    {
        foreach (Transform child in categoriesContent)
            Destroy(child.gameObject);

        categoryToggles.Clear();

        foreach (string category in wordDatabase.GetCategoryNames())
        {
            CategoryToggleUI toggleUI = Instantiate(categoryTogglePrefab, categoriesContent);
            toggleUI.Setup(category);
            categoryToggles.Add(toggleUI);
        }
    }

    private void IncreaseImpostorCount()
    {
        int maxImpostors = Mathf.Max(1, players.Count - 1);

        if (impostorCount < maxImpostors)
        {
            impostorCount++;
            RefreshImpostorCountUI();
        }
    }

    private void DecreaseImpostorCount()
    {
        impostorCount = Mathf.Max(1, impostorCount - 1);
        RefreshImpostorCountUI();
    }

    private void RefreshImpostorCountUI()
    {
        int maxImpostors = Mathf.Max(1, players.Count - 1);

        impostorCount = Mathf.Clamp(impostorCount, 1, maxImpostors);

        impostorCountText.text = impostorCount.ToString();
    }

    public void StartGameButton()
    {
        GameSettings settings = BuildSettingsFromUI();

        if (settings == null)
            return;

        gameRoundManager.StartRound(settings);
        ShowRevealScreen();
    }

    private GameSettings BuildSettingsFromUI()
    {
        GameSettings settings = new()
        {
            Players = new List<PlayerData>(players),
            EnabledCategories = GetEnabledCategories(),
            HintsEnabled = hintsToggle != null && hintsToggle.isOn
        };

        settings.ImpostorCount = impostorCount;

        return settings;
    }

    private List<string> GetEnabledCategories()
    {
        List<string> enabled = new();

        foreach (CategoryToggleUI toggleUI in categoryToggles)
        {
            if (toggleUI.IsOn)
                enabled.Add(toggleUI.CategoryName);
        }

        return enabled;
    }

    private void ShowResultsScreen()
    {
        initialScreen.SetActive(false);
        setupScreen.SetActive(false);
        revealScreen.SetActive(false);
        discussionScreen.SetActive(false);
        resultsScreen.SetActive(true);
        errorScreen.SetActive(false);

        List<string> impostorNames = new();

        foreach (PlayerData player in gameRoundManager.Settings.Players)
        {
            if (player.IsImpostor)
                impostorNames.Add(player.PlayerName);
        }

        impostorsResultText.text = "Impostores:\n" + string.Join("\n", impostorNames);
        wordResultText.text = "La palabra era:\n" + gameRoundManager.CurrentWordData.Word;
    }

    public void RevealRoleButton()
    {
        gameRoundManager.RevealCurrentPlayerRole();
    }

    public void NextPlayerButton()
    {
        gameRoundManager.GoToNextPlayer();
    }

    public void CloseErrorButton()
    {
        errorScreen.SetActive(false);
    }

    private void HandlePlayerChanged(PlayerData player, int playerIndex)
    {
        playerNameText.text = player.PlayerName;
        playerCounterText.text = $"Jugador {playerIndex + 1}/{gameRoundManager.Settings.Players.Count}";
        ShowRoleHiddenState();
    }

    private void HandleRoleRevealed(string roleMessage)
    {
        roleText.text = roleMessage;
        ShowRoleVisibleState();
    }

    private void HandleRoleHidden()
    {
        roleText.text = "";
        ShowRoleHiddenState();
    }

    private void HandleRevealFinished()
    {
        ShowDiscussionScreen();
    }

    private void HandleError(string message)
    {
        errorText.text = message;
        errorScreen.SetActive(true);
    }

    private void ShowInitialScreen()
    {
        initialScreen.SetActive(true);
        setupScreen.SetActive(false);
        revealScreen.SetActive(false);
        discussionScreen.SetActive(false);
        resultsScreen.SetActive(false);
        errorScreen.SetActive(false);
    }

    private void ShowSetupScreen()
    {
        initialScreen.SetActive(false);
        setupScreen.SetActive(true);
        revealScreen.SetActive(false);
        discussionScreen.SetActive(false);
        resultsScreen.SetActive(false);
        errorScreen.SetActive(false);
    }

    private void ShowRevealScreen()
    {
        initialScreen.SetActive(false);
        setupScreen.SetActive(false);
        revealScreen.SetActive(true);
        discussionScreen.SetActive(false);
        errorScreen.SetActive(false);
        resultsScreen.SetActive(false);

        ShowRoleHiddenState();
    }

    private void ShowDiscussionScreen()
    {
        initialScreen.SetActive(false);
        setupScreen.SetActive(false);
        revealScreen.SetActive(false);
        discussionScreen.SetActive(true);
        errorScreen.SetActive(false);

        discussionTitleText.text = "Que comience el juego...";
    }

    private void ShowRoleHiddenState()
    {
        roleHiddenScreen.SetActive(true);
        roleVisibleScreen.SetActive(false);

        revealRoleButton.gameObject.SetActive(true);
        nextPlayerButton.gameObject.SetActive(false);
    }

    private void ShowRoleVisibleState()
    {
        initialScreen.SetActive(false);
        roleHiddenScreen.SetActive(false);
        roleVisibleScreen.SetActive(true);

        revealRoleButton.gameObject.SetActive(false);
        nextPlayerButton.gameObject.SetActive(true);
    }
}