using UnityEngine;

public class FxOnUpdatePlayerStat : IFluxAction
{
    public readonly string fieldName;
    public readonly int increase;

    public FxOnUpdatePlayerStat(string fieldName, int increase)
    {
        this. fieldName = fieldName;
        this.increase = increase;
    }
}
