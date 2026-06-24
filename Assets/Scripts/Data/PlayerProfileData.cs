using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Impostor/Player Profile")]
public class PlayerProfileData : ScriptableObject
{
    public List<PlayerRevealVisualData> revealPlayerVisuals = new();
}

[Serializable]
public class PlayerRevealVisualData
{
    public Color CircleColor = new(1f, 0.25f, 0.25f, 1f);
    public Sprite Emoji;

    public static PlayerRevealVisualData Default => new()
    {
        CircleColor = new Color(1f, 0.25f, 0.25f, 1f),
        Emoji = null
    };
}
