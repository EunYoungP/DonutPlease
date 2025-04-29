using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct IntercationData
{
    public int InteractionId;
    public InteractionType InteractionType;
}

public class UIInteraction : MonoBehaviour
{
    [SerializeField] private List<IntercationData> _interactionDatas;

    [SerializeField] private TextMeshProUGUI _textGold;
    [SerializeField] private Image _imgFilled;

    private bool _isTrigger;

    public void AddData(int interactionId, InteractionType type)
    {
        IntercationData intercationData = new IntercationData
        {
            InteractionId = interactionId,
            InteractionType = type
        };

        _interactionDatas.Add(intercationData);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isTrigger = true;

            StartCoroutine(Filled(1.5f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isTrigger = false;
        }
    }

    private IEnumerator Filled(float duration)
    {
        float elapsedTime = 0f;
        _imgFilled.fillAmount = 0;

        while (elapsedTime < duration)
        {
            if (!_isTrigger)
                yield break;

            _imgFilled.fillAmount = elapsedTime / duration;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        foreach (var intercationData in _interactionDatas)
        {
            FluxSystem.Dispatch(intercationData,  new OnTriggerEnterInteractionUI(intercationData.InteractionType));
        }

        Destroy(gameObject);
    }
}
