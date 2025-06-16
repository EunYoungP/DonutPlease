using DonutPlease.Game.Character;
using UnityEngine;

public class FxOnGetItem : IFluxAction
{
    public readonly EItemType itemType;
    public readonly GameObject item;
    public readonly CharacterBase character;

    public FxOnGetItem(EItemType itemType, GameObject item, CharacterBase character)
    {
        this.item = item;
        this.character = character;
        this.itemType = itemType;
    }
}
