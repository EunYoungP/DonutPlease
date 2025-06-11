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
    OpenHR,
    OpenUpgrade,
    OpenDriveThru,
    TrashCan,
}

public class InteractionSystem
{
    [SerializeField]
    private Transform InteractionRoot;

    private GameManager GameManager => GameManager.GetGameManager;
    private readonly CompositeDisposable disposable = new();

    public  void Initialize()
    {
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
            //GameManager.LocalMap.OfficeUpgradeProps.SetActive(true);
        }
    }

    private void CreateProp(int id)
    {
        GameManager.LocalMap.CreateProp(id);
    }

    public void CreateInteractionUI(int id)
    {
        GameManager.LocalMap.CreateInteractionUI(id);
    }
}
