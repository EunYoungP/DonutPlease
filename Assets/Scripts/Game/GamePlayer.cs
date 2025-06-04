using DonutPlease.Game.Character;
using UnityEngine;


public class GamePlayer
{
    [SerializeField] private CharacterPlayer _character;

    public CharacterPlayer Character => _character;

    public void Initialize()
    {
        _character = GameObject.FindAnyObjectByType<CharacterPlayer>();
        _character.Initialize();
    }
}
