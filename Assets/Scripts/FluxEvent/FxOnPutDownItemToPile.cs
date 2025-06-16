using DonutPlease.Game.Character;

public class FxOnPutDownItemToPile : IFluxAction
{
    public readonly EItemType itemType;
    public readonly CharacterBase character;
    public readonly PileBase pile;

    public FxOnPutDownItemToPile(EItemType itemType, CharacterBase character, PileBase pile)
    {
        this.itemType = itemType;
        this.character = character;
        this.pile = pile;
    }
}
