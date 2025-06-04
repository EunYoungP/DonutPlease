using DonutPlease.Game.Character;
using UnityEngine;

public class OnGetItem
{
    public readonly EItemType itemType;
    public readonly GameObject item;
    public readonly CharacterBase character;

    public OnGetItem(EItemType itemType, GameObject item, CharacterBase character)
    {
        this.item = item;
        this.character = character;
        this.itemType = itemType;
    }
}
