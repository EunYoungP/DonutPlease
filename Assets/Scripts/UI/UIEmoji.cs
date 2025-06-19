using UnityEngine;
using UnityEngine.UI;

public class UIEmoji : MonoBehaviour
{
    [SerializeField] private Image _image;

    public RectTransform RectTransform { get; private set; }

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
    }

    public void SetImage(string emojiName)
    {
        var Resource = GameManager.GetGameManager.Resource;
        _image.sprite = Resource.GetSprite(Resource.GetImagePath(emojiName));
    }
}
