using DonutPlease.Game.Character;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerCamera : MonoBehaviour
{
    private CharacterPlayer _player;
    private Camera _mainCamera;

    [SerializeField]
    private Vector3 _offset;

    public float RoatateX { get; private set; } =  45f;
    public float RoatateY { get; private set; } = -45f;

    private Vector3 playerBeforePos;

    private void LateUpdate()
    {
        FollowPlayer();
    }

    public void Initialize(CharacterPlayer player)
    {
        _player = player;
        _mainCamera = Camera.main;

        if (_player != null)
            playerBeforePos = _player.transform.position;

        Quaternion rotation = Quaternion.Euler(RoatateX, RoatateY, 0f);
        _mainCamera.transform.rotation = rotation;

        Vector3 offset = rotation * _offset;
        _mainCamera.transform.position = playerBeforePos + offset;
    }

    private void FollowPlayer()
    {
        if (_player == null)
            return;

        Vector3 playerCurPos = _player.transform.position;

        Vector3 tmp = transform.position;
        tmp += (playerCurPos - playerBeforePos);
        transform.position = tmp;

        playerBeforePos = playerCurPos;
    }
}
