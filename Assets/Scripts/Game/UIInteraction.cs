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

    private Action _callbacks;
    private bool _isTrigger;
    private float _duration = 0.1f;

    private int payUnit = 10;
    private int needCash;
    private int paidCash = 0;

    public int Id { get; private set; }
    public bool IsPaidComplete => paidCash >= needCash;


    private void Awake()
    {
        _imgFilled.fillAmount = 0f;
    }

    public void SetId(int interactionId)
    {
        Id = interactionId;

        needCash = GameManager.GetGameManager.LocalMap.GetPropData(Id).NeedCash;
        _textGold.text = needCash.ToString();
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
            if (!GameManager.GetGameManager.Player.Currency.Pay(payUnit))
            {
                payUnit = GameManager.GetGameManager.Player.Currency.Cash;
                if (payUnit == 0)
                    yield break;

                GameManager.GetGameManager.Player.Currency.Pay(payUnit);
            }

            // 돈 더미 하나 받기
           yield return StartCoroutine(CoPayToInteractionUI());
        }

        // 돈 지불 완료
        if (IsPaidComplete)
        {
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
            paidCash += payUnit;

            _textGold.text = (needCash - paidCash).ToString();
            _imgFilled.fillAmount = (float)paidCash / needCash;
        });
    }
}
