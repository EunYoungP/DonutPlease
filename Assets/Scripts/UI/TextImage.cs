using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextImage : UIBehaviour
{
    [SerializeField] private Text _text;

    public void SetText(int value)
    {
        _text.text = value.ToString();
    }
}
