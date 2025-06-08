using NUnit.Framework;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public enum InteractionType
{
    OpenFrontDoor,
    CreateTable,
    CreateMachine,
    CreateCounter,
    CreateInteractionUI,
    HireStaff,
    CreateDriveThru,
    TrashCan,
}

public class InteractionSystem
{
    [SerializeField]
    private Transform InteractionRoot;

    private readonly CompositeDisposable disposable = new();

    public  void Initialize()
    {
        FluxSystem.ActionStream
            .Subscribe(data =>
            {
                ActiveInteraction(data);

            }).AddTo(disposable);
    }

    private void ActiveInteraction(object action)
    {
        if (action is OnTriggerEnterInteractionUI uiInteraction)
        {
            int interactionId = uiInteraction.interactionId;
            InteractionType interactionType = uiInteraction.interactionType;
            int nextInteractionId = uiInteraction.nextInerationId;

            if (interactionType == InteractionType.OpenFrontDoor)
            {
                CreateProp(interactionId, interactionType);
            }
            else if (interactionType == InteractionType.CreateCounter)
            {
                CreateProp(interactionId, interactionType);
            }
            else if (interactionType == InteractionType.CreateTable)
            {
                CreateProp(interactionId, interactionType);
            }
            else if (interactionType == InteractionType.CreateMachine)
            {
                CreateProp(interactionId, interactionType);
            }
            else if (interactionType == InteractionType.CreateInteractionUI)
            {
                CreateInteractionUI(interactionId, nextInteractionId);
            }
        }
    }

    private void CreateProp(int id, InteractionType type)
    {
        GameManager.GetGameManager.LocalMap.CreateProp(id, type);
    }

    private void CreateInteractionUI(int id, int nextInterationId)
    {
        GameManager.GetGameManager.LocalMap.CreateInteractionUI(id, nextInterationId);
    }
}
