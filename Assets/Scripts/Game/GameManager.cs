using DonutPlease.Game.Character;
using DonutPlease.System;
using System;
using System.IO;
using UniRx;
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

    [SerializeField]public UIRoot _uiRoot;

    [SerializeField]public Canvas _canvas;

    [SerializeField]public GameObject _popupsRoot;

    [SerializeField]private Vector3 _startPos;

    [SerializeField]private GameObject _characterPrefab;

    [Header("System")]
    [SerializeField]public LocalMapSystem LocalMap;
    public InteractionSystem Intercation;
    public StoreSystem Store;
    public ResourceSystem Resource;
    public TutorialSystem Tutorial;
    public PopupSystem Popup;

    public DataManager Data;
    public GamePlayer Player { get; private set; }
    public bl_Joystick JoyStick;

    private void Awake()
    {
        Player = new GamePlayer();
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

        if (Store == null)
        {
            Debug.LogError("StoreSystem not found");
            return;
        }

        Popup = new();
        if (Popup == null)
        {
            Debug.LogError("PopupSystem not found");
            return;
        }

        Data = GetComponentInChildren<DataManager>();
        if (Data == null)
        {
            Debug.LogError("DataManager not found");
            return;
        }

        JoyStick = _canvas.GetComponentInChildren<bl_Joystick>();

        Initialize();
    }

    private void OnApplicationQuit()
    {
        Data.Save();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Data.Save();
        }
    }

    private void Initialize()
    {
        CreateActors();

        Debug.Log($"Application.persistentDataPath : {Application.persistentDataPath}");
        Debug.Log($"Application.dataPath : {Application.dataPath}");
        Debug.Log($"Directory.GetParent(Application.dataPath) : {Directory.GetParent(Application.dataPath)}");

        Resource.Initialize();
        ContentLockSystem.Initialize();

        Data.Load(out SaveData data);
        Player.Initialize(data.playerData);
        LocalMap.Initialize();
        Intercation.Initialize();
        Store.Initialize();
    }

    private void CreateActors()
    {
        GameObject playerObject = Instantiate(_characterPrefab, _startPos, Quaternion.identity) as GameObject;
    }
}
