using DonutPlease.Game.Character;
using UnityEngine;


public class GamePlayerComponent
{
    [SerializeField] private CharacterPlayer _player;

    public PlayerStockComponent Stock { get; private set; }
    
    public void Initialize()
    {
        _player = GameObject.FindAnyObjectByType<CharacterPlayer>();
        _player.Initialize();

        Stock = new PlayerStockComponent();
    }

    public CharacterPlayer GetPlayer()
    {
        return _player;
    }
}
