using DonutPlease.Game.Character;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class GameManager : MonoBehaviour
{
    [Serializable]
    public class UIRoot
    {
        public GameObject Root;
    }

    [Serializable]
    public class UICommon : UIRoot
    {
    }

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
    public UIRoot _uiRoot;

    [SerializeField]
    public Canvas _canvas;

    [SerializeField]
    private Vector3 _startPos;

    [SerializeField]
    private GameObject _characterPrefab;

    [SerializeField] 
    public LocalMapSystem LocalMap;

    private InteractionSystem Intercation;
    public StoreSystem Store;

    public GamePlayerComponent Player { get; private set; }
    public  bl_Joystick JoyStick;

    private void Awake()
    {
        Player = new GamePlayerComponent();
        if (Player == null)
        {
            Debug.LogError("Player not found");
            return;
        }

        Intercation = new InteractionSystem();
        if (Intercation == null)
        {
            Debug.LogError("InteractionSystem not found");
            return;
        }

        Store = new StoreSystem();
        if (Store == null)
        {
            Debug.LogError("StoreSystem not found");
            return;
        }

        JoyStick = _canvas.GetComponentInChildren<bl_Joystick>();

        CreateActors();

        Initialize();
    }

    private void CreateActors()
    {
        GameObject playerObject = Instantiate(_characterPrefab, _startPos, Quaternion.identity) as GameObject;
    }

    private void Initialize()
    {
        Player.Initialize();
        LocalMap.Initialize();
        Intercation.Initialize();
        Store.Initialize();
    }
}
