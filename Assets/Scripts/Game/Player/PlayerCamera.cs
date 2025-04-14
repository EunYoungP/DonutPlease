using DonutPlease.Game.Character;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerCamera : MonoBehaviour
{
    private CharacterPlayer _player;

    private float distanceX = 0f;
    private float distanceY = 11f;
    private float distanceZ = -6f;

    private Vector3 playerBeforePos;

    public PlayerCamera(CharacterPlayer player)
    {
        _player = player;

        Initialize();
    }

    private void LateUpdate()
    {
        FollowPlayer();
    }

    private void Initialize()
    {
        Debug.LogError("Initialize Camera");

        if (_player != null)
            playerBeforePos = _player.transform.position;

        // 카메라 시작 위치 지정
        transform.position = playerBeforePos + new Vector3(distanceX, distanceY, distanceZ);
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
