using DonutPlease.Game.Character;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerCamera : MonoBehaviour
{
    private CharacterPlayer _player;
    private Camera _mainCamera;

    private float distanceX = 0f;
    private float distanceY = 20f;
    private float distanceZ = -10f;
    private float roatateX = 60f;
    private float roatateY = 0.0f;

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

        // 카메라 시작 위치 지정
        _mainCamera.transform.position = playerBeforePos + new Vector3(distanceX, distanceY, distanceZ);
        _mainCamera.transform.rotation = Quaternion.Euler( new Vector3(roatateX, roatateY, 0f));
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
