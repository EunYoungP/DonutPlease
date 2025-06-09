using UnityEngine;
using UnityEngine.UI;

public class TextImage : MonoBehaviour
{
    [SerializeField] private Text _text;

    public void SetText(int value)
    {
        _text.text = value.ToString();
    }
}
