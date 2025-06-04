using DonutPlease.Game.Character;
using UnityEngine;

public class OnPutDownItemToPos
{
    public readonly EItemType itemType;
    public readonly CharacterBase character;
    public readonly Transform transform;

    public OnPutDownItemToPos(EItemType itemType, CharacterBase character, Transform transform)
    {
        this.itemType = itemType;
        this.character = character;
        this.transform = transform;
    }
}
