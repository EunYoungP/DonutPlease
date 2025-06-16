using DonutPlease.Game.Character;
using UnityEngine;

public class FxOnTriggerExit : IFluxAction
{
    public readonly CharacterBase characterBase;
    public readonly EColliderIdentifier colliderType;

    public FxOnTriggerExit(CharacterBase characterBase, EColliderIdentifier colliderType)
    {
        this.characterBase = characterBase;
        this.colliderType = colliderType;
    }
}
