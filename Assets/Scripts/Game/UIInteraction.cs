using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DonutEditor;
using DG.Tweening;
using UnityEngine.TextCore.Text;

[Serializable]
public class IntercationData
{
    public int InteractionId;
    public InteractionType InteractionType;

    [ShowEnum("InteractionType", InteractionType.CreateInteractionUI)]
    public int NextInteractionId;
}

public class UIInteraction : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textGold;
    [SerializeField] private Image _imgFilled;

    private float _duration = 0.1f;
    private int _payUnit = 10;
    private int _paidCash = 0;

    private int _needCash;
    private int _rewardExp;

    private bool _isTrigger;
    private Action _callbacks;

    public int Id { get; private set; }
    public bool IsPaidComplete => _paidCash >= _needCash;


    private void Awake()
    {
        _imgFilled.fillAmount = 0f;
    }

    public void SetId(int interactionId)
    {
        Id = interactionId;

        _needCash = GameManager.GetGameManager.LocalMap.GetPropData(Id).NeedCash;
        _rewardExp = GameManager.GetGameManager.LocalMap.GetPropData(Id).RewardExp;
        _textGold.text = _needCash.ToString();
    }

    public void AddCallback(Action callback)
    {
        _callbacks += callback;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isTrigger = true;

            StartCoroutine(CoPayCash());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isTrigger = false;
        }
    }

    private IEnumerator CoPayCash()
    {
        while (_isTrigger && !IsPaidComplete)
        {
            // 플레이어 재화 갱신
            if (!GameManager.GetGameManager.Player.Currency.Pay(_payUnit))
            {
                _payUnit = GameManager.GetGameManager.Player.Currency.Cash;
                if (_payUnit == 0)
                    yield break;

                GameManager.GetGameManager.Player.Currency.Pay(_payUnit);

                // 경험치 받기
            }

            // 돈 더미 이동
           yield return StartCoroutine(CoPayToInteractionUI());
        }

        // 돈 지불 완료
        if (IsPaidComplete)
        {
            GameManager.GetGameManager.Player.Growth.AddExp(_rewardExp);

            _callbacks?.Invoke();

            Destroy(gameObject);
        }
    }

    private IEnumerator CoPayToInteractionUI()
    {
        float elapsedTime = 0f;
        while (elapsedTime < _duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Cash 이동
        var player = GameManager.GetGameManager.Player;
        player.Character.RemoveCashFromPlayerTo(gameObject.transform, () =>
        {
            _paidCash += _payUnit;

            _textGold.text = (_needCash - _paidCash).ToString();
            _imgFilled.fillAmount = (float)_paidCash / _needCash;
        });
    }
}
