using NUnit.Framework;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public enum InteractionType
{
    Open,
    CreateTable,
    CreateMachine,
    CreateCounter,
    CreateInteractionUI,
    HireStaff,
    CreateDriveThru,
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

            if (interactionType == InteractionType.Open)
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
                CreateInteractionUI(interactionId);
            }
        }
    }

    private void CreateProp(int id, InteractionType type)
    {
        GameManager.GetGameManager.LocalMap.CreateProp(id, type);
    }

    private void CreateInteractionUI(int id)
    {
        GameManager.GetGameManager.LocalMap.CreateInteractionUI(id);
    }
}
