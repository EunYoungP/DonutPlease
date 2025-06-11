using DonutPlease.Game.Character;
using NUnit.Framework.Constraints;
using UnityEngine;

public class GamePlayer
{
    [SerializeField] private CharacterPlayer _character;

    public CharacterPlayer Character => _character;

    public PlayerCurrencyComponent Currency;
    public PlayerGrowthComponent Growth;

    public void Initialize(PlayerData playerData)
    {
        _character = GameObject.FindAnyObjectByType<CharacterPlayer>();
        _character?.Initialize();

        Currency = new PlayerCurrencyComponent();
        Currency?.Initialize(playerData);

        Growth = new PlayerGrowthComponent();
        Growth?.Initialize(playerData);
    }
}
