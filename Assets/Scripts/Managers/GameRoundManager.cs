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

    public int CurrentPlayerIndex { get; private set; }
    public PlayerData CurrentPlayer => Settings.Players[CurrentPlayerIndex];

    public bool IsRoundStarted { get; private set; }
    public bool IsRevealFinished { get; private set; }
    public bool IsRoleVisible { get; private set; }

    public event Action<PlayerData, int> OnPlayerChanged;
    public event Action<string> OnRoleRevealed;
    public event Action OnRoleHidden;
    public event Action OnRevealFinished;
    public event Action<string> OnError;

    private readonly Dictionary<PlayerData, string> impostorHints = new();
    private readonly List<string> availableHints = new();

    public void StartRound(GameSettings settings)
    {
        if (!CanStartRound(settings))
            return;

        Settings = settings;

        AssignRandomImpostors(Settings);

        CurrentWordData = GetRandomWordForRound(out string selectedCategory);
        CurrentCategory = selectedCategory;
        PrepareImpostorHints();

        CurrentPlayerIndex = 0;
        IsRoundStarted = true;
        IsRevealFinished = false;
        IsRoleVisible = false;

        NotifyCurrentPlayer();
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

    public void RevealCurrentPlayerRole()
    {
        if (!IsRoundValid())
            return;

        if (IsRevealFinished)
            return;

        IsRoleVisible = true;

        string text = GetRevealTextForPlayer(CurrentPlayer);
        OnRoleRevealed?.Invoke(text);
    }

    public void HideCurrentPlayerRole()
    {
        if (!IsRoundValid())
            return;

        IsRoleVisible = false;
        OnRoleHidden?.Invoke();
    }

    public void GoToNextPlayer()
    {
        if (!IsRoundValid())
            return;

        HideCurrentPlayerRole();

        CurrentPlayerIndex++;

        if (CurrentPlayerIndex >= Settings.Players.Count)
        {
            FinishRevealPhase();
            return;
        }

        NotifyCurrentPlayer();
    }

    private void FinishRevealPhase()
    {
        IsRevealFinished = true;
        IsRoleVisible = false;

        OnRevealFinished?.Invoke();
    }

    private void NotifyCurrentPlayer()
    {
        OnPlayerChanged?.Invoke(CurrentPlayer, CurrentPlayerIndex);
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

        if (CurrentPlayerIndex < 0 || CurrentPlayerIndex >= Settings.Players.Count)
        {
            SendError("El jugador actual no es válido.");
            return false;
        }

        return true;
    }

    public void RerollCurrentWord()
    {
        if (!IsRoundValid())
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

        CurrentPlayerIndex = 0;
        IsRevealFinished = false;
        IsRoleVisible = false;

        NotifyCurrentPlayer();
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