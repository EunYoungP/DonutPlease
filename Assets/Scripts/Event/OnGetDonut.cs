using DonutPlease.Game.Character;
using UnityEngine;

public class OnGetDonut
{
    public readonly GameObject donut;
    public readonly CharacterBase character;

    public OnGetDonut(GameObject donut, CharacterBase character)
    {
        this.donut = donut;
        this.character = character;
    }
}
