using System;
using System.Collections.Generic;
using UnityEngine;

public class GameRoundManager : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] private WordDatabase wordDatabase;

    public GameSettings Settings { get; private set; }

    public string CurrentCategory { get; private set; }
    public WordData CurrentWordData { get; private set; }

    public int CurrentPlayerIndex { get; private set; } = -1;
    public PlayerData CurrentPlayer => HasCurrentPlayer ? Settings.Players[CurrentPlayerIndex] : null;

    public bool IsRoundStarted { get; private set; }
    public bool IsRevealFinished { get; private set; }
    public bool IsRoleVisible { get; private set; }
    public bool HasCurrentPlayer => Settings != null && Settings.Players != null && CurrentPlayerIndex >= 0 && CurrentPlayerIndex < Settings.Players.Count;

    public int RevealedPlayerCount => revealedPlayers.Count;

    public event Action<PlayerData, int> OnPlayerChanged;
    public event Action<string> OnRoleRevealed;
    public event Action OnRoleHidden;
    public event Action OnPlayerRevealCompleted;
    public event Action OnRevealReset;
    public event Action OnRevealFinished;
    public event Action<string> OnError;

    private readonly Dictionary<PlayerData, string> impostorHints = new();
    private readonly List<string> availableHints = new();
    private readonly HashSet<PlayerData> revealedPlayers = new();

    public bool StartRound(GameSettings settings)
    {
        if (!CanStartRound(settings))
            return false;

        Settings = settings;

        AssignRandomImpostors(Settings);

        CurrentWordData = GetRandomWordForRound(out string selectedCategory);
        CurrentCategory = selectedCategory;

        if (CurrentWordData == null)
            return false;

        PrepareImpostorHints();

        CurrentPlayerIndex = -1;
        revealedPlayers.Clear();
        IsRoundStarted = true;
        IsRevealFinished = false;
        IsRoleVisible = false;

        return true;
    }

    private void AssignRandomImpostors(GameSettings settings)
    {
        foreach (PlayerData player in settings.Players)
            player.IsImpostor = false;

        int assigned = 0;

        while (assigned < settings.ImpostorCount)
        {
            int randomIndex = UnityEngine.Random.Range(0, settings.Players.Count);
            PlayerData randomPlayer = settings.Players[randomIndex];

            if (randomPlayer.IsImpostor)
                continue;

            randomPlayer.IsImpostor = true;
            assigned++;
        }
    }

    public bool SelectPlayerForReveal(PlayerData player)
    {
        if (!IsRoundValid())
            return false;

        if (IsRevealFinished)
            return false;

        if (player == null)
        {
            SendError("El jugador seleccionado no es válido.");
            return false;
        }

        int playerIndex = Settings.Players.IndexOf(player);

        if (playerIndex < 0)
        {
            SendError("El jugador seleccionado no pertenece a esta partida.");
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
            SendError("El índice del jugador seleccionado no es válido.");
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
        if (!IsCurrentPlayerValid())
            return;

        if (IsRevealFinished)
            return;

        IsRoleVisible = true;

        string text = GetRevealTextForPlayer(CurrentPlayer);
        OnRoleRevealed?.Invoke(text);
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
        if (!IsCurrentPlayerValid())
            return;

        if (!IsRoleVisible)
            return;

        PlayerData completedPlayer = CurrentPlayer;
        revealedPlayers.Add(completedPlayer);

        CurrentPlayerIndex = -1;
        IsRoleVisible = false;

        if (revealedPlayers.Count >= Settings.Players.Count)
        {
            FinishRevealPhase();
            return;
        }

        OnPlayerRevealCompleted?.Invoke();
    }

    // Lo dejo para no romper botones o referencias viejas del Inspector.
    public void GoToNextPlayer()
    {
        CompleteCurrentPlayerReveal();
    }

    private void FinishRevealPhase()
    {
        IsRevealFinished = true;
        IsRoleVisible = false;
        CurrentPlayerIndex = -1;

        OnRevealFinished?.Invoke();
    }

    private string GetRevealTextForPlayer(PlayerData player)
    {
        if (player.IsImpostor)
        {
            string impostorText = $"Sos impostor.\nCategoría: {CurrentCategory}";

            if (Settings.HintsEnabled && impostorHints.TryGetValue(player, out string hint))
                return impostorText + $"\nPista: {hint}";

            return impostorText;
        }

        return $"Categoría: {CurrentCategory}\n\nPalabra: {CurrentWordData.Word}";
    }

    private bool CanStartRound(GameSettings settings)
    {
        if (wordDatabase == null)
        {
            SendError("Falta asignar WordDatabase en el inspector.");
            return false;
        }

        if (settings == null)
        {
            SendError("GameSettings es null.");
            return false;
        }

        if (settings.Players == null || settings.Players.Count < 3)
        {
            SendError("Necesitás al menos 3 jugadores.");
            return false;
        }

        if (settings.EnabledCategories == null || settings.EnabledCategories.Count == 0)
        {
            SendError("Tenés que habilitar al menos una categoría.");
            return false;
        }

        if (settings.ImpostorCount <= 0)
        {
            SendError("Tenés que elegir al menos 1 impostor.");
            return false;
        }

        if (settings.ImpostorCount >= settings.Players.Count)
        {
            SendError("La cantidad de impostores tiene que ser menor a la cantidad de jugadores.");
            return false;
        }

        return true;
    }

    private bool IsRoundValid()
    {
        if (!IsRoundStarted)
        {
            SendError("La partida todavía no empezó.");
            return false;
        }

        if (Settings == null || Settings.Players == null)
        {
            SendError("La configuración de partida no es válida.");
            return false;
        }

        if (CurrentWordData == null)
        {
            SendError("No hay palabra seleccionada.");
            return false;
        }

        return true;
    }

    private bool IsCurrentPlayerValid()
    {
        if (!IsRoundValid())
            return false;

        if (!HasCurrentPlayer)
        {
            SendError("No hay ningún jugador seleccionado para revelar.");
            return false;
        }

        return true;
    }

    public void RerollCurrentWord()
    {
        if (!IsCurrentPlayerValid())
            return;

        if (!IsRoleVisible)
            return;

        if (CurrentPlayer.IsImpostor)
            return;

        WordData newWordData = GetRandomWordForRound(out string selectedCategory);

        if (newWordData == null)
        {
            SendError("No se pudo rollear una nueva palabra.");
            return;
        }

        CurrentWordData = newWordData;
        CurrentCategory = selectedCategory;

        PrepareImpostorHints();

        CurrentPlayerIndex = -1;
        revealedPlayers.Clear();
        IsRevealFinished = false;
        IsRoleVisible = false;

        OnRevealReset?.Invoke();
    }

    private WordData GetRandomWordForRound(out string selectedCategory)
    {
        selectedCategory = Settings.EnabledCategories[
            UnityEngine.Random.Range(0, Settings.EnabledCategories.Count)];

        WordCategory category = wordDatabase.GetCategory(selectedCategory);

        if (category == null)
        {
            SendError($"No existe la categoría: {selectedCategory}");
            return null;
        }

        if (category.CategoryType == CategoryType.PlayerNames)
        {
            return CreatePlayerNameWord(Settings);
        }

        return wordDatabase.GetRandomWordFromEnabledCategories(
            new List<string> { selectedCategory },
            out _
        );
    }

    private WordData CreatePlayerNameWord(GameSettings settings)
    {
        PlayerData randomPlayer =
            settings.Players[UnityEngine.Random.Range(0, settings.Players.Count)];

        return new WordData
        {
            Word = randomPlayer.PlayerName,
            Hints = new()
            {
                "Jugador",
            },
            Description = $"{randomPlayer.PlayerName} es un jugador de esta partida."
        };
    }

    private void PrepareImpostorHints()
    {
        impostorHints.Clear();
        availableHints.Clear();

        if (CurrentWordData == null || CurrentWordData.Hints == null)
            return;

        List<string> allHints = new();

        foreach (string hint in CurrentWordData.Hints)
        {
            if (!string.IsNullOrWhiteSpace(hint))
                allHints.Add(hint);
        }

        if (allHints.Count == 0)
            return;

        foreach (PlayerData player in Settings.Players)
        {
            if (!player.IsImpostor)
                continue;

            if (availableHints.Count == 0)
                availableHints.AddRange(allHints);

            int randomIndex = UnityEngine.Random.Range(0, availableHints.Count);
            impostorHints[player] = availableHints[randomIndex];
            availableHints.RemoveAt(randomIndex);
        }
    }

    private void SendError(string message)
    {
        Debug.LogError(message);
        OnError?.Invoke(message);
    }
}
