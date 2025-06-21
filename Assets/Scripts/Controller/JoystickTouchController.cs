using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickTouchController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] public bl_Joystick Joystick;
    [SerializeField] private RectTransform handle;
    [SerializeField] private float handleRange = 100f;

    private RectTransform _canvasRect;
    private RectTransform _joystickRect;
    private Vector2 _startPos;

    private void Awake()
    {
        _canvasRect = GameManager.GetGameManager._canvas.GetComponent<RectTransform>();
        _joystickRect = Joystick.GetComponent<RectTransform>();
        Joystick.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, eventData.position, eventData.pressEventCamera, out _startPos);

        // ���̽�ƽ ��ġ ����
        _joystickRect.anchoredPosition = eventData.position;
        //handle.anchoredPosition = Vector2.zero;
        Joystick.gameObject.SetActive(true);

        Joystick.OnPointerDown(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, eventData.position, eventData.pressEventCamera, out localPos);

        //Vector2 direction = localPos - _startPos;
        //direction = Vector2.ClampMagnitude(direction, handleRange);
        //handle.anchoredPosition = direction;

        // ���Ⱚ ó��: direction.normalized ��
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        handle.anchoredPosition = Vector2.zero;
        Joystick.gameObject.SetActive(false);

        // ���Ⱚ �ʱ�ȭ
    }
}
