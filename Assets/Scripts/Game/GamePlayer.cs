using DonutPlease.Game.Character;
using UnityEngine;


public class GamePlayer
{
    [SerializeField] private CharacterPlayer _player;

    public CharacterStockComponent Stock { get; private set; }
    
    public void Initialize()
    {
        _player = GameObject.FindAnyObjectByType<CharacterPlayer>();
        _player.Initialize();

        Stock = new CharacterStockComponent();
    }

    public CharacterPlayer GetPlayer()
    {
        return _player;
    }
}
