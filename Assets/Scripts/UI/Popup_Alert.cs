using TMPro;
using UnityEngine;

public class Popup_Alert : UIPopup
{
    [SerializeField] private TextMeshProUGUI _descText;

    public void SetText(string message)
    {
        if (_descText != null)
        {
            _descText.text = message;
        }
    }
}
