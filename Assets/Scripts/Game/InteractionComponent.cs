using UniRx;
using UnityEngine;

public enum InteractionType
{
    Open,
    CreateTable,
    CreateMachine,
    CreateCounter,
    GetDonut,
    SetDonut,
    Calculate,
    GetMoney,
    ClearTable,
    DisposeTrash,
    HireStaff,
    DriveThru,
}

public class InteractionComponent : MonoBehaviour
{
    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        FluxSystem.ActionStream
            .Subscribe(interactionUI =>
            {
                ActiveInteraction(interactionUI);
            }).AddTo(this);
    }

    private void ActiveInteraction(object action)
    {
        if (action is OnTriggerEnterInteractionUI uiInteraction)
        {
            InteractionType interactionType = uiInteraction.interactionType;
            if (interactionType == InteractionType.Open)
            {
                OpenStore();
            }
            else if (interactionType == InteractionType.CreateTable)
            {
                // Handle CreateTable interaction
            }
            else if (interactionType == InteractionType.CreateMachine)
            {
                // Handle CreateMachine interaction
            }
            else if (interactionType == InteractionType.CreateCounter)
            {
                // Handle CreateCounter interaction
            }
            else if (interactionType == InteractionType.GetDonut)
            {
                // Handle GetDonut interaction
            }
            else if (interactionType == InteractionType.SetDonut)
            {
                // Handle SetDonut interaction
            }
        }
    }

    private void OpenStore()
    {

    }
}
