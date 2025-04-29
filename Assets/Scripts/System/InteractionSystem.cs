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
                ActiveInteraction(data.Item1, data.Item2);

            }).AddTo(disposable);
    }

    private void ActiveInteraction(IntercationData intercationData, object action)
    {
        if (action is OnTriggerEnterInteractionUI uiInteraction)
        {
            InteractionType interactionType = uiInteraction.interactionType;
            if (interactionType == InteractionType.Open)
            {
                OpenStore(intercationData.InteractionId);
            }
            else if (interactionType == InteractionType.CreateTable)
            {
                CreateTable(intercationData.InteractionId);
            }
            else if (interactionType == InteractionType.CreateMachine)
            {
                CreateMachine(intercationData.InteractionId);
            }
            else if (interactionType == InteractionType.CreateInteractionUI)
            {
                CreateInteractionUI(intercationData.InteractionId);
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
