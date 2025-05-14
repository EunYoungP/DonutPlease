using DonutPlease.Game.Character;
using UnityEngine;

public class OnPutDownDonut
{
    public readonly CharacterBase character;
    public readonly PileBase pile;

    public OnPutDownDonut(CharacterBase character, PileBase pile)
    {
        this.character = character;
        this.pile = pile;
    }
}
