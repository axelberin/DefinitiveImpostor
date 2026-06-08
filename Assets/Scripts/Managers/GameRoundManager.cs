using System;
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

    public void StartRound(GameSettings settings)
    {
        if (!CanStartRound(settings))
            return;

        Settings = settings;

        AssignRandomImpostors(Settings);

        CurrentWordData = wordDatabase.GetRandomWordFromEnabledCategories(
            Settings.EnabledCategories,
            out string selectedCategory
        );

        if (CurrentWordData == null)
        {
            SendError("No se pudo elegir una palabra.");
            return;
        }

        CurrentCategory = selectedCategory;

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
            if (Settings.HintsEnabled && !string.IsNullOrWhiteSpace(CurrentWordData.Hint))
                return $"Sos impostor.\n\nPista: {CurrentWordData.Hint}";

            return "Sos impostor.";
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

    private void SendError(string message)
    {
        Debug.LogError(message);
        OnError?.Invoke(message);
    }
}