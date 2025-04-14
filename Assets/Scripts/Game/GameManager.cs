using DonutPlease.Game.Character;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _gameManager;
    public static GameManager GetGameManager
    {
        get
        {
            if (_gameManager == null)
            {
                GameObject obj = GameObject.Find("GameManager");
                DontDestroyOnLoad(obj);

                _gameManager = obj.GetComponent<GameManager>();
            }
            return _gameManager;
        }
    }

    [SerializeField]
    public Canvas _canvas;
    [SerializeField]
    private Vector3 _startPos;
    [SerializeField]
    private GameObject _characterPrefab;

    private GamePlayerComponent Player;

    private void Awake()
    {
        Player = GamePlayerComponent.GamePlayer;
        if (Player == null)
        {
            Debug.LogError("Player not found");
            return;
        }

        GetGameManager.Initialize();
    }

    private void Initialize()
    {
        // GameObject Setting
        GameObject playerObject = Instantiate(_characterPrefab, _startPos, Quaternion.identity) as GameObject;

        Player.Initialize();
    }
}
