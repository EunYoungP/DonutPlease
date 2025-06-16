using UnityEngine;

public class FxOnUpdateHRStat : IFluxAction
{
    public readonly string fieldName;
    public readonly int increase;

    public FxOnUpdateHRStat(string fieldName, int increase)
    {
        this.fieldName = fieldName;
        this.increase = increase;
    }
}
