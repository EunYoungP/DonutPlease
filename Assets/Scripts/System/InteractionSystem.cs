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

    private GameManager _gameManager;

    private readonly CompositeDisposable disposable = new();

    public  void Initialize()
    {
        _gameManager = GameManager.GetGameManager;

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
                OpenStore(interactionId);
            }
            else if (interactionType == InteractionType.CreateTable)
            {
                CreateTable(interactionId);
            }
            else if (interactionType == InteractionType.CreateMachine)
            {
                CreateMachine(interactionId);
            }
            else if (interactionType == InteractionType.CreateInteractionUI)
            {
                CreateInteractionUI(interactionId);
            }
        }
    }

    private void OpenStore(int id)
    {
        GameManager.GetGameManager.LocalMap.CreateProp(id);
    }

    private void CreateTable(int id)
    {
        GameManager.GetGameManager.LocalMap.CreateProp(id);
    }

    private void CreateMachine(int id)
    {
        GameManager.GetGameManager.LocalMap.CreateProp(id);
    }

    private void CreateInteractionUI(int id)
    {
        GameManager.GetGameManager.LocalMap.CreateInteractionUI(id);
    }
}
