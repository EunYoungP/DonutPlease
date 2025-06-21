using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Collections;

public enum InteractionType
{
    OpenFrontDoor,
    CreateTable,
    CreateMachine,
    CreateCounter,
    CreateInteractionUI,
    OpenHR,
    OpenUpgrade,
    OpenDriveThru,
    TrashCan,
}

public class InteractionSystem : MonoBehaviour
{
    [SerializeField]
    private Transform InteractionRoot;

    [SerializeField]
    public List<InteractionProp> _interactionPropDatas;

    private GameManager GameManager => GameManager.GetGameManager;
    
    // 현재 활성화 된 UIInteraction
    public Dictionary<int, UIInteractionData> UIInteractionsInStore { get; private set; } = new();
    

    public  void Initialize(List<UIInteractionData> loadDatas)
    {
        foreach (var loadData in loadDatas)
            UIInteractionsInStore.Add(loadData.interactionId, loadData);

        InitializeInteraction();
    }

    private void InitializeInteraction()
    {
        foreach (var (id, UIInteraction) in UIInteractionsInStore)
        {
            if (UIInteraction.isComplete)
            {
                // 완료된 것들은 연결된 콜백중에 UI제외된 것들만 생성
                var propData = GameManager.LocalMap.GetInteractionPropData(id);

                foreach (int nextId in propData.NextIds)
                {
                    var connectDatas = GameManager.LocalMap.GetInteractionPropData(nextId);
                    if (connectDatas.Type == InteractionType.CreateInteractionUI)
                        continue;

                    ActiveInteraction(nextId);
                }
            }
            else
            {
                GameManager.LocalMap.CreateInteractionUI(id);
            }
        }

        if (UIInteractionsInStore.Count == 0)
            CreateInteractionUI(0);
    }

    public void ActiveInteraction(int interactionId)
    {
        var propData = GameManager.LocalMap.GetInteractionPropData(interactionId);
        var interactionType = propData.Type;

        if (interactionType == InteractionType.OpenFrontDoor)
        {
            CreateProp(interactionId);
        }
        else if (interactionType == InteractionType.CreateCounter)
        {
            CreateProp(interactionId);
        }
        else if (interactionType == InteractionType.CreateTable)
        {
            CreateProp(interactionId);
        }
        else if (interactionType == InteractionType.CreateMachine)
        {
            CreateProp(interactionId);
        }
        else if (interactionType == InteractionType.CreateInteractionUI)
        {
            CreateInteractionUI(interactionId);
        }
        else if (interactionType == InteractionType.OpenHR)
        {
            SetActiveProp(interactionId);
        }
        else if (interactionType == InteractionType.OpenUpgrade)
        {
            SetActiveProp(interactionId);
        }
        else if (interactionType == InteractionType.OpenDriveThru)
        {
            SetActiveProp(interactionId);
        }
        else if (interactionType == InteractionType.TrashCan)
        {
            SetActiveProp(interactionId);
        }
    }

    private void CreateProp(int id)
    {
        GameManager.LocalMap.CreateProp(id);
    }

    public void CreateInteractionUI(int id)
    {
        GameManager.LocalMap.CreateInteractionUI(id);

        UpdateInteractionInStore(id, false);
    }

    private void SetActiveProp(int id)
    {
        GameManager.LocalMap.SetActiveProp(id);
    }

    public bool IsCompleteUIInteractionDataInStore(int id)
    {
        if (UIInteractionsInStore.TryGetValue(id, out var uiInteractionData))
        {
            return uiInteractionData.isComplete;
        }
        return false;
    }

    public void UpdateInteractionInStore(int id, bool isComplete, int paidCash = 0)
    {
        UIInteractionData UIInteractionData;
        if (UIInteractionsInStore.TryGetValue(id, out var uiIntercationData))
        {
            uiIntercationData.isComplete = isComplete;
            uiIntercationData.paidCash = paidCash;

            UIInteractionData = uiIntercationData;
        }
        else
        {
            UIInteractionData = new UIInteractionData
            {
                interactionId = id,
                isComplete = isComplete,
                paidCash = paidCash
            };

            UIInteractionsInStore.Add(id, UIInteractionData);
        }

        GameManager.GetGameManager.Data.SaveUIIntercationData(UIInteractionData);
    }
}
