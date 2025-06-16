using DonutPlease.Game.Character;
using UnityEngine;

public class FxOnTriggerEnter : IFluxAction
{
    public readonly CharacterBase characterBase;
    public readonly EColliderIdentifier colliderType;

    public FxOnTriggerEnter(CharacterBase characterBase, EColliderIdentifier colliderType)
    {
        this.characterBase = characterBase;
        this.colliderType = colliderType;
    }
}
