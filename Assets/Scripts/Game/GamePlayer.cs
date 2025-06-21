using DonutPlease.Game.Character;
using UnityEngine;

public class GamePlayer
{
    public CharacterPlayer Character { get; private set; }

    public PlayerCurrencyComponent Currency;
    public PlayerGrowthComponent Growth;

    public void Initialize(PlayerData playerData)
    {
        Character = GameObject.FindAnyObjectByType<CharacterPlayer>();
        Character?.Initialize();

        Currency = new PlayerCurrencyComponent();
        Currency?.Initialize(playerData);

        Growth = new PlayerGrowthComponent();
        Growth?.Initialize(playerData);
    }
}
