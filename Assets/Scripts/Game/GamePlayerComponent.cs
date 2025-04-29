using DonutPlease.Game.Character;
using UnityEngine;

public class GamePlayerComponent
{
    [SerializeField]
    private CharacterPlayer _player;
    
    public void Initialize()
    {
        _player = GameObject.FindAnyObjectByType<CharacterPlayer>();
        _player.Initialize();
    }

    public CharacterPlayer GetPlayer()
    {
        return _player;
    }
}
