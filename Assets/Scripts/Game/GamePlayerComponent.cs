using DonutPlease.Game.Character;
using UnityEngine;

public class GamePlayerComponent
{
    public static GamePlayerComponent GamePlayer { get; private set; }

    [SerializeField]
    private CharacterPlayer _player;
    
    public void Initialize()
    {
        _player = GameObject.FindAnyObjectByType<CharacterPlayer>();
        _player.Initialize();
    }

    public void SetPlayer(CharacterPlayer player)
    {
        _player = player;
    }
}
