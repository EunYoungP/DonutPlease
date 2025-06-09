using UnityEngine;
using UnityEngine.UI;

public class ExpBar : MonoBehaviour
{
    [SerializeField] private Image _expBar;
    [SerializeField] private Button _expImgButton;
    [SerializeField] private Text _expText;

    private void Awake()
    {
        _expImgButton.onClick.AddListener(() =>
        {
            Debug.Log("Exp bar button clicked!");
        });
    }

    public void SetExp(int curExp, int totalExp)
    {
        _expText.text = curExp.ToString();
        _expBar.fillAmount = (float)curExp / totalExp;
    }
}
