using DonutPlease.Game.Character;
using UnityEngine;

public class GamePlayerComponent : MonoBehaviour
{
    private static GamePlayerComponent _gamePlayer;
    public static GamePlayerComponent GamePlayer
    {
        get
        {
            if (_gamePlayer == null)
            {
                GameObject obj = new GameObject("GamePlayerComponent", typeof(GamePlayerComponent));
                obj.transform.SetParent(GameManager.GetGameManager.transform);
                DontDestroyOnLoad(obj);
                _gamePlayer = obj.GetComponent<GamePlayerComponent>();
            }

            return _gamePlayer;
        }
    }

    private CharacterPlayer _player;
    private PlayerCamera _camera;

    public void Initialize()
    {
        _player = new CharacterPlayer();
        _camera = new PlayerCamera(_player);
    }

    public void SetPlayer(CharacterPlayer player)
    {
        _player = player;
    }
}
