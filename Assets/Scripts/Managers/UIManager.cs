using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameRoundManager;

public class UIManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GameRoundManager gameRoundManager;

    [Header("Screens")]
    [SerializeField] private GameObject initialScreen;
    [SerializeField] private GameObject setupScreen;
    [SerializeField] private GameObject menuSetupScreen;
    [SerializeField] private GameObject impostorSetupScreen;
    [SerializeField] private GameObject playersSetupScreen;
    [SerializeField] private GameObject categorySetupScreen;
    [SerializeField] private GameObject revealScreen;
    [SerializeField] private GameObject playerRevealSelectionScreen;
    [SerializeField] private GameObject roleHiddenScreen;
    [SerializeField] private GameObject roleVisibleScreen;
    [SerializeField] private GameObject discussionScreen;
    [SerializeField] private GameObject resultsScreen;
    [SerializeField] private GameObject errorScreen;

    [SerializeField] private Button initialButton;

    [Header("Setup")]
    [SerializeField] private Button impostorSettingsButton;
    [SerializeField] private Button playerSettingsButton;
    [SerializeField] private Button categorySettingsButton;
    [SerializeField] private Button backMenuSettingsButton;
    [SerializeField] private Button startGameButton;

    [Header("Setup - Players")]
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private Button addPlayerButton;
    [SerializeField] private Transform playersContent;
    [SerializeField] private PlayerRowUI playerRowPrefab;
    [SerializeField] private GameObject noPlayersConteiner;
    [SerializeField] private Button backPlayerSettingsButton;
    [SerializeField] private Toggle rerollToggle;

    [Header("Setup - Impostors")]
    [SerializeField] private TMP_Text impostorCountText;
    [SerializeField] private Button decreaseImpostorButton;
    [SerializeField] private Button increaseImpostorButton;
    [SerializeField] private Button backImpostorSettingsButton;
    [SerializeField] private Toggle hintsToggle;

    private int impostorCount = 1;

    [Header("Setup - Categories")]
    [SerializeField] private Transform categoriesContent;
    [SerializeField] private CategoryToggleUI categoryTogglePrefab;
    [SerializeField] private WordDatabase wordDatabase;
    [SerializeField] private Button backCategorySettingsButton;
    [SerializeField] private TMP_Text selectedCategoriesText;
    [SerializeField] private Button selectAllCategoriesButton;
    [SerializeField] private Button deselectAllCategoriesButton;

    [Header("Reveal Selection UI")]
    [SerializeField] private Transform revealPlayersContent;
    [SerializeField] private RoleRevealPlayerRowUI revealPlayerRowPrefab;
    [SerializeField] private TMP_Text revealPlayersCounterText;
    [SerializeField] private PlayerProfileData playerProfileData;
    [SerializeField] private Button backRevealSelectionButton;

    [Header("Reveal UI")]
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text playerCounterText;
    [SerializeField] private TMP_Text categoryText;
    [SerializeField] private TMP_Text roleText;
    [SerializeField] private TMP_Text wordText;
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private Image currentPlayerCircleImage;
    [SerializeField] private Image currentPlayerIconImage;
    [SerializeField] private Button revealRoleButton;
    [SerializeField] private Button nextPlayerButton;
    [SerializeField] private Button rerollWordButton;
    [SerializeField] private Button wordDescriptionButton;

    [Header("Word Description UI")]
    [SerializeField] private GameObject wordDescriptionPanel;
    [SerializeField] private TMP_Text wordDescriptionText;
    [SerializeField] private Button closeWordDescriptionButton;

    [Header("Discussion UI")]
    [SerializeField] private TMP_Text discussionTitleText;
    [SerializeField] private Button revealResultsButton;
    [SerializeField] private Image initialPlayerIcon;

    [Header("Results UI")]
    [SerializeField] private TMP_Text impostorsResultText;
    [SerializeField] private TMP_Text wordResultText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    [Header("Error UI")]
    [SerializeField] private TMP_Text errorText;
    [SerializeField] private Button closeErrorButton;

    private readonly List<PlayerData> players = new();
    private readonly List<PlayerRowUI> playerRows = new();
    private readonly List<CategoryToggleUI> categoryToggles = new();
    private readonly List<RoleRevealPlayerRowUI> revealPlayerRows = new();

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        initialButton.onClick.AddListener(ShowSetupScreen);
        startGameButton.onClick.AddListener(StartGameButton);

        impostorSettingsButton.onClick.AddListener(ShowImpostorSettingsScreen);
        playerSettingsButton.onClick.AddListener(ShowPlayersSettingsScreen);
        categorySettingsButton.onClick.AddListener(ShowCategorySettingsScreen);

        backMenuSettingsButton.onClick.AddListener(ShowInitialScreen);
        backImpostorSettingsButton.onClick.AddListener(ShowSetupScreen);
        backPlayerSettingsButton.onClick.AddListener(ShowSetupScreen);
        backCategorySettingsButton.onClick.AddListener(ShowSetupScreen);
        backRevealSelectionButton.onClick.AddListener(BackFromRevealSelection);

        revealRoleButton.onClick.AddListener(RevealRoleButton);
        nextPlayerButton.onClick.AddListener(NextPlayerButton);
        decreaseImpostorButton.onClick.AddListener(DecreaseImpostorCount);
        increaseImpostorButton.onClick.AddListener(IncreaseImpostorCount);
        revealResultsButton.onClick.AddListener(ShowResultsScreen);
        rerollWordButton.onClick.AddListener(RerollWordButton);
        wordDescriptionButton.onClick.AddListener(ShowWordDescription);
        closeWordDescriptionButton.onClick.AddListener(HideWordDescription);
        selectAllCategoriesButton.onClick.AddListener(SelectAllCategories);
        deselectAllCategoriesButton.onClick.AddListener(DeselectAllCategories);

        restartButton.onClick.AddListener(StartGameButton);
        menuButton.onClick.AddListener(ShowSetupScreen);
        closeErrorButton.onClick.AddListener(CloseErrorButton);

        playerNameInput.onEndEdit.AddListener(AddPlayerFromInput);
        addPlayerButton.onClick.AddListener(AddPlayerFromButton);
        playerNameInput.onValueChanged.AddListener(UpdateAddPlayerButtonState);
        UpdateAddPlayerButtonState(playerNameInput.text);

        UpdateNoPlayersConteiner();
        RefreshImpostorCountUI();
        BuildCategoryToggles();
        UpdateStartGameButtonState();
    }

    private void OnEnable()
    {
        gameRoundManager.OnPlayerChanged += HandlePlayerChanged;
        gameRoundManager.OnRoleRevealed += HandleRoleRevealed;
        gameRoundManager.OnRoleHidden += HandleRoleHidden;
        gameRoundManager.OnPlayerRevealCompleted += HandlePlayerRevealCompleted;
        gameRoundManager.OnRevealReset += HandleRevealReset;
        gameRoundManager.OnRevealFinished += HandleRevealFinished;
        gameRoundManager.OnError += HandleError;
    }

    private void OnDisable()
    {
        gameRoundManager.OnPlayerChanged -= HandlePlayerChanged;
        gameRoundManager.OnRoleRevealed -= HandleRoleRevealed;
        gameRoundManager.OnRoleHidden -= HandleRoleHidden;
        gameRoundManager.OnPlayerRevealCompleted -= HandlePlayerRevealCompleted;
        gameRoundManager.OnRevealReset -= HandleRevealReset;
        gameRoundManager.OnRevealFinished -= HandleRevealFinished;
        gameRoundManager.OnError -= HandleError;
    }

    private void Start()
    {
        PrewarmScreens();
        ShowInitialScreen();
    }

    private void PrewarmScreens()
    {
        GameObject[] screens =
        {
            initialScreen,
            setupScreen,
            menuSetupScreen,
            impostorSetupScreen,
            playersSetupScreen,
            categorySetupScreen,
            revealScreen,
            playerRevealSelectionScreen,
            roleHiddenScreen,
            roleVisibleScreen,
            discussionScreen,
            resultsScreen,
            errorScreen,
            wordDescriptionPanel
        };

        foreach (GameObject screen in screens)
        {
            if (screen != null)
                screen.SetActive(true);
        }

        Canvas.ForceUpdateCanvases();

        foreach (GameObject screen in screens)
        {
            if (screen != null)
                screen.SetActive(false);
        }
    }

    private void AddPlayerFromButton()
    {
        AddPlayerFromInput(playerNameInput.text);
    }

    private void UpdateAddPlayerButtonState(string playerName)
    {
        addPlayerButton.interactable = !string.IsNullOrWhiteSpace(playerName);
    }

    private void AddPlayerFromInput(string playerName)
    {
        playerName = playerName.Trim();

        if (string.IsNullOrWhiteSpace(playerName))
        {
            UpdateAddPlayerButtonState(playerNameInput.text);
            return;
        }

        AddPlayer(playerName);

        playerNameInput.text = "";
        UpdateAddPlayerButtonState(playerNameInput.text);

        playerNameInput.ActivateInputField();
    }

    private void AddPlayer(string playerName)
    {
        PlayerData player = new PlayerData(playerName);
        players.Add(player);

        PlayerRowUI row = Instantiate(playerRowPrefab, playersContent);
        playerRows.Add(row);

        row.Setup(playerName, players.Count, () =>
        {
            RemovePlayer(player, row);
        });

        UpdatePlayersUI();
    }

    private void RemovePlayer(PlayerData player, PlayerRowUI row)
    {
        players.Remove(player);
        playerRows.Remove(row);

        Destroy(row.gameObject);

        RefreshPlayerRowsOrder();
        UpdatePlayersUI();
    }

    private void RefreshPlayerRowsOrder()
    {
        for (int i = 0; i < playerRows.Count; i++)
        {
            playerRows[i].SetPlayerNumber(i + 1);
        }
    }

    private void UpdatePlayersUI()
    {
        UpdateNoPlayersConteiner();
        RefreshImpostorCountUI();
        UpdateStartGameButtonState();
    }

    private void UpdateNoPlayersConteiner()
    {
        noPlayersConteiner.SetActive(players.Count <= 0);
    }

    private void BuildCategoryToggles()
    {
        foreach (Transform child in categoriesContent)
            Destroy(child.gameObject);

        categoryToggles.Clear();

        foreach (string categoryName in wordDatabase.GetCategoryNames())
        {
            WordCategory category = wordDatabase.GetCategory(categoryName);

            if (category == null)
                continue;

            CategoryToggleUI toggleUI = Instantiate(categoryTogglePrefab, categoriesContent);
            toggleUI.Setup(category);

            toggleUI.OnValueChanged += UpdateCategoriesUI;

            categoryToggles.Add(toggleUI);
        }

        UpdateCategoriesUI();
    }

    private void UpdateCategoriesUI()
    {
        int selectedCount = GetEnabledCategories().Count;

        if (selectedCategoriesText != null)
        {
            selectedCategoriesText.text = $"{selectedCount} seleccionadas";
        }

        if (selectAllCategoriesButton != null)
            selectAllCategoriesButton.interactable = selectedCount < categoryToggles.Count;

        if (deselectAllCategoriesButton != null)
            deselectAllCategoriesButton.interactable = selectedCount > 0;

        UpdateStartGameButtonState();
    }

    private void SelectAllCategories()
    {
        SetAllCategories(true);
    }

    private void DeselectAllCategories()
    {
        SetAllCategories(false);
    }

    private void SetAllCategories(bool isSelected)
    {
        foreach (CategoryToggleUI toggleUI in categoryToggles)
        {
            toggleUI.SetSelected(isSelected, false);
        }

        UpdateCategoriesUI();
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
        UpdateStartGameButtonState();
    }

    public void StartGameButton()
    {
        GameSettings settings = BuildSettingsFromUI();

        if (settings == null)
            return;

        if (gameRoundManager.StartRound(settings))
            ShowRevealScreen();
    }

    private void UpdateStartGameButtonState()
    {
        bool hasEnoughPlayers = players.Count >= 3;
        bool hasCategorySelected = GetEnabledCategories().Count > 0;
        bool hasAtLeastOneImpostor = impostorCount >= 1;

        startGameButton.interactable = hasEnoughPlayers && hasCategorySelected && hasAtLeastOneImpostor;
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
        playerRevealSelectionScreen.SetActive(false);
        discussionScreen.SetActive(false);
        resultsScreen.SetActive(true);
        errorScreen.SetActive(false);

        List<string> impostorNames = new();

        foreach (PlayerData player in gameRoundManager.Settings.Players)
        {
            if (player.IsImpostor)
                impostorNames.Add(player.PlayerName);
        }

        impostorsResultText.text = string.Join("\n", impostorNames);
        wordResultText.text = gameRoundManager.CurrentWordData.Word;
    }

    public void RevealRoleButton()
    {
        gameRoundManager.RevealCurrentPlayerRole();
    }

    public void NextPlayerButton()
    {
        gameRoundManager.CompleteCurrentPlayerReveal();
    }

    public void CloseErrorButton()
    {
        errorScreen.SetActive(false);
    }

    private void HandlePlayerChanged(PlayerData player, int playerIndex)
    {
        playerNameText.text = player.PlayerName;
        playerCounterText.text = $"Jugador {playerIndex + 1}/{gameRoundManager.Settings.Players.Count}";

        UpdateCurrentPlayerRevealVisual(playerIndex);
        ClearRevealTexts();

        ShowRoleHiddenState();
    }

    private void HandleRoleRevealed(RoleRevealData revealData)
    {
        bool isImpostor = revealData.Role == "Impostor";

        SetTextAndVisibility(categoryText, revealData.HasHint && isImpostor ? "" : $"Categoría: {revealData.Category}");
        SetTextAndVisibility(roleText, isImpostor ? $"{revealData.Role}" : "");
        SetTextAndVisibility(wordText, revealData.HasWord && !isImpostor ? $"{revealData.Word}" : "");
        SetTextAndVisibility(hintText, revealData.HasHint ? $"Pista: {revealData.Hint}" : "");

        ShowRoleVisibleState();
    }

    private void UpdateCurrentPlayerRevealVisual(int playerIndex)
    {
        PlayerRevealVisualData visualData = GetRevealVisualData(playerIndex);

        if (currentPlayerCircleImage != null)
            currentPlayerCircleImage.color = visualData.CircleColor;

        if (currentPlayerIconImage != null)
            currentPlayerIconImage.sprite = visualData.Emoji;
    }

    private void HandleRoleHidden()
    {
        ClearRevealTexts();
        ShowRoleHiddenState();
    }

    private void HandlePlayerRevealCompleted()
    {
        ShowRevealSelectionState();
    }

    private void HandleRevealReset()
    {
        BuildRevealPlayerRows();
        ShowRevealSelectionState();
    }

    private void HandleRevealFinished()
    {
        ShowDiscussionScreen();
    }

    public void RerollWordButton()
    {
        if (!rerollToggle.isOn)
            return;

        gameRoundManager.RerollCurrentWord();
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
        playerRevealSelectionScreen.SetActive(false);
        discussionScreen.SetActive(false);
        resultsScreen.SetActive(false);
        errorScreen.SetActive(false);
        impostorSetupScreen.SetActive(false);
        playersSetupScreen.SetActive(false);
        categorySetupScreen.SetActive(false);
        HideWordDescription();
    }

    private void ShowSetupScreen()
    {
        initialScreen.SetActive(false);
        setupScreen.SetActive(true);
        revealScreen.SetActive(false);
        playerRevealSelectionScreen.SetActive(false);
        discussionScreen.SetActive(false);
        resultsScreen.SetActive(false);
        errorScreen.SetActive(false);

        menuSetupScreen.SetActive(true);
        impostorSetupScreen.SetActive(false);
        playersSetupScreen.SetActive(false);
        categorySetupScreen.SetActive(false);

        HideWordDescription();
    }

    private void ShowImpostorSettingsScreen()
    {
        setupScreen.SetActive(true);

        menuSetupScreen.SetActive(false);
        impostorSetupScreen.SetActive(true);
        playersSetupScreen.SetActive(false);
        categorySetupScreen.SetActive(false);
    }

    private void ShowPlayersSettingsScreen()
    {
        setupScreen.SetActive(true);

        menuSetupScreen.SetActive(false);
        impostorSetupScreen.SetActive(false);
        playersSetupScreen.SetActive(true);
        categorySetupScreen.SetActive(false);
    }

    private void ShowCategorySettingsScreen()
    {
        setupScreen.SetActive(true);

        menuSetupScreen.SetActive(false);
        impostorSetupScreen.SetActive(false);
        playersSetupScreen.SetActive(false);
        categorySetupScreen.SetActive(true);
    }

    private void ShowRevealScreen()
    {
        initialScreen.SetActive(false);
        setupScreen.SetActive(false);
        revealScreen.SetActive(false);
        playerRevealSelectionScreen.SetActive(false);
        discussionScreen.SetActive(false);
        resultsScreen.SetActive(false);
        errorScreen.SetActive(false);

        HideRevealRolePanels();
        HideWordDescription();

        BuildRevealPlayerRows();
        ShowRevealSelectionState();
    }

    private void BuildRevealPlayerRows()
    {
        if (gameRoundManager.Settings == null || gameRoundManager.Settings.Players == null)
            return;

        int playersCount = gameRoundManager.Settings.Players.Count;

        while (revealPlayerRows.Count < playersCount)
        {
            RoleRevealPlayerRowUI row = Instantiate(revealPlayerRowPrefab, revealPlayersContent);
            revealPlayerRows.Add(row);
        }

        for (int i = 0; i < revealPlayerRows.Count; i++)
        {
            bool shouldBeActive = i < playersCount;
            revealPlayerRows[i].gameObject.SetActive(shouldBeActive);

            if (!shouldBeActive)
                continue;

            int playerIndex = i;
            PlayerData player = gameRoundManager.Settings.Players[playerIndex];
            PlayerRevealVisualData visualData = GetRevealVisualData(playerIndex);

            revealPlayerRows[i].Setup(
                player.PlayerName,
                visualData.CircleColor,
                visualData.Emoji,
                gameRoundManager.HasPlayerRevealed(player),
                () =>
                {
                    SelectPlayerForReveal(player);
                }
            );
        }

        UpdateRevealPlayerRows();
    }

    private void SelectPlayerForReveal(PlayerData player)
    {
        gameRoundManager.SelectPlayerForReveal(player);
    }

    private void UpdateRevealPlayerRows()
    {
        if (gameRoundManager.Settings == null || gameRoundManager.Settings.Players == null)
            return;

        for (int i = 0; i < revealPlayerRows.Count; i++)
        {
            PlayerData player = gameRoundManager.Settings.Players[i];
            revealPlayerRows[i].SetRevealed(gameRoundManager.HasPlayerRevealed(player));
        }

        if (revealPlayersCounterText != null)
        {
            revealPlayersCounterText.text =
                $"{gameRoundManager.RevealedPlayerCount}/{gameRoundManager.Settings.Players.Count} revelados";
        }
    }

    private PlayerRevealVisualData GetRevealVisualData(int playerIndex)
    {
        if (playerProfileData != null && playerProfileData.revealPlayerVisuals != null
            && playerProfileData.revealPlayerVisuals.Count > 0)
            return playerProfileData.revealPlayerVisuals[playerIndex % playerProfileData.revealPlayerVisuals.Count];

        return PlayerRevealVisualData.Default;
    }

    private void ShowRevealSelectionState()
    {
        initialScreen.SetActive(false);
        setupScreen.SetActive(false);
        revealScreen.SetActive(false);
        playerRevealSelectionScreen.SetActive(true);
        discussionScreen.SetActive(false);
        resultsScreen.SetActive(false);
        errorScreen.SetActive(false);

        HideRevealRolePanels();
        ClearRevealTexts();
        HideWordDescription();
        UpdateRevealPlayerRows();
    }

    private void ShowDiscussionScreen()
    {
        initialScreen.SetActive(false);
        setupScreen.SetActive(false);
        revealScreen.SetActive(false);
        playerRevealSelectionScreen.SetActive(false);
        discussionScreen.SetActive(true);
        resultsScreen.SetActive(false);
        errorScreen.SetActive(false);

        var players = gameRoundManager.Settings.Players;
        var randomPlayer = players[UnityEngine.Random.Range(0, players.Count)];
        discussionTitleText.text = $"Jugador {randomPlayer.PlayerName} comienza la ronda";
        initialPlayerIcon.sprite = GetRevealVisualData(players.IndexOf(randomPlayer)).Emoji;
        HideWordDescription();
    }

    private void ShowRoleHiddenState()
    {
        initialScreen.SetActive(false);
        setupScreen.SetActive(false);
        playerRevealSelectionScreen.SetActive(false);
        revealScreen.SetActive(true);
        discussionScreen.SetActive(false);
        resultsScreen.SetActive(false);
        errorScreen.SetActive(false);

        roleHiddenScreen.SetActive(true);
        roleVisibleScreen.SetActive(false);

        revealRoleButton.gameObject.SetActive(true);

        nextPlayerButton.gameObject.SetActive(false);
        rerollWordButton.gameObject.SetActive(false);
        wordDescriptionButton.gameObject.SetActive(false);

        HideWordDescription();
    }

    private void ShowRoleVisibleState()
    {
        initialScreen.SetActive(false);
        setupScreen.SetActive(false);
        playerRevealSelectionScreen.SetActive(false);
        revealScreen.SetActive(true);
        discussionScreen.SetActive(false);
        resultsScreen.SetActive(false);
        errorScreen.SetActive(false);

        roleHiddenScreen.SetActive(false);
        roleVisibleScreen.SetActive(true);

        revealRoleButton.gameObject.SetActive(false);
        nextPlayerButton.gameObject.SetActive(true);

        bool isCivilian = !gameRoundManager.CurrentPlayer.IsImpostor;

        rerollWordButton.gameObject.SetActive(isCivilian && rerollToggle.isOn);
        wordDescriptionButton.gameObject.SetActive(isCivilian);

        roleText.color = isCivilian ? Color.white : Color.red;

        HideWordDescription();
    }

    private void HideRevealRolePanels()
    {
        if (roleHiddenScreen != null)
            roleHiddenScreen.SetActive(false);

        if (roleVisibleScreen != null)
            roleVisibleScreen.SetActive(false);

        if (revealRoleButton != null)
            revealRoleButton.gameObject.SetActive(false);

        if (nextPlayerButton != null)
            nextPlayerButton.gameObject.SetActive(false);

        if (rerollWordButton != null)
            rerollWordButton.gameObject.SetActive(false);

        if (wordDescriptionButton != null)
            wordDescriptionButton.gameObject.SetActive(false);

        HideWordDescription();
    }

    private void ShowWordDescription()
    {
        if (wordDescriptionPanel == null || wordDescriptionText == null)
            return;

        WordData currentWord = gameRoundManager.CurrentWordData;

        if (currentWord == null)
        {
            wordDescriptionText.text = "No hay una palabra cargada.";
        }
        else if (string.IsNullOrWhiteSpace(currentWord.Description))
        {
            wordDescriptionText.text = $"{currentWord.Word}: no tiene descripción cargada todavía.";
        }
        else
        {
            wordDescriptionText.text = currentWord.Description;
        }

        wordDescriptionPanel.SetActive(true);
    }

    private void BackFromRevealSelection()
    {
        roleHiddenScreen.SetActive(false);
        roleVisibleScreen.SetActive(false);
        revealScreen.SetActive(false);
        playerRevealSelectionScreen.SetActive(false);

        ShowSetupScreen();
    }

    private void HideWordDescription()
    {
        if (wordDescriptionPanel != null)
            wordDescriptionPanel.SetActive(false);
    }

    private void SetTextAndVisibility(TMP_Text text, string value)
    {
        if (text == null)
            return;

        bool hasValue = !string.IsNullOrWhiteSpace(value);

        text.text = hasValue ? value : "";
        text.gameObject.SetActive(hasValue);
    }

    private void ClearRevealTexts()
    {
        SetTextAndVisibility(categoryText, "");
        SetTextAndVisibility(roleText, "");
        SetTextAndVisibility(wordText, "");
        SetTextAndVisibility(hintText, "");
    }
}
