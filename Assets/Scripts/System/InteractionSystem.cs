using System.Collections.Generic;
using UnityEngine;

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
    
    // 현재 활성화 된 UIInteraction
    public Dictionary<int, UIInteractionData> UIInteractionsInStore { get; private set; } = new();

    private GameManager GameManager => GameManager.GetGameManager;

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
                var propData = GameManager.LocalMap.GetPropData(id);

                foreach (int nextId in propData.NextIds)
                {
                    var connectDatas = GameManager.LocalMap.GetPropData(nextId);
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
        var propData = GameManager.LocalMap.GetPropData(interactionId);
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
            GameManager.LocalMap.OfficeHRProps.SetActive(true);
        }
        else if (interactionType == InteractionType.OpenUpgrade)
        {
            GameManager.LocalMap.OfficeUpgradeProps.SetActive(true);
        }
        else if (interactionType == InteractionType.OpenDriveThru)
        {
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
