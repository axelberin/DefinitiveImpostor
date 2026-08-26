using System;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerRole
{
    Civilian,
    Impostor
}

public class GameRoundManager : MonoBehaviour
{
    public readonly struct RoleRevealData
    {
        public readonly string CategoryId;
        public readonly PlayerRole Role;
        public readonly WordData Word;
        public readonly HintData Hint;

        public RoleRevealData(string categoryId, PlayerRole role, WordData word, HintData hint)
        {
            CategoryId = categoryId;
            Role = role;
            Word = word;
            Hint = hint;
        }

        public bool HasWord => Word != null;
        public bool HasHint => Hint != null;
    }

    public readonly struct ErrorData
    {
        public readonly string Key;
        public readonly object[] Arguments;

        public ErrorData(string key, object[] arguments)
        {
            Key = key;
            Arguments = arguments;
        }
    }

    [Header("Database")]
    [SerializeField] private WordDatabase wordDatabase;

    [Header("Impostor Selection")]
    [SerializeField, Min(0.01f)] private float initialImpostorWeight = 1f;
    [SerializeField, Min(0.01f)] private float recentImpostorWeight = 0.15f;
    [SerializeField, Min(0f)] private float weightIncreasePerRound = 1f;
    [SerializeField, Min(0.01f)] private float maximumImpostorWeight = 5f;

    public GameSettings Settings { get; private set; }
    public string CurrentCategoryId { get; private set; }
    public WordData CurrentWordData { get; private set; }

    public int CurrentPlayerIndex { get; private set; } = -1;
    public PlayerData CurrentPlayer => HasCurrentPlayer ? Settings.Players[CurrentPlayerIndex] : null;
    public bool IsRoundStarted { get; private set; }
    public bool IsRevealFinished { get; private set; }
    public bool IsRoleVisible { get; private set; }
    public bool HasCurrentPlayer => Settings != null && Settings.Players != null
        && CurrentPlayerIndex >= 0 && CurrentPlayerIndex < Settings.Players.Count;
    public int RevealedPlayerCount => revealedPlayers.Count;

    public event Action<PlayerData, int> OnPlayerChanged;
    public event Action<RoleRevealData> OnRoleRevealed;
    public event Action OnRoleHidden;
    public event Action OnPlayerRevealCompleted;
    public event Action OnRevealReset;
    public event Action OnRevealFinished;
    public event Action<ErrorData> OnError;

    private readonly Dictionary<PlayerData, HintData> impostorHints = new();
    private readonly Dictionary<PlayerData, float> impostorSelectionWeights = new();
    private readonly List<HintData> availableHints = new();
    private readonly HashSet<PlayerData> revealedPlayers = new();

    public bool StartRound(GameSettings settings)
    {
        if (!CanStartRound(settings))
            return false;

        Settings = settings;
        CurrentWordData = GetRandomWordForRound(out string selectedCategoryId);
        CurrentCategoryId = selectedCategoryId;

        if (CurrentWordData == null)
            return false;

        AssignWeightedImpostors(Settings);
        PrepareImpostorHints();

        CurrentPlayerIndex = -1;
        revealedPlayers.Clear();
        IsRoundStarted = true;
        IsRevealFinished = false;
        IsRoleVisible = false;
        return true;
    }

    public bool SelectPlayerForReveal(PlayerData player)
    {
        if (!IsRoundValid() || IsRevealFinished)
            return false;

        if (player == null)
        {
            SendError("ui.error.invalid_player");
            return false;
        }

        int playerIndex = Settings.Players.IndexOf(player);
        if (playerIndex < 0)
        {
            SendError("ui.error.player_not_in_game");
            return false;
        }

        if (HasPlayerRevealed(player))
            return false;

        CurrentPlayerIndex = playerIndex;
        IsRoleVisible = false;
        OnPlayerChanged?.Invoke(player, playerIndex);
        return true;
    }

    public bool SelectPlayerForReveal(int playerIndex)
    {
        if (!IsRoundValid())
            return false;

        if (playerIndex < 0 || playerIndex >= Settings.Players.Count)
        {
            SendError("ui.error.invalid_player_index");
            return false;
        }

        return SelectPlayerForReveal(Settings.Players[playerIndex]);
    }

    public bool HasPlayerRevealed(PlayerData player)
    {
        return player != null && revealedPlayers.Contains(player);
    }

    public void RevealCurrentPlayerRole()
    {
        if (!IsCurrentPlayerValid() || IsRevealFinished)
            return;

        IsRoleVisible = true;
        OnRoleRevealed?.Invoke(GetRevealDataForPlayer(CurrentPlayer));
    }

    public RoleRevealData GetCurrentRevealData()
    {
        return HasCurrentPlayer ? GetRevealDataForPlayer(CurrentPlayer) : default;
    }

    public void HideCurrentPlayerRole()
    {
        if (!IsCurrentPlayerValid())
            return;

        IsRoleVisible = false;
        OnRoleHidden?.Invoke();
    }

    public void CompleteCurrentPlayerReveal()
    {
        if (!IsCurrentPlayerValid() || !IsRoleVisible)
            return;

        revealedPlayers.Add(CurrentPlayer);
        CurrentPlayerIndex = -1;
        IsRoleVisible = false;

        if (revealedPlayers.Count >= Settings.Players.Count)
        {
            IsRevealFinished = true;
            OnRevealFinished?.Invoke();
            return;
        }

        OnPlayerRevealCompleted?.Invoke();
    }

    // Compatibility for buttons that may still reference the old method.
    public void GoToNextPlayer()
    {
        CompleteCurrentPlayerReveal();
    }

    public void RerollCurrentWord()
    {
        if (!IsCurrentPlayerValid() || !IsRoleVisible || CurrentPlayer.IsImpostor)
            return;

        WordData newWord = GetRandomWordForRound(out string selectedCategoryId);
        if (newWord == null)
        {
            SendError("ui.error.reroll_failed");
            return;
        }

        CurrentWordData = newWord;
        CurrentCategoryId = selectedCategoryId;
        PrepareImpostorHints();

        CurrentPlayerIndex = -1;
        revealedPlayers.Clear();
        IsRevealFinished = false;
        IsRoleVisible = false;
        OnRevealReset?.Invoke();
    }

    private void AssignWeightedImpostors(GameSettings settings)
    {
        foreach (PlayerData player in settings.Players)
        {
            player.IsImpostor = false;
            if (!impostorSelectionWeights.ContainsKey(player))
                impostorSelectionWeights[player] = initialImpostorWeight;
        }

        RemoveInactivePlayersFromImpostorHistory(settings.Players);

        List<PlayerData> candidates = new(settings.Players);
        HashSet<PlayerData> selectedImpostors = new();

        for (int i = 0; i < settings.ImpostorCount; i++)
        {
            PlayerData selected = GetWeightedRandomPlayer(candidates);
            selected.IsImpostor = true;
            selectedImpostors.Add(selected);
            candidates.Remove(selected);
        }

        foreach (PlayerData player in settings.Players)
        {
            impostorSelectionWeights[player] = selectedImpostors.Contains(player)
                ? recentImpostorWeight
                : Mathf.Min(impostorSelectionWeights[player] + weightIncreasePerRound,
                    Mathf.Max(maximumImpostorWeight, initialImpostorWeight));
        }
    }

    private PlayerData GetWeightedRandomPlayer(List<PlayerData> candidates)
    {
        float totalWeight = 0f;
        foreach (PlayerData player in candidates)
            totalWeight += Mathf.Max(0.01f, impostorSelectionWeights[player]);

        float randomValue = UnityEngine.Random.value * totalWeight;
        foreach (PlayerData player in candidates)
        {
            randomValue -= Mathf.Max(0.01f, impostorSelectionWeights[player]);
            if (randomValue <= 0f)
                return player;
        }

        return candidates[candidates.Count - 1];
    }

    private void RemoveInactivePlayersFromImpostorHistory(List<PlayerData> activePlayers)
    {
        List<PlayerData> trackedPlayers = new(impostorSelectionWeights.Keys);
        foreach (PlayerData tracked in trackedPlayers)
        {
            if (!activePlayers.Contains(tracked))
                impostorSelectionWeights.Remove(tracked);
        }
    }

    private RoleRevealData GetRevealDataForPlayer(PlayerData player)
    {
        PlayerRole role = player.IsImpostor ? PlayerRole.Impostor : PlayerRole.Civilian;
        WordData word = player.IsImpostor ? null : CurrentWordData;
        HintData hint = null;

        if (player.IsImpostor && Settings.HintsEnabled)
            impostorHints.TryGetValue(player, out hint);

        return new RoleRevealData(CurrentCategoryId, role, word, hint);
    }

    private WordData GetRandomWordForRound(out string selectedCategoryId)
    {
        selectedCategoryId = Settings.EnabledCategories[
            UnityEngine.Random.Range(0, Settings.EnabledCategories.Count)];

        WordCategory category = wordDatabase.GetCategory(selectedCategoryId);
        if (category == null)
        {
            SendError("ui.error.category_missing",
                GameLocalization.Args("category", selectedCategoryId));
            return null;
        }

        if (category.CategoryType == CategoryType.PlayerNames)
            return CreatePlayerNameWord(Settings);

        WordData word = wordDatabase.GetRandomWordFromEnabledCategories(
            new[] { selectedCategoryId }, out _);

        if (word == null)
            SendError("ui.error.no_available_words");

        return word;
    }

    private static WordData CreatePlayerNameWord(GameSettings settings)
    {
        PlayerData randomPlayer = settings.Players[UnityEngine.Random.Range(0, settings.Players.Count)];
        return new WordData
        {
            WordId = "runtime.current_player",
            RuntimeWord = randomPlayer.PlayerName,
            DescriptionKey = "content.dynamic.player_description",
            DescriptionArguments = new object[] { GameLocalization.Args("player", randomPlayer.PlayerName) },
            Hints = new List<HintData>
            {
                new()
                {
                    HintId = "runtime.current_player.hint",
                    TextKey = "content.dynamic.player_hint"
                }
            }
        };
    }

    private void PrepareImpostorHints()
    {
        impostorHints.Clear();
        availableHints.Clear();

        if (CurrentWordData?.Hints == null)
            return;

        List<HintData> allHints = new();
        foreach (HintData hint in CurrentWordData.Hints)
        {
            if (hint != null && (!string.IsNullOrWhiteSpace(hint.TextKey)
                || !string.IsNullOrWhiteSpace(hint.RuntimeText)))
            {
                allHints.Add(hint);
            }
        }

        foreach (PlayerData player in Settings.Players)
        {
            if (!player.IsImpostor || allHints.Count == 0)
                continue;

            if (availableHints.Count == 0)
                availableHints.AddRange(allHints);

            int randomIndex = UnityEngine.Random.Range(0, availableHints.Count);
            impostorHints[player] = availableHints[randomIndex];
            availableHints.RemoveAt(randomIndex);
        }
    }

    private bool CanStartRound(GameSettings settings)
    {
        if (wordDatabase == null)
            return Fail("ui.error.word_database_missing");
        if (settings == null)
            return Fail("ui.error.settings_null");
        if (settings.Players == null || settings.Players.Count < 3)
            return Fail("ui.error.minimum_players");
        if (settings.EnabledCategories == null || settings.EnabledCategories.Count == 0)
            return Fail("ui.error.no_categories");
        if (settings.ImpostorCount <= 0)
            return Fail("ui.error.minimum_impostors");
        if (settings.ImpostorCount >= settings.Players.Count)
            return Fail("ui.error.too_many_impostors");

        return true;
    }

    private bool IsRoundValid()
    {
        if (!IsRoundStarted)
            return Fail("ui.error.round_not_started");
        if (Settings?.Players == null)
            return Fail("ui.error.invalid_settings");
        if (CurrentWordData == null)
            return Fail("ui.error.no_word");

        return true;
    }

    private bool IsCurrentPlayerValid()
    {
        if (!IsRoundValid())
            return false;
        if (!HasCurrentPlayer)
            return Fail("ui.error.no_player_selected");

        return true;
    }

    private bool Fail(string key, params object[] arguments)
    {
        SendError(key, arguments);
        return false;
    }

    private void SendError(string key, params object[] arguments)
    {
        Debug.LogError($"Localization error key: {key}");
        OnError?.Invoke(new ErrorData(key, arguments));
    }
}
