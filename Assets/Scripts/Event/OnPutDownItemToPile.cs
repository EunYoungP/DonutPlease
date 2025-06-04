using DonutPlease.Game.Character;
using UnityEngine;

public class OnPutDownItemToPile
{
    public readonly EItemType itemType;
    public readonly CharacterBase character;
    public readonly PileBase pile;

    public OnPutDownItemToPile(EItemType itemType, CharacterBase character, PileBase pile)
    {
        this.itemType = itemType;
        this.character = character;
        this.pile = pile;
    }
}
