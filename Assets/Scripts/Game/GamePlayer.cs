using DonutPlease.Game.Character;
using NUnit.Framework.Constraints;
using UnityEngine;


public class GamePlayer
{
    [SerializeField] private CharacterPlayer _character;

    public CharacterPlayer Character => _character;

    private PlayerCurrencyComponent Currency;

    public void Initialize()
    {
        _character = GameObject.FindAnyObjectByType<CharacterPlayer>();
        _character.Initialize();

        Currency = new PlayerCurrencyComponent();
        if (Currency == null)
            Currency.Initialize();
    }
}
